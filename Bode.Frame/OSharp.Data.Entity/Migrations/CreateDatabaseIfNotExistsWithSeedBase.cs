﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace OSharp.Data.Entity.Migrations
{
    /// <summary>
    /// 在数据库不存在时使用种子数据创建数据库
    /// </summary>
    public abstract class CreateDatabaseIfNotExistsWithSeedBase<TDbContext> : CreateDatabaseIfNotExists<TDbContext> where TDbContext : DbContext
    {
        /// <summary>
        /// 初始化一个<see cref="CreateDatabaseIfNotExistsWithSeedBase{TDbContext}"/>类型的新实例
        /// </summary>
        protected CreateDatabaseIfNotExistsWithSeedBase()
        {
            SeedActions = new List<ISeedAction<TDbContext>>();
        }

        /// <summary>
        /// 获取 数据迁移初始化种子数据操作信息集合，各个模块可以添加自己的数据初始化操作
        /// </summary>
        public ICollection<ISeedAction<TDbContext>> SeedActions { get; private set; }

        /// <summary>
        /// 获取实体映射程序集
        /// </summary>
        protected ICollection<Assembly> MapperAssemblies { get; set; }

        /// <summary>
        /// 获取迁移种子数据
        /// </summary>
        public void InitSeedActions()
        {
            if (MapperAssemblies.Count == 0)
            {
                throw new InvalidOperationException(string.Format("上下文种子数据“{0}”初始化失败，实体映射程序集不存在", this.GetType().FullName));
            }
            Type baseType = typeof(ISeedAction<TDbContext>);
            Type[] seedTypes = MapperAssemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type) && type != baseType && !type.IsAbstract).ToArray();
            SeedActions = seedTypes.Select(type => Activator.CreateInstance(type) as ISeedAction<TDbContext>).ToList();
        }

        /// <summary>
        /// 重写Seed方法
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(TDbContext context)
        {
            if (!SeedActions.Any())
            {
                InitSeedActions();
            }
            IEnumerable<ISeedAction<TDbContext>> seedActions = SeedActions.OrderBy(m => m.Order);
            foreach (ISeedAction<TDbContext> seedAction in seedActions)
            {
                seedAction.Action(context);
            }
        }
    }
}