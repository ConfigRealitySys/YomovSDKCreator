using System.Collections.Generic;
#if FISHNET_3_10_8
using System.Runtime.CompilerServices;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Observing;
#endif
using UnityEngine;

namespace Yomov
{
    [CreateAssetMenu(menuName = "FishNet/Observers/Team Condition", fileName = "New Team Condition")]
#if FISHNET_3_10_8
    public class TeamCondition : ObserverCondition
#else
    public class TeamCondition : ScriptableObject
#endif
    {
#if FISHNET_3_10_8
        public class ConditionData
        {
            public Dictionary<long, HashSet<NetworkObject>> MatchObjects = new ();
            public Dictionary<NetworkObject, long> ObjectMatches = new ();
            public Dictionary<NetworkObject, float> SameTeamDistance = new();
            public Dictionary<NetworkObject, float> OtherTeamDistance = new();
        }

        /// <summary>
        /// Collections for each NetworkManager instance.
        /// </summary>
        private static Dictionary<NetworkManager, ConditionData> _collections = new();

        /// <summary>
        /// 如果此条件所在的对象应该对连接可见，则返回true。
        /// </summary>
        /// <param name="connection">正在检查条件的连接。</param>
        /// <param name="currentlyAdded">如果连接当前可以看到此对象，则为真。</param>
        /// <param name="notProcessed">如果条件未被处理，则为真。这可以用于跳过性能处理。当输出为true时，此条件结果假定前一个ConditionMet值</param>
        /// <returns></returns>
        /// 内联（Inlining） 是一种编译器优化技术，它将一个方法的代码直接插入到调用该方法的代码中，而不是生成调用另一个方法的代码。这样做的目的是为了减少方法调用的开销，因为方法调用涉及到一系列的操作，如参数传递、栈管理等，这些操作都会增加程序的执行时间。
        /// MethodImplOptions.AggressiveInlining 是 MethodImplAttribute 的一个枚举值，它告诉编译器尽可能地内联方法，即使这可能会增加代码大小。使用这个属性时，编译器会尝试将该方法内联到任何调用它的地方，如果内联成功，可以提高程序的执行效率，但同时可能会增加编译后的程序大小。
        /// 需要注意的是，即使方法被标记为 AggressiveInlining，最终是否内联还是由编译器决定。编译器会根据多种因素（如方法的大小、复杂性、调用频率等）来决定是否真的进行内联。
        /// 使用 AggressiveInlining 时，通常应用于性能敏感的代码区域，尤其是那些被频繁调用的小型方法。然而，不恰当的使用也可能导致性能下降，因为过大的方法体如果被内联，可能会增加缓存未命中的可能性，从而降低程序的执行速度。
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool ConditionMet(NetworkConnection connection, bool currentlyAdded, out bool notProcessed)
        {
            notProcessed = false;
            /* If object is owned then check if owner
             * and connection share a match. */
            var conditionData = GetCollections();

            if (IsSameTeam(base.NetworkObject, connection.FirstObject))
            {
                if (conditionData.SameTeamDistance.TryGetValue(base.NetworkObject, out var distance))
                {
                    //同队距离大于999，一直可见
                    if (distance >= 999)
                    {
                        return true;
                    }
                    //判断距离是否符合
                    return Vector3.Distance(base.NetworkObject.transform.position,
                               connection.FirstObject.transform.position) <=
                           conditionData.SameTeamDistance[base.NetworkObject];
                }

                //不需要匹配队伍距离。返回true
                return true;
            }
            else
            {
                if (conditionData.OtherTeamDistance.TryGetValue(base.NetworkObject, out var distance))
                {
                    //不同队距离小于0，不可见
                    if (distance <= 0)
                    {
                        return false;
                    }
                }
                else
                {
                    //不需要匹配队伍距离。返回true
                    return true;
                }

                // Debug.Log($"owner:{owner.ClientId}  connection:{connection.ClientId} 不同队显示距离:{distance} 距离：{Vector3.Distance(owner.FirstObject.transform.position, connection.FirstObject.transform.position)}");
                //判断距离是否符合
                if (base.NetworkObject == null)
                {
                    return false;
                }

                if (connection.FirstObject == null)
                {
                    return false;
                }

                return Vector3.Distance(base.NetworkObject.transform.position,
                           connection.FirstObject.transform.position) <=
                       conditionData.OtherTeamDistance[base.NetworkObject];
            }
        }

     

        private static bool IsSameTeam(NetworkObject obj, NetworkObject other,
            NetworkManager manager = null)
        {
            ConditionData cc = GetCollections(manager);
            if (cc.ObjectMatches.TryGetValue(obj, out var team))
            {
                if (cc.MatchObjects.TryGetValue(team, out var o)) return o.Contains(other);
            }

            return false;
        }
        

        public static void AddObjectDistance(NetworkObject obj, float sameTeamDistance, float otherTeamDistance,
            NetworkManager manager = null)
        {
            ConditionData cc = GetCollections(manager);
            cc.SameTeamDistance[obj] = sameTeamDistance;
            cc.OtherTeamDistance[obj] = otherTeamDistance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="manager"></param>
        public static void RemoveObjectDistance(NetworkObject obj, NetworkManager manager = null)
        {
            ConditionData cc = GetCollections(manager);
            cc.SameTeamDistance.Remove(obj);
            cc.OtherTeamDistance.Remove(obj);
        }

        public static void AddToTeam(NetworkObject no, long team, NetworkManager manager = null)
        {
            ConditionData cc = GetCollections(manager);
            if (!cc.MatchObjects.ContainsKey(team))
                cc.MatchObjects[team] = new HashSet<NetworkObject>();
            if (!cc.MatchObjects[team].Contains(no))
            {
                cc.MatchObjects[team].Add(no);
            }

            cc.ObjectMatches[no] = team;
        }

        /// <summary>
        /// 下线时调用
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="team"></param>
        /// <param name="manager"></param>
        public static void RemoveFromTeam(NetworkObject connection, NetworkManager manager = null)
        {
            ConditionData cc = GetCollections(manager);
            if (cc.ObjectMatches.Remove(connection, out var team) && cc.MatchObjects.ContainsKey(team))
            {
                cc.MatchObjects[team].Remove(connection);
                if (cc.MatchObjects[team].Count == 0)
                    cc.MatchObjects.Remove(team);
            }

        }

        /// <summary>
        /// Gets condition collections for a NetowrkManager.
        /// </summary>
        private static ConditionData GetCollections(NetworkManager manager = null)
        {
            if (manager == null)
                manager = InstanceFinder.NetworkManager;

            ConditionData cc;
            if (!_collections.TryGetValue(manager, out cc))
            {
                cc = new ConditionData();
                _collections[manager] = cc;
            }

            return cc;
        }

        public void ConditionConstructor()
        {
        }

        /// <summary>
        /// How a condition is handled.
        /// </summary>
        /// <returns></returns>
        public override ObserverConditionType GetConditionType() => ObserverConditionType.Timed;

        public override ObserverCondition Clone()
        {
            TeamCondition copy = ScriptableObject.CreateInstance<TeamCondition>();
            copy.ConditionConstructor();
            return copy;
        }
#else
        // 无 FishNet 版本 - 提供静态方法的空实现，避免其他代码调用时出错
        public static void AddObjectDistance(object obj, float sameTeamDistance, float otherTeamDistance, object manager = null)
        {
            // 无 FishNet 时不执行
        }

        public static void RemoveObjectDistance(object obj, object manager = null)
        {
            // 无 FishNet 时不执行
        }

        public static void AddToTeam(object no, long team, object manager = null)
        {
            // 无 FishNet 时不执行
        }

        public static void RemoveFromTeam(object connection, object manager = null)
        {
            // 无 FishNet 时不执行
        }
#endif
    }
}
