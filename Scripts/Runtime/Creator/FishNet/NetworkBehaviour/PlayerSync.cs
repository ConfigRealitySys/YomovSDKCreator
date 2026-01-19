using System;
#if FISHNET_3_10_8
using FishNet.Object;
using FishNet.Object.Synchronizing;
#endif
using UnityEngine;
using Yomov.Network;

namespace Yomov
{
    public enum PlayerType
    {
        Mine,
        Team,
        Other,
    }

    /// <summary>
    /// 玩家网络同步 - 只负责网络数据传输
    /// </summary>
    public class PlayerSync : TeamNetBehaviour, IPlayerSync
    {
    #region 网络同步字段 (SyncVar)
    
#if FISHNET_3_10_8
    [SyncVar(OnChange = nameof(OnTeamIdChange))]
    private long teamId;
    
    [SyncVar]
    private string avatarResourceName;
    
    [SyncVar(OnChange = nameof(OnAvatarSkinResourceNameChange))]
    private string avatarSkinResourceName;
    
    [SyncVar]
    private long playerOriginId;
    
    [SyncVar(OnChange = nameof(OnPlayerNickChange))]
    private string nickname;
    
    [SyncVar]
    private long contentId;
    
    [field: SyncVar(OnChange = nameof(OnMultiAvatarSyncData))]
    private VRAvatar.MultiBodySyncData multiBodySyncData;
#else
    private long teamId;
    private string avatarResourceName;
    private string avatarSkinResourceName;
    private long playerOriginId;
    private string nickname;
    private long contentId;
    private VRAvatar.MultiBodySyncData multiBodySyncData;
#endif
    
    #endregion
    
    #region 跨程序集访问方法 (Get 方法)
    
    /// <summary>
    /// 获取队伍 ID（跨程序集安全访问）
    /// </summary>
    public long GetTeamId() => teamId;
    
    /// <summary>
    /// 获取角色资源名称（跨程序集安全访问）
    /// </summary>
    public string GetAvatarResourceName() => avatarResourceName;
    
    /// <summary>
    /// 获取角色皮肤资源名称（跨程序集安全访问）
    /// </summary>
    public string GetAvatarSkinResourceName() => avatarSkinResourceName;
    
    /// <summary>
    /// 获取玩家原始 ID（跨程序集安全访问）
    /// </summary>
    public long GetPlayerOriginId() => playerOriginId;
    
    /// <summary>
    /// 获取玩家昵称（跨程序集安全访问）
    /// </summary>
    public string GetNickname() => nickname;
    
    /// <summary>
    /// 获取玩家昵称（IPlayerSync 接口实现）
    /// </summary>
    public string GetPlayerNick() => nickname;
    
    /// <summary>
    /// 获取头像面部资源名称（IPlayerSync 接口实现）
    /// </summary>
    public string GetAvatarFaceResourceName() => avatarResourceName;
    
    /// <summary>
    /// 获取内容 ID（跨程序集安全访问）
    /// </summary>
    public long GetContentId() => contentId;
    
    /// <summary>
    /// 获取多人身体同步数据（跨程序集安全访问）
    /// </summary>
    public VRAvatar.MultiBodySyncData GetMultiBodySyncData() => multiBodySyncData;
    
    /// <summary>
    /// 获取网络对象（IPlayerSync 接口实现）
    /// </summary>
    INetworkObject IPlayerSync.NetworkObject
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
        
    #region 暴露给业务层的事件
    
    // ===== 生命周期事件 =====
    public new event Action OnStartClientEvent;  // 覆盖基类的事件
    public event Action OnDestroyEvent;
    public event Action OnOwnerUpdateEvent;  // IsOwner 的 Update
        
        // ===== 数据变化事件 =====
        public event Action<long, long, bool> OnTeamIdChangedEvent;  // (prev, next, asServer)
        public event Action<string, string, bool> OnPlayerNickChangedEvent;  // (prev, next, asServer)
        public event Action<string, string, bool> OnAvatarSkinChangedEvent;  // (prev, next, asServer)
        public event Action<VRAvatar.MultiBodySyncData, VRAvatar.MultiBodySyncData, bool> OnBodySyncDataChangedEvent;  // (prev, next, asServer)
        
        // ===== 条件查询委托 =====
        public Func<bool> ShouldCreateAvatar { get; set; }  // 是否应该创建 Avatar
        public Func<bool> ShouldSyncBodyData { get; set; }  // 是否应该同步身体数据
        
        // ===== 数据获取委托 =====
        public Func<VRAvatar.MultiBodySyncData> GetBodySyncData { get; set; }  // 获取当前身体同步数据
        public Func<Vector3> GetLocalPlayerPosition { get; set; }  // 获取本地玩家位置
        
        #endregion
        
        #region 网络同步逻辑
        
        private VRAvatar.MultiBodySyncData _BodyData = new();
        private float _UpdateTime = 0;
        
        public
#if FISHNET_3_10_8
        override
#else
        new
#endif
        void OnStartClient()
        {
            base.OnStartClient();

            // 触发事件，让业务层处理
            OnStartClientEvent?.Invoke();
        }
        
        private void Update()
        {
            if (IsOwner)
            {
#if FISHNET_3_10_8
                // 身体数据同步
                if (ShouldSyncBodyData?.Invoke() == true)
                {
                    _UpdateTime += Time.deltaTime;
                    if (FishNetConfig.AvatarSyncTime < _UpdateTime)
                    {
                        _UpdateTime = 0;
                        if (GetBodySyncData != null)
                        {
                            _BodyData = GetBodySyncData.Invoke();
                            SyncData(_BodyData);
                        }
                    }
                }
#endif
                
                // 触发业务层的 Update 事件
                OnOwnerUpdateEvent?.Invoke();
            }
        }
        
        private void OnDestroy()
        {
            OnDestroyEvent?.Invoke();
        }
        
        #endregion
        
        #region 网络 RPC 方法
        
#if FISHNET_3_10_8
        [ServerRpc]
        private void SyncData(VRAvatar.MultiBodySyncData bodyData)
        {
            multiBodySyncData = bodyData;
        }
        
        /// <summary>
        /// 客户端请求服务器设置玩家数据（从客户端调用）
        /// </summary>
        [ServerRpc]
        public void SetPlayerData(long teamId, long playerOriginId, long contentId, 
            string nickname, string avatarResourceName, string avatarSkinResourceName)
        {
            this.teamId = teamId;
            this.playerOriginId = playerOriginId;
            this.contentId = contentId;
            this.nickname = nickname;
            this.avatarResourceName = avatarResourceName;
            this.avatarSkinResourceName = avatarSkinResourceName;
            TeamCondition.AddToTeam(this.NetworkObject, teamId);
        }
#else
        public void SetPlayerData(long teamId, long playerOriginId, long contentId, 
            string nickname, string avatarResourceName, string avatarSkinResourceName)
        {
            // 无 FishNet 时不执行
        }
#endif
        
        /// <summary>
        /// 服务器端直接设置玩家数据（仅在服务器端调用）
        /// </summary>
        public void ServerSetPlayerData(long teamId, long playerOriginId, long contentId, 
            string nickname, string avatarResourceName, string avatarSkinResourceName)
        {
#if FISHNET_3_10_8
            if (!IsServer)
            {
                Debug.LogWarning("ServerSetPlayerData can only be called on the server!");
                return;
            }
            
            this.teamId = teamId;
            this.playerOriginId = playerOriginId;
            this.contentId = contentId;
            this.nickname = nickname;
            this.avatarResourceName = avatarResourceName;
            this.avatarSkinResourceName = avatarSkinResourceName;
            TeamCondition.AddToTeam(this.NetworkObject, teamId);
#endif
        }
        
#if FISHNET_3_10_8
        [ServerRpc]
        public void SetAvatarSkinResourceName(string skinName)
        {
            this.avatarSkinResourceName = skinName;
        }
#else
        public void SetAvatarSkinResourceName(string skinName)
        {
            // 无 FishNet 时不执行
        }
#endif
        
        /// <summary>
        /// 服务器端直接设置身体同步数据（仅在服务器端调用）
        /// </summary>
        public void ServerSetMultiBodySyncData(VRAvatar.MultiBodySyncData bodyData)
        {
#if FISHNET_3_10_8
            if (!IsServer)
            {
                // Debug.LogWarning("ServerSetMultiBodySyncData can only be called on the server!");
                return;
            }
            multiBodySyncData = bodyData;
#endif
        }
        
        #endregion
        
        #region SyncVar 变化回调
        
#if FISHNET_3_10_8
        private void OnTeamIdChange(long prev, long next, bool asServer)
        {
            OnTeamIdChangedEvent?.Invoke(prev, next, asServer);
        }
        
        private void OnPlayerNickChange(string prev, string next, bool asServer)
        {
            gameObject.name = next;
            OnPlayerNickChangedEvent?.Invoke(prev, next, asServer);
        }
        
        private void OnAvatarSkinResourceNameChange(string prev, string next, bool asServer)
        {
            if (!IsServer && !string.IsNullOrEmpty(next) && prev != next)
            {
                if (ShouldCreateAvatar?.Invoke() == true)
                {
                    OnAvatarSkinChangedEvent?.Invoke(prev, next, asServer);
                }
            }
        }
        
        private void OnMultiAvatarSyncData(VRAvatar.MultiBodySyncData prev, 
            VRAvatar.MultiBodySyncData next, bool asServer)
        {
            OnBodySyncDataChangedEvent?.Invoke(prev, next, asServer);
        }
#endif
        
        #endregion
    }
}
