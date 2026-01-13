using System;
using System.Collections.Generic;

namespace Yomov.Network
{
    /// <summary>
    /// 服务器管理器接口
    /// 提供服务器端的网络功能
    /// </summary>
    public interface IServerManager
    {
        /// <summary>
        /// 启动服务器连接
        /// </summary>
        /// <param name="port">监听端口</param>
        void StartConnection(ushort port);
        
        /// <summary>
        /// 注册广播消息处理器（服务器端，不带连接参数）
        /// </summary>
        /// <typeparam name="T">广播消息类型（必须是值类型）</typeparam>
        /// <param name="handler">处理器回调</param>
        void RegisterBroadcast<T>(Action<T> handler) where T : struct;
        
        /// <summary>
        /// 注册广播消息处理器（带连接参数）
        /// </summary>
        /// <typeparam name="T">广播消息类型（必须是值类型）</typeparam>
        /// <param name="handler">处理器回调，包含连接信息和消息</param>
        void RegisterBroadcast<T>(Action<INetworkConnection, T> handler) where T : struct;
        
        /// <summary>
        /// 向指定客户端广播消息
        /// </summary>
        /// <typeparam name="T">广播消息类型（必须是值类型）</typeparam>
        /// <param name="connection">目标客户端连接</param>
        /// <param name="message">要发送的消息</param>
        void Broadcast<T>(INetworkConnection connection, T message) where T : struct;
        
        /// <summary>
        /// 向所有客户端广播消息
        /// </summary>
        /// <typeparam name="T">广播消息类型（必须是值类型）</typeparam>
        /// <param name="message">要发送的消息</param>
        void Broadcast<T>(T message) where T : struct;
        
        /// <summary>
        /// 在网络中生成对象
        /// </summary>
        /// <param name="networkObject">要生成的网络对象</param>
        /// <param name="ownerConnection">对象所有者（可选，默认为服务器所有）</param>
        void Spawn(INetworkObject networkObject, INetworkConnection ownerConnection = null);
        
        /// <summary>
        /// 销毁网络对象
        /// </summary>
        /// <param name="networkObject">要销毁的网络对象</param>
        void Despawn(INetworkObject networkObject);
        
        /// <summary>
        /// 踢出指定客户端
        /// </summary>
        /// <param name="clientId">客户端ID</param>
        /// <param name="reason">踢出原因</param>
        void Kick(int clientId, object reason);
        
        /// <summary>
        /// 获取所有已连接的客户端字典（只读）
        /// Key: 客户端ID, Value: 连接对象
        /// </summary>
        IReadOnlyDictionary<int, INetworkConnection> Clients { get; }
        
        /// <summary>
        /// 服务器连接状态变化事件
        /// </summary>
        event Action<ServerConnectionStateArgs> OnServerConnectionState;
        
        /// <summary>
        /// 远程客户端连接状态变化事件
        /// </summary>
        event Action<INetworkConnection, RemoteConnectionStateArgs> OnRemoteConnectionState;
    }
}

