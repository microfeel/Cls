using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroFeel.CLS
{
    public class LogSets
    {
        public LogSet[] logsets { get; set; }
    }

    public class LogSet
    {
        /// <summary>
        /// 日志集ID
        /// </summary>
        public string logset_id { get; set; }

        /// <summary>
        /// 日志集的名字
        /// </summary>
        public string logset_name { get; set; }

        /// <summary>
        /// 保存周期（天）
        /// </summary>
        public int period { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string create_time { get; set; }

        /// <summary>
        /// 实例化LogSet对象
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="period">保存天数</param>
        public LogSet(string name,int period)
        {
            logset_name = name;
            this.period = period;
        }

        /// <summary>
        /// 创建日志集
        /// </summary>
        /// <param name="logset">日志集对象</param>
        /// <returns>日志集id</returns>
        public static async Task<string> CreateAsync(LogSet logset)
        {
            return (await ClsClient.Client.CreateAsync("logset", logset)).logset_id;
        }

        /// <summary>
        /// 删除日志集
        /// </summary>
        /// <param name="id">日志集id</param>
        /// <returns>Null</returns>
        public static async Task DeleteAsync(string id)
        {
            await ClsClient.Client.DeleteAsync($"logset?logset_id={id}");
        }

        /// <summary>
        /// 获取日志集列表
        /// </summary>
        /// <returns>日志集列表</returns>
        public static async Task<IEnumerable<LogSet>> GetListAsync()
        {
            return (await ClsClient.Client.GetAsync<LogSets>($"logsets")).logsets;
        }

        /// <summary>
        /// 获取日志集信息
        /// </summary>
        /// <param name="logset_id">日志集id</param>
        /// <returns>日志集实例</returns>
        public static async Task<LogSet> GetAsync(string logset_id)
        {
            return await ClsClient.Client.GetAsync<LogSet>($"logset?logset_id={logset_id}");
        }

        /// <summary>
        /// 更新日志集
        /// </summary>
        /// <param name="logset">日志集对象</param>
        /// <returns>Null</returns>
        public static async Task UpdateAsync(LogSet logset)
        {
            await ClsClient.Client.UpdateAsync("logset", logset);
        }

    }
}
