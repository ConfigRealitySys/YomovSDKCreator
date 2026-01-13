#if FISHNET_3_10_8
using FishNet;
#endif
using UnityEngine;

namespace Yomov.Network.FishNet
{
    /// <summary>
    /// FishNet 网络管理器适配器
    /// 实现 INetworkManager 接口，提供对 FishNet 网络服务的统一访问
    /// </summary>
    public class FishNetNetworkManager : INetworkManager
    {
        private readonly FishNetServerManager _serverManager;
        private readonly FishNetClientManager _clientManager;
        private readonly FishNetTransportManager _transportManager;
        private readonly FishNetSceneManager _sceneManager;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public FishNetNetworkManager()
        {
            Debug.Log("[FishNetNetworkManager] 构造函数执行");
            _serverManager = new FishNetServerManager();
            _clientManager = new FishNetClientManager();
            _transportManager = new FishNetTransportManager();
            _sceneManager = new FishNetSceneManager();
            
            Debug.Log("[FishNetNetworkManager] 所有管理器已创建（事件将延迟初始化）");
        }
        
        /// <summary>
        /// 服务器管理器
        /// </summary>
        public IServerManager ServerManager => _serverManager;
        
        /// <summary>
        /// 客户端管理器
        /// </summary>
        public IClientManager ClientManager => _clientManager;
        
        /// <summary>
        /// 传输管理器
        /// </summary>
        public ITransportManager TransportManager => _transportManager;
        
        /// <summary>
        /// 场景管理器
        /// </summary>
        public ISceneManager SceneManager => _sceneManager;
        
        /// <summary>
        /// 是否为服务器
        /// </summary>
        public bool IsServer
        {
            get
            {
#if FISHNET_3_10_8
                return InstanceFinder.IsServer;
#else
                return false;
#endif
            }
        }
        
        /// <summary>
        /// 是否为客户端
        /// </summary>
        public bool IsClient
        {
            get
            {
#if FISHNET_3_10_8
                return InstanceFinder.IsClient;
#else
                return false;
#endif
            }
        }
        
        /// <summary>
        /// 是否为主机（同时是服务器和客户端）
        /// </summary>
        public bool IsHost => IsServer && IsClient;
        
        /// <summary>
        /// 从对象池获取实例化的网络对象
        /// </summary>
        public INetworkObject GetPooledInstantiated(INetworkObject prefab, bool makeActive)
        {
#if FISHNET_3_10_8
            if (InstanceFinder.NetworkManager != null)
            {
                var fishNetPrefab = (prefab as FishNetNetworkObject)?.UnderlyingNetworkObject;
                if (fishNetPrefab != null)
                {
                    var instance = InstanceFinder.NetworkManager.GetPooledInstantiated(fishNetPrefab, makeActive);
                    return new FishNetNetworkObject(instance);
                }
            }
#endif
            return null;
        }
    }
}
