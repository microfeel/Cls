using Microsoft.Extensions.Logging;

namespace MicroFeel.CLS
{
    /// <summary>
    /// 日志服务设置
    /// </summary>
    public class ClsSetting
    {
        /// <summary>
        /// 腾讯云secretId
        /// </summary>
        public string SecretId { get; set; }
        /// <summary>
        /// 腾讯云secretKey
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 日志服务地址
        /// </summary>
        public string Endpoint { get; set; }
        /// <summary>
        /// 启用标识
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 日志集名称
        /// </summary>
        public string LogSetName { get; set; }
        /// <summary>
        /// 日志主题名称
        /// </summary>
        public string LogTopicName { get; set; }
        /// <summary>
        /// 保存天数
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel LogLevel { get; set; }
    }
}
