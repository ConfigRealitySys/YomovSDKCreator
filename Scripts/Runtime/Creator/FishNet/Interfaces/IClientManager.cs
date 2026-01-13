using System;

namespace Yomov.Network
{
    /// <summary>
    /// 客户端管理器接口
    /// 提供客户端的网络功能
    /// </summary>
    public interface IClientManager
    {
        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="address">服务器地址</param>
        /// <param name="port">服务器端口</param>
        void StartConnection(string address, ushort port);
        
        /// <summary>
        /// 连接到服务器（使用默认配置）
        /// </summary>
        void StartConnection();
        
        /// <summary>
        /// 注册广播消息处理器
        /// </summary>
        /// <typeparam name="T">广播消息类型（必须是值类型）</typeparam>
        /// <param name="handler">处理器回调</param>
        void RegisterBroadcast<T>(Action<T> handler) where T : struct;
        
        /// <summary>
        /// 向服务器广播消息
        /// </summary>
        /// <typeparam name="T">广播消息类型（必须是值类型）</typeparam>
        /// <param name="message">要发送的消息</param>
        void Broadcast<T>(T message) where T : struct;
        
        /// <summary>
        /// 当前客户端的连接
        /// </summary>
        INetworkConnection Connection { get; }
        
        /// <summary>
        /// 客户端连接状态变化事件
        /// </summary>
        event Action<ClientConnectionStateArgs> OnClientConnectionState;
    }
}

