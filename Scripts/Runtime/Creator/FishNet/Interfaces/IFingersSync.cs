using System;
using System.Threading;
using UnityEngine;
using static Yomov.HandTrackingSync;

namespace Yomov.Network
{
    /// <summary>
    /// 手指同步接口
    /// 抽象手指网络同步的核心功能
    /// </summary>
    public interface IFingersSync
    {
        // Unity 组件访问
        GameObject gameObject { get; }
        Transform transform { get; }
        
        // 网络状态
        bool IsOwner { get; }
        bool IsServer { get; }
        INetworkObject NetworkObject { get; }
        
        // 数据访问
        FingersData GetSyncLeftHand();
        FingersData GetSyncRightHand();
        void SetSyncLeftHand(FingersData data);
        void SetSyncRightHand(FingersData data);
        
        // 事件
        event Action OnStartClientEvent;
        event Action OnDestroyEvent;
        event Action<FingersData, FingersData, bool> OnLeftHandDataChangedEvent;
        event Action<FingersData, FingersData, bool> OnRightHandDataChangedEvent;
        
        // 取消令牌
        CancellationTokenSource[] destroyCancellationTokens { get; set; }
    }
}

