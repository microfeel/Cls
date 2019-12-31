using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MicroFeel.CLS
{
    /// <summary>
    /// 投递任务
    /// </summary>
    public class Shipper
    {
        /// <summary>
        /// 投递的 ID
        /// </summary>
        public string shipper_id { get; set; }
        /// <summary>
        /// 所属的 topic id
        /// </summary>
        public string topic_id { get; set; }
        /// <summary>
        /// 投递的 bucket，格式：{bucketName}-{appid}
        /// </summary>
        public string bucket { get; set; }
        /// <summary>
        /// 投递目录的前缀
        /// </summary>
        public string prefix { get; set; }
        /// <summary>
        /// 投递规则的名字
        /// </summary>
        public string shipper_name { get; set; }
        /// <summary>
        /// 	投递的时间间隔，单位 秒，默认 300，范围 60 ~ 3600
        /// </summary>
        public int interval { get; set; }
        /// <summary>
        /// 	投递的文件的最大值，单位 MB，默认 256，范围 100 ~ 10240
        /// </summary>
        public int max_size { get; set; }
        /// <summary>
        /// 是否生效
        /// </summary>
        public bool effective { get; set; }
        /// <summary>
        /// 投递日志的过滤规则.匹配的日志进行投递，各 rule 之间是and关系，最多5个，数组为空则表示不过滤而全部投递
        /// </summary>
        public FilterRule filter_rules { get; set; }
        /// <summary>
        /// 投递日志的创建时间
        /// </summary>
        public string create_time { get; set; }
        /// <summary>
        /// 投递日志的分区规则，支持strftime的时间格式表示
        /// </summary>
        public string partition { get; set; }
        /// <summary>
        /// 投递日志的压缩配置
        /// </summary>
        public FormatCfg compress { get; set; }
        /// <summary>
        /// 投递日志的内容格式配置
        /// </summary>
        public FormatCfg content { get; set; }


        public Shipper(string topicid, string bucket, string prefix, string name, int interval)
        {
            topic_id = topicid;
            this.bucket = bucket;
            this.prefix = prefix;
            shipper_name = name;
            this.interval = interval;
        }

        /// <summary>
        /// 创建投递任务
        /// </summary>
        /// <param name="shipper">投递任务对象</param>
        /// <returns>投递任务id</returns>
        public static async Task<string> CreateAsync(Shipper shipper)
        {
            return (await ClsClient.Client.CreateAsync("shipper", shipper)).shipper_id;
        }

        /// <summary>
        /// 删除投递配置
        /// </summary>
        /// <param name="id">投递任务id</param>
        /// <returns>Null</returns>
        public static async Task DeleteAsync(string id)
        {
            await ClsClient.Client.DeleteAsync($"shipper?shipper_id={id}");
        }

        /// <summary>
        /// 获取投递任务列表
        /// </summary>
        /// <param name="shipperid">查询的投递规则 id</param>
        /// <param name="dtstart">查询的开始时间，支持最近 3 天的查询</param>
        /// <param name="dtEnd">查询的结束时间</param>
        /// <returns>投递任务列表</returns>
        public static async Task<IEnumerable<PostTask>> GetTaskListAsync(string shipperid, DateTime dtstart, DateTime dtEnd)
        {
            return (await ClsClient.Client.GetAsync<PostTasks>($"tasks?shipper_id={shipperid}&start_time={WebUtility.UrlEncode(dtstart.ToString("yyyy-MM-dd HH:mm:ss"))}&end_time={WebUtility.UrlEncode(dtEnd.ToString("yyyy-MM-dd HH:mm:ss"))}")).tasks;
        }

        /// <summary>
        /// 获取投递信息
        /// </summary>
        /// <param name="topicid">主题id</param>
        /// <param name="offset">查询的起始位置，默认 0</param>
        /// <param name="count">查询的个数，默认 50，最大 1000</param>
        /// <returns>投递任务实例</returns>
        public static async Task<IEnumerable<Shipper>> GetShipperListAsync(string topicid, int offset = 0, int count = 50)
        {
            return (await ClsClient.Client.GetAsync<Shippers>($"shippers?topic_id={topicid}&offset={offset}&count={count}")).shippers;
        }

        /// <summary>
        /// 更新投递任务
        /// </summary>
        /// <param name="shipper">投递任务对象</param>
        /// <returns>Null</returns>
        public static async Task UpdateAsync(Shipper shipper)
        {
            await ClsClient.Client.UpdateAsync("shipper", shipper);
        }

        /// <summary>
        /// 重试失败的任务
        /// </summary>
        /// <param name="shipperid">投递对象id</param>
        /// <param name="taskid">任务id</param>
        /// <returns>Null</returns>
        public static async Task RetryFailedTasksAsync(string shipperid, string taskid)
        {
            await ClsClient.Client.UpdateAsync("task", new { shipper_id = shipperid, task_id = taskid });
        }

    }

    public class Shippers
    {
        public IEnumerable<Shipper> shippers;
    }

    public class PostTask
    {
        /// <summary>
        /// 投递任务的 ID
        /// </summary>
        public string task_id { get; set; }
        /// <summary>
        /// 投递规则的 ID
        /// </summary>
        public string shipper_id { get; set; }
        /// <summary>
        /// 日志主题的 ID
        /// </summary>
        public string topic_id { get; set; }
        /// <summary>
        /// 本批投递的日志的开始时间
        /// </summary>
        public string range_start { get; set; }
        /// <summary>
        /// 本批投递的日志的结束时间
        /// </summary>
        public string range_end { get; set; }
        /// <summary>
        /// 本次投递任务的开始时间
        /// </summary>
        public string start_time { get; set; }
        /// <summary>
        ///本次投递任务的结束时间
        /// </summary>
        public string end_time { get; set; }
        /// <summary>
        /// 	本次投递的结果，"success","running","failed","wait"
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 结果的详细信息
        /// </summary>
        public string message { get; set; }
    }

    public class PostTasks
    {
        public IEnumerable<PostTask> tasks { get; set; }
    }

    public class FormatCfg
    {
        public string format { get; set; }
    }


    public class FilterRule
    {
        /// <summary>
        /// 用来比较的 key，__CONTENT__代表全文
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 比较内容的提取正则表达式
        /// </summary>
        public string regex { get; set; }
        /// <summary>
        /// 与上面 regex 提取出的内容比较的 value，如果一致则命中
        /// </summary>
        public string value { get; set; }
    }
}