using Cls;
using Google.Protobuf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MicroFeel.CLS
{
    public class ClsClient
    {

        /// <summary>
        /// 签名默认过期时间
        /// </summary>
        private const int ExpireSeconds = 60;
        /// <summary>
        /// 日志服务的基地址
        /// </summary>
        private readonly string BaseAddress;
        /// <summary>
        /// 签名生成器
        /// </summary>
        private readonly Sign sign;

        public const string JsonFormat = "application/json";
        public const string PBFormat = "application/x-protobuf";

        private readonly JsonSerializerSettings jsSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        /// <summary>
        /// 保存最近的实例 
        /// </summary>
        public static ClsClient Client;


        /// <summary>
        /// 创建日志服务管理对象
        /// </summary>
        /// <param name="endpoint">终结点</param>
        /// <param name="secretId">APPID</param>
        /// <param name="secretKey">APPKEY</param>
        public ClsClient(string endpoint, string secretId, string secretKey)
        {
            BaseAddress = endpoint;
            //创建签名器实例
            sign = new Sign(secretId, secretKey);
            Client = this;
        }

        #region 日志管理

        #endregion

        #region 通用数据操作
        /// <summary>
        /// Create an Object
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="query">Query path</param>
        /// <param name="t">object</param>
        /// <returns>Object</returns>
        public async Task<T> CreateAsync<T>(string query, T t, string mediaType = JsonFormat) where T : class
        {
            var uri = new Uri($"http://{BaseAddress}/{query}");
            HttpResponseMessage rsp;
            if (mediaType == JsonFormat)
            {
                var body = JsonConvert.SerializeObject(t, jsSetting);
                rsp = await SendRequest(uri, body, HttpMethod.Post, mediaType);
                return await GetResponseObjectAsync<T>(rsp);
            }
            else
            {
                if (!(t is LogGroupList))
                {
                    throw new TypeLoadException("传输类型必须是LostGroupList!");
                }
                var logGroupList = t as LogGroupList;
                var data = logGroupList.ToByteArray();
                rsp = await SendRequest(uri, data, HttpMethod.Post);
                rsp.EnsureSuccessStatusCode();
                return null;
            }
        }

        /// <summary>
        /// Get an object infomation
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="query">Query path</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string query, string mediaType = JsonFormat) where T : class
        {
            var uri = new Uri($"http://{BaseAddress}/{query}");
            var rsp = await SendRequest(uri, "", HttpMethod.Get, mediaType);
            return await GetResponseObjectAsync<T>(rsp);
        }

        /// <summary>
        /// Update Object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="query">Request query</param>
        /// <param name="t">A Object of Type T</param>
        /// <returns>Null</returns>
        public async Task UpdateAsync<T>(string query, T t, string mediaType = JsonFormat) where T : class
        {
            var uri = new Uri($"http://{BaseAddress}/{query}");
            var body = JsonConvert.SerializeObject(t, jsSetting);

            var rsp = await SendRequest(uri, body, HttpMethod.Put, mediaType);
            rsp.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// DELETE A Object
        /// </summary>
        /// <param name="query">QueryString</param>
        public async Task DeleteAsync(string query)
        {
            var uri = new Uri($"http://{BaseAddress}/{query}");
            var rsp = await SendRequest(uri, "", HttpMethod.Delete);
            rsp.EnsureSuccessStatusCode();
        }
        #endregion

        /// <summary>
        /// 发送HTTP请求
        /// </summary>
        /// <param name="uri">目标地址</param>
        /// <param name="body">请求内容</param>
        /// <param name="method">请求方法</param>
        /// <returns>HttpResponseMessage实例对象</returns>
        private async Task<HttpResponseMessage> SendRequest(Uri uri, string body, HttpMethod method, string mediaType = JsonFormat)
        {
            var headers = new Dictionary<string, string> { { "Host", BaseAddress } };

            using (var httpmsg = new HttpRequestMessage { RequestUri = uri, Method = method, Content = new StringContent(body, Encoding.UTF8, mediaType) })
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Host = BaseAddress;
                if (method == HttpMethod.Put || method == HttpMethod.Post)
                {
                    httpmsg.Content.Headers.Add("Content-Length", body.Length.ToString());

                    using (var md5 = MD5.Create())
                    {
                        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(body));
                        headers.Add("Content-MD5", BitConverter.ToString(hash).Replace("-", "").ToLower());
                        httpmsg.Content.Headers.Add("Content-MD5", headers["Content-MD5"]);
                    }
                }
                return await SendRequest(uri, method, headers, httpmsg, httpClient);
            }
        }

        private async Task<HttpResponseMessage> SendRequest(Uri uri, byte[] data, HttpMethod method)
        {
            var headers = new Dictionary<string, string> { { "Host", BaseAddress } };

            using (var httpmsg = new HttpRequestMessage { RequestUri = uri, Method = method, Content = new ByteArrayContent(data) })
            using (var httpClient = new HttpClient())
            {
                httpmsg.Content.Headers.ContentType = new MediaTypeHeaderValue(PBFormat);
                httpClient.DefaultRequestHeaders.Host = BaseAddress;
                //签名
                return await SendRequest(uri, method, headers, httpmsg, httpClient);
            }
        }

        /// <summary>
        /// 签名并发送网络请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="httpmsg"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendRequest(Uri uri, HttpMethod method, Dictionary<string, string> headers, HttpRequestMessage httpmsg, HttpClient httpClient)
        {
            //签名
            var signtime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var qkeytime = $"{signtime};{signtime + ExpireSeconds}";
            var signStr = sign.Signature(uri, method.Method, qkeytime, headers);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", signStr);
            return await httpClient.SendAsync(httpmsg);
        }

        /// <summary>
        /// 解析回应到对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="rsp">回应消息</param>
        /// <returns>从网络回应生成对象实例</returns>
        private async Task<T> GetResponseObjectAsync<T>(HttpResponseMessage rsp) where T : class
        {
            rsp.EnsureSuccessStatusCode();

            if (typeof(LogGroupList) == typeof(T))
            {
                var rspBytes = await rsp.Content.ReadAsByteArrayAsync();
                return LogGroupList.Parser.ParseFrom(rspBytes) as T;
            }
            else
            {
                var rspStr = await rsp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(rspStr);
            }
        }

    }
}
