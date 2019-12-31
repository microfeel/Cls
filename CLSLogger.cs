using Cls;
using MicroFeel.CLS;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace MicroFeel.CLS
{
    public class CLSLogger : ILogger
    {
        private readonly string ip = "Unknown";
        private readonly string _name;
        private readonly ClsSetting _settings;
        private readonly IExternalScopeProvider _externalScopeProvider;

        /// <summary>
        /// 保存了日志集id
        /// </summary>
        private static string logsetid = "";
        /// <summary>
        /// 保存的日志主题id
        /// </summary>
        private static string logTopicid = "";

        internal IExternalScopeProvider ScopeProvider { get; set; }
        /// <summary>
        /// 创建日志记录器实例
        /// </summary>
        /// <param name="factory">logger工厂</param>
        /// <param name="endpoint">日志服务地址</param>
        /// <param name="secretId">Appid</param>
        /// <param name="secretKey">Appkey</param>
        /// <param name="logSetName">日志集名称</param>
        /// <param name="logTopicName">日志主题名称</param>
        /// <param name="period">保存天数</param>
        public CLSLogger(string name, ClsSetting setting, IExternalScopeProvider externalScopeProvider)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _settings = setting ?? throw new ArgumentNullException(nameof(setting));
            _externalScopeProvider = externalScopeProvider;
            var ipaddress = NetworkInterface.GetAllNetworkInterfaces().SelectMany(i => i.GetIPProperties().UnicastAddresses).FirstOrDefault(v => v.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Address;
            if (ipaddress != null)
            {
                ip = ipaddress.ToString();
            }
        }

        /// <summary>
        /// 获取日志集和主题id,保存备用
        /// </summary>
        /// <param name="logsetName"></param>
        /// <returns></returns>
        private async Task SetLogsetId(string logsetName)
        {
            var logsets = await LogSet.GetListAsync();
            var logset = logsets.FirstOrDefault(ls => ls.logset_name == logsetName);

            if (logset == null)
            {
                logsetid = await LogSet.CreateAsync(new LogSet(logsetName, _settings.Period));
                logset = LogSet.GetAsync(logsetid).Result;
            }
            else
            {
                logsetid = logset.logset_id;
            }
            //查找日志主题
            var logTopics = await LogTopic.GetListAsync(logsetid);
            var logtopic = logTopics.FirstOrDefault(lt => lt.topic_name == _settings.LogTopicName);

            if (logtopic == null)
            {
                logTopicid = await LogTopic.CreateAsync(new LogTopic(logsetid, _settings.LogTopicName));
            }
            else
            {
                logTopicid = logtopic.topic_id;
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _externalScopeProvider?.Push(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _settings.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            PostLogAsync(logLevel, eventId, state, exception, formatter).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    //TODO 需要先存储到文件,没有错误后再发布到云
                    throw t.Exception;
                }
            });
        }

        private async Task PostLogAsync<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
            {
                return;
            }
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            //构造日志数据
            var logGroupList = new LogGroupList
            {
                LogGroupList_ = { new LogGroup
                {
                    Source =ip,
                    ContextFlow="microfeel", //保持上下文的 UID，该字段目前暂无效用
                    Filename = $"{_name}-{timestamp}",
                    Logs={ new Log
                    {
                        Contents={ new Log.Types.Content { Key= logLevel.ToString(), Value=formatter(state,exception)} },
                        Time =  timestamp
                    } }
                }}
            };
            if (logsetid == "" || logTopicid == "")
            {
                await SetLogsetId(_settings.LogSetName);
            }

            await LogObject.UploadAsync(logTopicid, logGroupList);
        }
    }
}
