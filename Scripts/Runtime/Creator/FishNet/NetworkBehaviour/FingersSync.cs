using System;
using System.Threading;
#if FISHNET_3_10_8
using FishNet.Object;
using FishNet.Object.Synchronizing;
#endif
using UnityEngine;
using Yomov.Network;
using static Yomov.HandTrackingSync;

namespace Yomov
{
    /// <summary>
    /// 手指网络同步 - 只负示网络数据传输
    /// </summary>
    public class FingersSync : TeamNetBehaviour, IFingersSync
    {
#if FISHNET_3_10_8
        [SyncVar(SendRate = FishNetConfig.AvatarSyncTime, OnChange = nameof(OnLeftHandDataChange))]
        private FingersData _syncLeftHand;
        
        [SyncVar(SendRate = FishNetConfig.AvatarSyncTime, OnChange = nameof(OnRightHandDataChange))]
        private FingersData _syncRightHand;
#else
        private FingersData _syncLeftHand;
        private FingersData _syncRightHand;
#endif
        
        #region 跨程序集访问方法 (Get/Set 方法)
        
        /// <summary>
        /// 获取左手手指数据（跨程序集安全访问）
        /// </summary>
        public FingersData GetSyncLeftHand() => _syncLeftHand;
        
        /// <summary>
        /// 获取右手手指数据（跨程序集安全访问）
        /// </summary>
        public FingersData GetSyncRightHand() => _syncRightHand;
        
        /// <summary>
        /// 获取网络对象（IFingersSync 接口实现）
        /// </summary>
        INetworkObject IFingersSync.NetworkObject
        {
            get
            {
#if FISHNET_3_10_8
                return new FishNetNetworkObject(base.NetworkObject);
#else
                return null;
#endif
            }
        }
        
        /// <summary>
        /// 设置左手手指数据（客户端调用，自动通过 ServerRpc 同步）
        /// </summary>
        public void SetSyncLeftHand(FingersData data)
        {
#if FISHNET_3_10_8
            SyncLeftHandData(data);
#else
            _syncLeftHand = data;
#endif
        }
        
        /// <summary>
        /// 设置右手手指数据（客户端调用，自动通过 ServerRpc 同步）
        /// </summary>
        public void SetSyncRightHand(FingersData data)
        {
#if FISHNET_3_10_8
            SyncRightHandData(data);
#else
            _syncRightHand = data;
#endif
        }
        
        #endregion
        
        // ===== 暴露给业务层的事件 =====
        public new event Action OnStartClientEvent;  // 覆盖基类的事件
        public event Action OnDestroyEvent;
        public event Action<FingersData, FingersData, bool> OnLeftHandDataChangedEvent;
        public event Action<FingersData, FingersData, bool> OnRightHandDataChangedEvent;
        
        // 为了兼容低于2022的Unity版本,而使用自定义的Cancel
        public CancellationTokenSource cancellationTokenSource;
        CancellationTokenSource[] IFingersSync.destroyCancellationTokens
        {
            get => new[] { cancellationTokenSource };
            set { if (value != null && value.Length > 0) cancellationTokenSource = value[0]; }
        }
        
        private void Awake()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }
        
        public
#if FISHNET_3_10_8
        override
#else
        new
#endif
        void OnStartClient()
        {
            base.OnStartClient();
            OnStartClientEvent?.Invoke();
        }
        
        public void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
            OnDestroyEvent?.Invoke();
        }
        
#if FISHNET_3_10_8
        [ServerRpc]
        private void SyncLeftHandData(FingersData data)
        {
            _syncLeftHand = data;
        }
        
        [ServerRpc]
        private void SyncRightHandData(FingersData data)
        {
            _syncRightHand = data;
        }
        
        private void OnLeftHandDataChange(FingersData prev, FingersData next, bool asServer)
        {
            OnLeftHandDataChangedEvent?.Invoke(prev, next, asServer);
        }
        
        private void OnRightHandDataChange(FingersData prev, FingersData next, bool asServer)
        {
            OnRightHandDataChangedEvent?.Invoke(prev, next, asServer);
        }
#endif
    }
}
