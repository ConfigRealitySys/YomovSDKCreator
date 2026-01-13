using System.Collections.Generic;

namespace Yomov.Network
{
    /// <summary>
    /// 网络连接接口
    /// 表示一个客户端连接
    /// </summary>
    public interface INetworkConnection
    {
        /// <summary>
        /// 客户端 ID
        /// </summary>
        int ClientId { get; }
        
        /// <summary>
        /// 获取连接的地址（IP:Port）
        /// </summary>
        /// <returns>地址字符串</returns>
        string GetAddress();
        
        /// <summary>
        /// 连接是否处于激活状态
        /// </summary>
        bool IsActive { get; }
        
        /// <summary>
        /// 该连接拥有的所有网络对象（只读集合）
        /// </summary>
        IReadOnlyCollection<INetworkObject> Objects { get; }
        
        /// <summary>
        /// 第一个网络对象（通常是玩家对象）
        /// 用于快速访问玩家的主要网络对象
        /// </summary>
        INetworkObject FirstObject { get; }
        
        /// <summary>
        /// 踢出该连接的客户端
        /// </summary>
        /// <param name="reason">踢出原因</param>
        void Kick(object reason);
    }
}

