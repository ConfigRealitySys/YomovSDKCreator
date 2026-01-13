namespace Yomov.Network
{
    /// <summary>
    /// 网络管理器接口
    /// 提供对服务器管理器和客户端管理器的访问
    /// </summary>
    public interface INetworkManager
    {
        /// <summary>
        /// 服务器管理器
        /// </summary>
        IServerManager ServerManager { get; }
        
        /// <summary>
        /// 客户端管理器
        /// </summary>
        IClientManager ClientManager { get; }
        
        /// <summary>
        /// 传输管理器
        /// </summary>
        ITransportManager TransportManager { get; }
        
        /// <summary>
        /// 场景管理器
        /// </summary>
        ISceneManager SceneManager { get; }
        
        /// <summary>
        /// 是否为服务器
        /// </summary>
        bool IsServer { get; }
        
        /// <summary>
        /// 是否为客户端
        /// </summary>
        bool IsClient { get; }
        
        /// <summary>
        /// 是否为主机（同时是服务器和客户端）
        /// </summary>
        bool IsHost { get; }
        
        /// <summary>
        /// 从对象池获取实例化的网络对象
        /// </summary>
        /// <param name="prefab">网络对象预制体</param>
        /// <param name="makeActive">是否激活对象</param>
        /// <returns>实例化的网络对象</returns>
        INetworkObject GetPooledInstantiated(INetworkObject prefab, bool makeActive);
    }
}

