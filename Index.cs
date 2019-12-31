using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroFeel.CLS
{
    public class Index
    {
        /// <summary>
        /// 索引规则属于的 topic id
        /// </summary>
        public string topic_id { get; set; }
        /// <summary>
        /// 是否生效
        /// </summary>
        public bool effective { get; set; }
        /// <summary>
        /// 索引规则，当 effective 为 true 时返回
        /// </summary>
        public IndexRule rule { get; set; }


        #region 索引管理
        /// <summary>
        /// 获取索引信息
        /// </summary>
        /// <param name="topicid">主题ID</param>
        /// <returns></returns>
        public async Task<Index> GetIndexAsync(string topicid)
        {
            return await ClsClient.Client.GetAsync<Index>($"index?topic_id={topicid}");
        }
        /// <summary>
        /// 修改索引任务
        /// </summary>
        /// <param name="topic_id"></param>
        /// <param name="effective"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public async Task UpdateIndexAsync(Index index)
        {
            await ClsClient.Client.UpdateAsync("index", index);
        }

        #endregion

    }

    public class IndexRule
    {
        public FullTextCFG full_text { get; set; }
        public IndexCfg key_value { get; set; }
    }

    /// <summary>
    /// kv 索引的相关配置
    /// </summary>
    public class IndexCfg
    {
        /// <summary>
        /// 	是否大小写敏感
        /// </summary>
        public bool case_sensitive { get; set; }
        /// <summary>
        /// 需要建索引的 key 的名字
        /// </summary>
        public IEnumerable<string> keys { get; set; }
        /// <summary>
        /// 上面 key 对应的类型，一一对应，目前支持 long double text
        /// </summary>
        public IEnumerable<string> types { get; set; }
        /// <summary>
        /// 上面 key 对应的分词符，一一对应，只对 text 类型设置，其他类型为空字符串
        /// </summary>
        public IEnumerable<string> tokenizer { get; set; }
    }

    /// <summary>
    /// 	全文索引的相关配置
    /// </summary>
    public class FullTextCFG
    {
        /// <summary>
        /// 	是否大小写敏感
        /// </summary>
        public bool case_sensitive { get; set; }
        /// <summary>
        /// 全文索引的分词符
        /// </summary>
        public string tokenizer { get; set; }
    }

}
