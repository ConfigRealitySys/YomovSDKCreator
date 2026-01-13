using UnityEngine;

namespace Yomov.Network
{
    /// <summary>
    /// 网络对象接口
    /// 抽象网络对象的核心功能，解耦具体网络框架
    /// </summary>
    public interface INetworkObject
    {
        /// <summary>
        /// 网络对象ID
        /// </summary>
        int ObjectId { get; }
        
        /// <summary>
        /// 对象所有者的客户端ID
        /// </summary>
        int OwnerId { get; }
        
        /// <summary>
        /// 对象是否已在网络中生成
        /// </summary>
        bool IsSpawned { get; }
        
        /// <summary>
        /// 关联的 GameObject
        /// </summary>
        GameObject gameObject { get; }
        
        /// <summary>
        /// 关联的 Transform
        /// </summary>
        Transform transform { get; }
    }
}

