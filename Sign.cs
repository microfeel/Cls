using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MicroFeel.CLS
{
    /// <summary>
    /// 日志服务签名类
    /// </summary>
    public class Sign
    {
        private readonly string SecretId;
        private readonly string SecretKey;

        public Sign(string secretId, string secretKey)
        {
            SecretId = secretId;
            SecretKey = secretKey;
        }

        ///<summary>生成签名</summary>
        ///<param name="signStr">被加密串</param>
        ///<param name="secret">加密密钥</param>
        ///<returns>签名</returns>
        ///<seealso cref="https://cloud.tencent.com/document/product/614/12445"/>
        public string Signature(Uri uri, string method, string qkeytime, IDictionary<string, string> headers, SignatureMethod signatureMethod = SignatureMethod.sha1)
        {
            //1.按照一定格式拼接Http请求的相关信息为字符串 HttpRequestInfo。
            //不能使用HttpUtility.UrlEncode!  存在大小写转换问题
            var headersString = string.Join("&", headers.Select(h => $"{h.Key.ToLower()}={WebUtility.UrlEncode(h.Value)}").OrderBy(h => h, StringComparer.Ordinal));

            var HttpRequestInfo = $"{method.ToLower()}\n{uri.LocalPath}\n{uri.Query.Replace("?", "")}\n{headersString}\n";

            //2.对 HttpRequestInfo 使用 sha1 算法计算哈希值，与其他指定参数按照一定格式组成签名原串 StringToSign。
            string hashReqInfo;
            switch (signatureMethod)
            {
                case SignatureMethod.sha1:
                    using (var sha1 = new SHA1Managed())
                    {
                        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(HttpRequestInfo));
                        hashReqInfo = BitConverter.ToString(hash).Replace("-", "").ToLower();
                    }
                    break;
                default:
                    throw new ArgumentException($"未知的签名加密类型{signatureMethod.ToString()}");
            }

            var StringToSign = $"{signatureMethod}\n{qkeytime}\n{hashReqInfo}\n";
            //3.使用 SecretKey 对 q-key-time 进行加密，得到 SignKey。
            var SignKey = SecretString(qkeytime, SecretKey);
            //4.使用 SignKey 对 StringToSign 进行加密，生成 Signature。
            var Signature = SecretString(StringToSign, SignKey);
            return $"q-sign-algorithm={signatureMethod}&q-ak={SecretId}&q-sign-time={qkeytime}&q-key-time={qkeytime}&q-header-list={string.Join(";", headers.Select(v => v.Key.ToLower()).OrderBy(v => v, StringComparer.Ordinal))}&q-url-param-list={string.Join(";", uri.Query.Replace("?", "").Split('&').Select(kv => kv.Split('=').First()).OrderBy(v => v, StringComparer.Ordinal))}&q-signature={Signature}";
        }

        ///<summary>生成签名</summary>
        ///<param name="signStr">被加密串</param>
        ///<param name="secret">加密密钥</param>
        ///<returns>签名</returns>
        private static string SecretString(string signStr, string secret, SignatureMethod signatureMethod = SignatureMethod.sha1)
        {
            switch (signatureMethod)
            {
                case SignatureMethod.sha1:
                    using (HMACSHA1 mac = new HMACSHA1(Encoding.UTF8.GetBytes(secret)))
                    {
                        byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(signStr));
                        return BitConverter.ToString(hash).Replace("-", "").ToLower();
                    }
                default:
                    throw new ArgumentException($"未知的签名加密类型{signatureMethod.ToString()}");
            }
        }
    }

    public enum SignatureMethod
    {
        sha1
    }
}
