using System;
using UnityEngine;

namespace Yomov.Network
{
    /// <summary>
    /// 玩家同步接口
    /// 抽象玩家网络同步的核心功能，解耦业务逻辑对 FishNet NetworkBehaviour 的依赖
    /// </summary>
    public interface IPlayerSync
    {
        // 基本信息获取
        long GetTeamId();
        long GetContentId();
        long GetPlayerOriginId();
        string GetPlayerNick();
        string GetNickname();
        string GetAvatarSkinResourceName();
        string GetAvatarFaceResourceName();
        
        // 设置方法（客户端/服务器通用）
        void SetPlayerData(long teamId, long playerOriginId, long contentId, string nick, string avatarResourceName, string avatarSkinResourceName);
        void SetAvatarSkinResourceName(string resourceName);
        
        // 服务器专用设置方法
        void ServerSetPlayerData(long teamId, long playerOriginId, long contentId, string nick, string avatarResourceName, string avatarSkinResourceName);
        void ServerSetMultiBodySyncData(VRAvatar.MultiBodySyncData data);
        
        // 数据访问
        VRAvatar.MultiBodySyncData GetMultiBodySyncData();
        
        // 网络状态
        bool IsOwner { get; }
        bool IsServer { get; }
        bool IsClient { get; }
        INetworkObject NetworkObject { get; }
        
        // Unity 组件
        Transform transform { get; }
        GameObject gameObject { get; }
        
        // 事件
        event Action OnStartClientEvent;
        event Action OnDestroyEvent;
        event Action OnOwnerUpdateEvent;
        event Action<long, long, bool> OnTeamIdChangedEvent;  // (prev, next, asServer)
        event Action<string, string, bool> OnPlayerNickChangedEvent;  // (prev, next, asServer)
        event Action<string, string, bool> OnAvatarSkinChangedEvent;  // (prev, next, asServer)
        event Action<VRAvatar.MultiBodySyncData, VRAvatar.MultiBodySyncData, bool> OnBodySyncDataChangedEvent;  // (prev, next, asServer)
        
        // 委托属性
        Func<bool> ShouldCreateAvatar { get; set; }
        Func<bool> ShouldSyncBodyData { get; set; }
        Func<VRAvatar.MultiBodySyncData> GetBodySyncData { get; set; }
        Func<Vector3> GetLocalPlayerPosition { get; set; }
    }
}

