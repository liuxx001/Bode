﻿// <autogenerated>
//   This file was generated by T4 code generator DtoMappersCodeScript.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>



using AutoMapper;
        using Bode.Services.Core.Models.User;
        using Bode.Services.Core.Dtos.User;
        
namespace Bode.Services.Core.Dtos
{
	public partial class DtoMappers
	{
        public static void MapperRegister()
        {
                                Mapper.CreateMap<FeedBackDto, FeedBack>();
                                        Mapper.CreateMap<UserInfoDto, UserInfo>();
                                        Mapper.CreateMap<ValidateCodeDto, ValidateCode>();
                    
            MapperRegisterCustom();
        }

       static partial void MapperRegisterCustom();
	}
}