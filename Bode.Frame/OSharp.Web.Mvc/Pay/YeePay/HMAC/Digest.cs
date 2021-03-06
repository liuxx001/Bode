﻿using System;
using System.Text;
using System.Web;

namespace OSharp.Web.Mvc.Pay.YeePay
{
    /// <summary>
    /// Digest 
    /// </summary>
    public abstract class Digest
    {
        public Digest() { }

        #region 获取HMAC

        /// <summary>
        /// 获取支付HMAC
        /// </summary>
        /// <param name="customernumber"></param>
        /// <param name="requestid"></param>
        /// <param name="amount"></param>
        /// <param name="assure"></param>
        /// <param name="productname"></param>
        /// <param name="productcat"></param>
        /// <param name="productdesc"></param>
        /// <param name="divideinfo"></param>
        /// <param name="callbackurl"></param>
        /// <param name="webcallbackurl"></param>
        /// <param name="bankid"></param>
        /// <param name="period"></param>
        /// <param name="memo"></param>
        /// <param name="hmacKey"></param>
        /// <returns></returns>
        public static string GetHMAC(
            string customernumber, 
            string requestid, 
            string amount, 
            string assure, 
            string productname, 
            string productcat, 
            string productdesc, 
            string divideinfo, 
            string callbackurl, 
            string webcallbackurl, 
            string bankid, 
            string period, 
            string memo, 
            string hmacKey)
        {
            string sign = "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}";
            sign = string.Format(sign, customernumber, requestid, amount, assure, productname, productcat, productdesc, divideinfo, callbackurl, webcallbackurl, bankid, period, memo);

            return HmacSign(sign, hmacKey);
        }

        /// <summary>
        /// 获取注册子账户HMAC
        /// </summary>
        /// <param name="customernumber"></param>
        /// <param name="requestid"></param>
        /// <param name="bindmobile"></param>
        /// <param name="customertype"></param>
        /// <param name="signedname"></param>
        /// <param name="linkman"></param>
        /// <param name="idcard"></param>
        /// <param name="businesslicence"></param>
        /// <param name="legalperson"></param>
        /// <param name="minsettleamount"></param>
        /// <param name="riskreserveday"></param>
        /// <param name="bankaccountnumber"></param>
        /// <param name="bankname"></param>
        /// <param name="accountname"></param>
        /// <param name="bankaccounttype"></param>
        /// <param name="bankprovince"></param>
        /// <param name="bankcity"></param>
        /// <param name="hmacKey"></param>
        /// <returns></returns>
        public static string GetRegisterHMAC(
            string customernumber, 
            string requestid, 
            string bindmobile, 
            string customertype, 
            string signedname, 
            string linkman,
            string idcard, 
            string businesslicence, 
            string legalperson, 
            string minsettleamount, 
            string riskreserveday, 
            string bankaccountnumber, 
            string bankname, 
            string accountname, 
            string bankaccounttype, 
            string bankprovince, 
            string bankcity, 
            string hmacKey)
        {
            string sign = "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}";
            sign = string.Format(sign, customernumber, requestid, bindmobile, customertype, signedname, linkman, idcard, businesslicence, legalperson, minsettleamount, riskreserveday, bankaccountnumber, bankname, accountname, bankaccounttype, bankprovince, bankcity);

            return HmacSign(sign, hmacKey);
        }

        #endregion

        #region 验证HMAC

        /// <summary>
        /// 支付请求HMAC验证
        /// </summary>
        /// <param name="customernumber"></param>
        /// <param name="requestid"></param>
        /// <param name="code"></param>
        /// <param name="externalid"></param>
        /// <param name="amount"></param>
        /// <param name="payurl"></param>
        /// <returns></returns>
        public static bool PayRequestVerifyHMAC(string customernumber, string requestid, int code, string externalid, string amount, string payurl, string hmacKey, string VSign)
        {
            string sign = "{0}{1}{2}{3}{4}{5}";


            sign = string.Format(sign, customernumber, requestid, code, externalid, amount, payurl);

            sign = HmacSign(sign, hmacKey);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(sign, VSign))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 支付结果HMAC验证
        /// </summary>
        /// <param name="customernumber"></param>
        /// <param name="requestid"></param>
        /// <param name="code"></param>
        /// <param name="notifytype"></param>
        /// <param name="externalid"></param>
        /// <param name="amount"></param>
        /// <param name="cardno"></param>
        /// <param name="hmacKey"></param>
        /// <param name="VSign"></param>
        /// <returns></returns>
        public static bool PayResultVerifyHMAC(string customernumber, string requestid, int code, string notifytype, string externalid, string amount, string cardno, string hmacKey, string VSign)
        {
            string sign = "{0}{1}{2}{3}{4}{5}{6}";
            sign = string.Format(sign, customernumber, requestid, code, notifytype, externalid, amount, cardno);
            sign = HmacSign(sign, hmacKey);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(sign, VSign))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 私有方法

        private static string HmacSign(string aValue, string aKey)
        {
            byte[] k_ipad = new byte[64];
            byte[] k_opad = new byte[64];
            byte[] keyb;
            byte[] Value;
            keyb = Encoding.UTF8.GetBytes(aKey);
            Value = Encoding.UTF8.GetBytes(aValue);

            for (int i = keyb.Length; i < 64; i++)
                k_ipad[i] = 54;

            for (int i = keyb.Length; i < 64; i++)
                k_opad[i] = 92;

            for (int i = 0; i < keyb.Length; i++)
            {
                k_ipad[i] = (byte)(keyb[i] ^ 0x36);
                k_opad[i] = (byte)(keyb[i] ^ 0x5c);
            }

            HmacMD5 md = new HmacMD5();

            md.update(k_ipad, (uint)k_ipad.Length);
            md.update(Value, (uint)Value.Length);
            byte[] dg = md.finalize();
            md.init();
            md.update(k_opad, (uint)k_opad.Length);
            md.update(dg, 16);
            dg = md.finalize();

            return toHex(dg);
        }

        private static string toHex(byte[] input)
        {
            if (input == null)
                return null;

            StringBuilder output = new StringBuilder(input.Length * 2);

            for (int i = 0; i < input.Length; i++)
            {
                int current = input[i] & 0xff;
                if (current < 16)
                    output.Append("0");
                output.Append(current.ToString("x"));
            }

            return output.ToString();
        }

        #endregion
    }
}
