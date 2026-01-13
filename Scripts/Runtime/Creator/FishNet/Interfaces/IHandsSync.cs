using System;
using UnityEngine;

namespace Yomov.Network
{
    /// <summary>
    /// 手部同步接口
    /// 抽象手部网络同步的核心功能
    /// </summary>
    public interface IHandsSync
    {
        // Unity 组件访问
        GameObject gameObject { get; }
        Transform transform { get; }
        
        // 网络状态
        bool IsOwner { get; }
        bool IsServer { get; }
        INetworkObject NetworkObject { get; }
        
        // 业务方法
        void ServerSetMultiHandsSyncData(VRAvatar.MultiHandsSyncData data);
        VRAvatar.MultiHandsSyncData GetMultiHandsSyncData();
        
        // 事件
        event Action OnStartClientEvent;
        event Action<VRAvatar.MultiHandsSyncData, VRAvatar.MultiHandsSyncData, bool> OnHandsSyncDataChangedEvent;
        
        // 委托属性
        Func<bool> ShouldSyncHandsData { get; set; }
        Func<VRAvatar.MultiHandsSyncData> GetHandsSyncDataFunc { get; set; }
    }
}

