//using Microsoft.Extensions.Http;
using Cls;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MicroFeel.CLS
{
    public class LogObject
    {
        /// <summary>
        /// 日志属于的 topic id
        /// </summary>
        public string topic_id { get; set; }
        /// <summary>
        /// 日志主题的名字
        /// </summary>
        public string topic_name { get; set; }
        /// <summary>
        /// 日志时间
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        public string content { get; set; }

        public LogObject()
        {
        }

        public static async Task UploadAsync(string topicid, LogGroupList logGroupList)
        {
            await ClsClient.Client.CreateAsync($"structuredlog?topic_id={topicid}", logGroupList, ClsClient.PBFormat);
        }

        public static async Task<string> GetCursorAsync(string topicid, DateTime start)
        {
            return (await ClsClient.Client.GetAsync<LogCursor>($"cursor?topic_id={topicid}&start={WebUtility.UrlEncode(start.ToString("yyyy-MM-dd HH:mm:ss"))}")).cursor;
        }

        public static async Task<string> SearchAsync(string topicid, DateTime start)
        {
            return (await ClsClient.Client.GetAsync<LogCursor>($"searchlog?topic_id={topicid}&start={WebUtility.UrlEncode(start.ToString("yyyy-MM-dd HH:mm:ss"))}")).cursor;
        }

        public static async Task<LogGroupList> DownloadAsync(string topicid, string cursor, int count = 10)
        {
            return await ClsClient.Client.GetAsync<LogGroupList>($"log?topic_id={topicid}&cursor={cursor}&count={count}");
        }
    }

    public class LogCursor
    {
        /// <summary>
        /// 游标
        /// </summary>
        public string cursor;
    }
}
