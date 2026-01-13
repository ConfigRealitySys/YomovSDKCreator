#if FISHNET_3_10_8
using FishNet.Object;
#endif
using UnityEngine;

namespace Yomov.Network
{
    /// <summary>
    /// FishNet 网络对象适配器
    /// 将 FishNet 的 NetworkObject 包装为 INetworkObject 接口
    /// </summary>
    public class FishNetNetworkObject : INetworkObject
    {
#if FISHNET_3_10_8
        private readonly NetworkObject _networkObject;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="networkObject">FishNet 的 NetworkObject</param>
        public FishNetNetworkObject(NetworkObject networkObject)
        {
            _networkObject = networkObject;
        }
        
        /// <summary>
        /// 获取底层的 FishNet NetworkObject（用于适配器内部）
        /// </summary>
        public NetworkObject UnderlyingNetworkObject => _networkObject;
        
        public int ObjectId => _networkObject.ObjectId;
        
        public int OwnerId => _networkObject.OwnerId;
        
        public bool IsSpawned => _networkObject.IsSpawned;
        
        public GameObject gameObject => _networkObject.gameObject;
        
        public Transform transform => _networkObject.transform;
#else
        public FishNetNetworkObject(object networkObject)
        {
            // 无 FishNet 时不执行
        }
        
        public object UnderlyingNetworkObject => null;
        
        public int ObjectId => -1;
        
        public int OwnerId => -1;
        
        public bool IsSpawned => false;
        
        public GameObject gameObject => null;
        
        public Transform transform => null;
#endif
    }
}
