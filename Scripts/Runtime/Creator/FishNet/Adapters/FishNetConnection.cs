using System.Collections.Generic;
#if FISHNET_3_10_8
using System.Linq;
using FishNet.Connection;
using FishNet.Transporting;
#endif

namespace Yomov.Network.FishNet
{
    /// <summary>
    /// FishNet 网络连接适配器
    /// 将 FishNet 的 NetworkConnection 包装为 INetworkConnection 接口
    /// </summary>
    public class FishNetConnection : INetworkConnection
    {
#if FISHNET_3_10_8
        /// <summary>
        /// 内部的 FishNet NetworkConnection 对象
        /// </summary>
        internal NetworkConnection InternalConnection { get; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">FishNet 的 NetworkConnection</param>
        public FishNetConnection(NetworkConnection connection)
        {
            InternalConnection = connection;
        }
        
        /// <summary>
        /// 客户端 ID
        /// </summary>
        public int ClientId => InternalConnection.ClientId;
        
        /// <summary>
        /// 获取连接的地址（IP:Port）
        /// </summary>
        public string GetAddress()
        {
            return InternalConnection.GetAddress();
        }
        
        /// <summary>
        /// 连接是否处于激活状态
        /// </summary>
        public bool IsActive => InternalConnection.IsActive;
        
        /// <summary>
        /// 该连接拥有的所有网络对象
        /// </summary>
        public IReadOnlyCollection<INetworkObject> Objects
        {
            get
            {
                return InternalConnection.Objects
                    .Select(obj => (INetworkObject)new FishNetNetworkObject(obj))
                    .ToList();
            }
        }
        
        /// <summary>
        /// 第一个网络对象（通常是玩家对象）
        /// </summary>
        public INetworkObject FirstObject
        {
            get
            {
                var firstObj = InternalConnection.FirstObject;
                return firstObj != null ? new FishNetNetworkObject(firstObj) : null;
            }
        }
        
        /// <summary>
        /// 踢出该连接的客户端
        /// </summary>
        /// <param name="reason">踢出原因</param>
        public void Kick(object reason)
        {
            var kickReason = reason is global::FishNet.Managing.Server.KickReason kr ? kr : global::FishNet.Managing.Server.KickReason.Unset;
            InternalConnection.Kick(kickReason);
        }
        
        /// <summary>
        /// 隐式转换：从 FishNet NetworkConnection 转换为 FishNetConnection
        /// </summary>
        public static implicit operator FishNetConnection(NetworkConnection connection)
        {
            return connection != null ? new FishNetConnection(connection) : null;
        }
#else
        /// <summary>
        /// 内部的连接对象（无 FishNet 版本）
        /// </summary>
        internal object InternalConnection { get; }
        
        public FishNetConnection(object connection)
        {
            InternalConnection = connection;
        }
        
        public int ClientId => -1;
        
        public string GetAddress()
        {
            return "0.0.0.0:0";
        }
        
        public bool IsActive => false;
        
        public IReadOnlyCollection<INetworkObject> Objects => new List<INetworkObject>();
        
        public INetworkObject FirstObject => null;
        
        public void Kick(object reason)
        {
            // 无 FishNet 时不执行
        }
#endif
    }
}
