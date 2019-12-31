using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroFeel.CLS
{
    /// <summary>
    /// 腾讯云机器组对象
    /// </summary>
    public class HostGroup
    {
        /// <summary>
        /// 机器组ID
        /// </summary>
        public string group_id { get; set; }
        /// <summary>
        /// 机器组名称 
        /// </summary>
        public string group_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> ips { get; set; }

        /// <summary>
        /// 机器组创建时间
        /// </summary>
        public string createt_time { get; set; }

        public HostGroup(string name, IEnumerable<string> iPAddresses)
        {
            group_name = name;
            ips = iPAddresses;
        }

        /// <summary>
        /// 创建机器组
        /// </summary>
        /// <param name="hostGroup">机器组对象</param>
        /// <returns>机器组id</returns>
        public static async Task<string> CreateAsync(HostGroup hostGroup)
        {
            return (await ClsClient.Client.CreateAsync("machinegroup", hostGroup)).group_id;
        }

        /// <summary>
        /// 删除机器组
        /// </summary>
        /// <param name="id">机器组id</param>
        /// <returns>Null</returns>
        public static async Task DeleteAsync(string id)
        {
            await ClsClient.Client.DeleteAsync($"machinegroup?group_id={id}");
        }

        /// <summary>
        /// 获取机器组状态
        /// </summary>
        /// <returns>机器组状态</returns>
        public static async Task<IEnumerable<HostGroupStatus>> GetStatusAsync(string groupid)
        {
            return (await ClsClient.Client.GetAsync<HostGroupStatuses>($"machines?group_id={groupid}")).machines;
        }

        /// <summary>
        /// 获取机器组列表
        /// </summary>
        /// <returns>机器组列表</returns>
        public static async Task<IEnumerable<HostGroup>> GetListAsync()
        {
            return (await ClsClient.Client.GetAsync<HostGroups>($"machinegroups")).machine_groups;
        }

        /// <summary>
        /// 获取机器组信息
        /// </summary>
        /// <param name="groupid">机器组id</param>
        /// <returns>机器组实例</returns>
        public static async Task<HostGroup> GetAsync(string groupid)
        {
            return await ClsClient.Client.GetAsync<HostGroup>($"machinegroup?group_id={groupid}");
        }

        /// <summary>
        /// 更新机器组
        /// </summary>
        /// <param name="hostGroup">机器组对象</param>
        /// <returns>Null</returns>
        public static async Task UpdateAsync(HostGroup hostGroup)
        {
            await ClsClient.Client.UpdateAsync("machinegroup", hostGroup);
        }


    }

    /// <summary>
    /// 机器组集合对象
    /// </summary>
    public class HostGroups
    {
        /// <summary>
        /// 机器组集合
        /// </summary>
        public HostGroup[] machine_groups { get; set; }
    }

    /// <summary>
    /// 机器组状态
    /// </summary>
    public class HostGroupStatus
    {
        /// <summary>
        /// 机器的 IP
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 0：异常 1：正常
        /// </summary>
        public int status { get; set; }
    }

    public class HostGroupStatuses
    {
        public IEnumerable<HostGroupStatus> machines { get; set; }
    }

}
