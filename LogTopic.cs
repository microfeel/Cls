using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroFeel.CLS
{
    public class LogTopics
    {
        public IEnumerable<LogTopic> topics;
    }

    public class LogTopic
    {
        /// <summary>
        ///    日志主题的 ID
        /// </summary>
        public string logset_id { get; set; }
        /// <summary>
        /// 日志主题的 ID
        /// </summary>
        public string topic_id;
        /// <summary>
        /// 日志主题的名字
        /// </summary>
        public string topic_name;
        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string path;
        /// <summary>
        /// 机器组id
        /// </summary>
        public string group_id { get; set; }
        /// <summary>
        /// 是否开启采集
        /// </summary>
        public bool collection;
        /// <summary>
        /// 是否开启索引
        /// </summary>
        public bool index;
        /// <summary>
        /// 采集的日志类型，json_log代表 json 格式日志，delimiter_log代表分隔符格式日志，minimalist_log代表极简日志
        /// </summary>
        public string log_type;
        /// <summary>
        /// 提取规则
        /// </summary>
        public ExtractRule extract_rule;
        /// <summary>
        ///  采集机器组信息
        /// </summary>
        public HostGroup machine_group;
        /// <summary>
        /// 创建时间
        /// </summary>
        public string create_time;

        public LogTopic(string logsetid, string topicname)
        {
            logset_id = logsetid;
            topic_name = topicname;
        }

        /// <summary>
        /// 创建日志主题
        /// </summary>
        /// <param name="logTopic">日志主题对象</param>
        /// <returns>日志主题id</returns>
        public static async Task<string> CreateAsync(LogTopic logTopic)
        {
            return (await ClsClient.Client.CreateAsync("topic", logTopic)).topic_id;
        }

        /// <summary>
        /// 删除日志主题
        /// </summary>
        /// <param name="id">日志主题id</param>
        /// <returns>Null</returns>
        public static async Task DeleteAsync(string id)
        {
            await ClsClient.Client.DeleteAsync($"topic?topic_id={id}");
        }

        /// <summary>
        /// 获取日志主题列表
        /// </summary>
        /// <returns>日志主题列表</returns>
        public static async Task<IEnumerable<LogTopic>> GetListAsync(string logsetid)
        {
            return (await ClsClient.Client.GetAsync<LogTopics>($"topics?logset_id={logsetid}")).topics;
        }

        /// <summary>
        /// 获取日志主题信息
        /// </summary>
        /// <param name="topic_id">日志主题id</param>
        /// <returns>日志主题实例</returns>
        public static async Task<LogTopic> GetAsync(string topic_id)
        {
            return await ClsClient.Client.GetAsync<LogTopic>($"topic?topic_id={topic_id}");
        }

        /// <summary>
        /// 更新日志主题
        /// </summary>
        /// <param name="logtopic">日志主题对象</param>
        /// <returns>Null</returns>
        public static async Task UpdateAsync(LogTopic logTopic)
        {
            await ClsClient.Client.UpdateAsync("topic", logTopic);
        }

    }

    /// <summary>
    /// 提取规则
    /// </summary>
    public class ExtractRule
    {
        /// <summary>
        /// 时间字段的 key 名字，time_key 和 time_format 必须成对出现
        /// </summary>
        public string time_key { get; set; }
        /// <summary>
        /// 时间字段的格式，参考 C 语言的strftime函数对于时间的格式说明
        /// </summary>
        public string time_format { get; set; }
        /// <summary>
        /// 分隔符类型日志的分隔符，只有log_type为delimiter_log时有效
        /// </summary>
        public string delimiter { get; set; }
        /// <summary>
        /// 提取的每个字段的 key 名字，为空的 key 代表丢弃这个字段，只有log_type为delimiter_log时有效，json_log的日志使用 json 本身的 key
        /// </summary>
        public IEnumerable<string> keys { get; set; }
        /// <summary>
        /// 需要过滤日志的 key，最多 5 个
        /// </summary>
        public IEnumerable<string> filter_keys { get; set; }
        /// <summary>
        /// 上面字段 key 对应的值，个数与 filter_keys 相同，一一对应，匹配的日志进行采集
        /// </summary>
        public IEnumerable<string> filter_regex { get; set; }
    }
}
