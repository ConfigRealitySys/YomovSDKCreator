using System;
#if FISHNET_3_10_8
using FishNet.Object;
using FishNet.Object.Synchronizing;
#endif
using UnityEngine;
using Yomov.Network;

namespace Yomov
{
    /// <summary>
    /// 手部网络同步 - 只负责网络数据传输
    /// </summary>
    public class HandsSync : TeamNetBehaviour, IHandsSync
    {
#if FISHNET_3_10_8
        [field: SyncVar(OnChange = nameof(OnMultiHandsSyncData))]
        private VRAvatar.MultiHandsSyncData multiHandsSyncData;
#else
        private VRAvatar.MultiHandsSyncData multiHandsSyncData;
#endif
        
        private VRAvatar.MultiHandsSyncData _HandsData = new();
        private float _UpdateTime = 0;
        
        #region 跨程序集访问方法 (Get 方法)
        
        /// <summary>
        /// 获取手部同步数据（跨程序集安全访问）
        /// </summary>
        public VRAvatar.MultiHandsSyncData GetMultiHandsSyncData() => multiHandsSyncData;
        
        /// <summary>
        /// 获取网络对象（IHandsSync 接口实现）
        /// </summary>
        INetworkObject IHandsSync.NetworkObject
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
        
        #endregion
        
        // ===== 暴露给业务层的事件 =====
        public new event Action OnStartClientEvent;  // 覆盖基类的事件
        public event Action<VRAvatar.MultiHandsSyncData, VRAvatar.MultiHandsSyncData, bool> OnHandsSyncDataChangedEvent;
        
        // ===== 业务逻辑注入点 =====
        public Func<bool> ShouldSyncHandsData { get; set; }
        public Func<VRAvatar.MultiHandsSyncData> GetHandsSyncDataFunc { get; set; }  // 重命名避免与 Get 方法冲突
        
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
        
        private void Update()
        {
#if FISHNET_3_10_8
            if (IsOwner && ShouldSyncHandsData?.Invoke() == true)
            {
                _UpdateTime += Time.deltaTime;
                if (FishNetConfig.AvatarSyncTime < _UpdateTime)
                {
                    _UpdateTime = 0;
                    if (GetHandsSyncDataFunc != null)
                    {
                        _HandsData = GetHandsSyncDataFunc.Invoke();
                        SyncData(_HandsData);
                    }
                }
            }
#endif
        }
        
#if FISHNET_3_10_8
        [ServerRpc]
        private void SyncData(VRAvatar.MultiHandsSyncData handData)
        {
            multiHandsSyncData = handData;
        }
#endif
        
        /// <summary>
        /// 服务器端直接设置手部同步数据（仅在服务器端调用）
        /// </summary>
        public void ServerSetMultiHandsSyncData(VRAvatar.MultiHandsSyncData handData)
        {
#if FISHNET_3_10_8
            if (!IsServer)
            {
                // Debug.LogWarning("ServerSetMultiHandsSyncData can only be called on the server!");
                return;
            }
            multiHandsSyncData = handData;
#endif
        }
        
#if FISHNET_3_10_8
        private void OnMultiHandsSyncData(VRAvatar.MultiHandsSyncData prev, 
            VRAvatar.MultiHandsSyncData next, bool asServer)
        {
            OnHandsSyncDataChangedEvent?.Invoke(prev, next, asServer);
        }
#endif
    }
}
