using System;
#if FISHNET_3_10_8
using FishNet.Connection;
using FishNet.Object;
#endif
using UnityEngine;

namespace Yomov
{
    /// <summary>
    /// 队伍网络行为基类 - 只负责网络同步
    /// </summary>
#if FISHNET_3_10_8
    public class TeamNetBehaviour : NetworkBehaviour
#else
    public class TeamNetBehaviour : MonoBehaviour
#endif
    {
        public float sameTeamDistance;
        public float otherTeamDistance;
        
        // ===== 暴露给业务层的事件 =====
        public event Action OnStartClientEvent;
#if FISHNET_3_10_8
        public event Action<NetworkConnection> OnSpawnServerEvent;
        public event Action<NetworkConnection> OnDespawnServerEvent;
#else
        public event Action<object> OnSpawnServerEvent;
        public event Action<object> OnDespawnServerEvent;
#endif
        
        // ===== 业务逻辑注入点 =====
        public Func<long> GetTeamIdFunc;  // 获取队伍ID（重命名避免与子类方法冲突）
        
#if FISHNET_3_10_8
        // FishNet 版本的网络属性 - 继承自 NetworkBehaviour
#else
        // 模拟网络属性（无 FishNet 版本）
        public bool IsOwner => false;
        public bool IsServer => false;
        public bool IsClient => false;
        public object NetworkObject => null;
#endif
        
        public
#if FISHNET_3_10_8
        override
#endif
        void OnStartClient()
        {
#if FISHNET_3_10_8
            base.OnStartClient();
            
            if (IsOwner)
            {
                var teamId = GetTeamIdFunc?.Invoke() ?? 0;
                if (teamId != 0)
                {
                    SetTeam(teamId);
                }
            }
#endif
            
            OnStartClientEvent?.Invoke();
        }
        
#if FISHNET_3_10_8
        public override void OnSpawnServer(NetworkConnection connection)
        {
            base.OnSpawnServer(connection);
            TeamCondition.AddObjectDistance(NetworkObject, sameTeamDistance, otherTeamDistance);
            OnSpawnServerEvent?.Invoke(connection);
        }
        
        public override void OnDespawnServer(NetworkConnection connection)
        {
            base.OnDespawnServer(connection);
            TeamCondition.RemoveFromTeam(NetworkObject);
            TeamCondition.RemoveObjectDistance(NetworkObject);
            OnDespawnServerEvent?.Invoke(connection);
        }
        
        [ServerRpc]
        public void SetTeam(long teamId)
        {
            TeamCondition.AddToTeam(NetworkObject, teamId);
        }
#else
        public virtual void OnSpawnServer(object connection)
        {
            OnSpawnServerEvent?.Invoke(connection);
        }
        
        public virtual void OnDespawnServer(object connection)
        {
            OnDespawnServerEvent?.Invoke(connection);
        }
        
        public void SetTeam(long teamId)
        {
            // 无 FishNet 时不执行
        }
#endif
    }
}
