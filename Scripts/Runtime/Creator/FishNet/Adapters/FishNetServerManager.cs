using System;
using System.Collections.Generic;
#if FISHNET_3_10_8
using System.Linq;
using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using FishNet.Managing.Server;
#endif
using UnityEngine;

namespace Yomov.Network.FishNet
{
    /// <summary>
    /// FishNet 服务器管理器适配器
    /// 实现 IServerManager 接口，包装 FishNet 的 ServerManager
    /// </summary>
    public class FishNetServerManager : IServerManager
    {
#if FISHNET_3_10_8
        private readonly Dictionary<int, INetworkConnection> _clientsCache = new Dictionary<int, INetworkConnection>();
        private readonly Dictionary<NetworkConnection, FishNetConnection> _connectionCache = new Dictionary<NetworkConnection, FishNetConnection>();
        private bool _eventsInitialized = false;
#endif
        
        public event Action<ServerConnectionStateArgs> OnServerConnectionState;
        public event Action<INetworkConnection, RemoteConnectionStateArgs> OnRemoteConnectionState;
        
        /// <summary>
        /// 确保事件已初始化（延迟初始化）
        /// </summary>
        private void EnsureEventsInitialized()
        {
#if FISHNET_3_10_8
            if (!_eventsInitialized && InstanceFinder.ServerManager != null)
            {
                Debug.Log("[FishNetServerManager] 延迟初始化服务器事件订阅");
                InstanceFinder.ServerManager.OnServerConnectionState += HandleServerConnectionState;
                InstanceFinder.ServerManager.OnRemoteConnectionState += HandleRemoteConnectionState;
                _eventsInitialized = true;
            }
#endif
        }
        
        /// <summary>
        /// 注册广播消息处理器（不带连接参数）
        /// </summary>
        public void RegisterBroadcast<T>(Action<T> handler) where T : struct
        {
#if FISHNET_3_10_8
            EnsureEventsInitialized();
            if (InstanceFinder.ServerManager != null)
            {
                // 确保 T 实现了 IBroadcast（在适配器层处理）
                if (typeof(global::FishNet.Broadcast.IBroadcast).IsAssignableFrom(typeof(T)))
                {
                    try
                    {
                        // FishNet 的 ServerManager.RegisterBroadcast 只有带 NetworkConnection 参数的版本
                        // 所以我们包装一下，忽略连接参数
                        Action<NetworkConnection, T> wrappedHandler = (conn, data) => handler(data);
                        
                        // 使用反射调用，不使用 dynamic（避免 lambda 类型推断问题）
                        var serverManagerType = typeof(global::FishNet.Managing.Server.ServerManager);
                        var registerMethod = serverManagerType.GetMethod("RegisterBroadcast", 
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        
                        if (registerMethod != null && registerMethod.IsGenericMethod)
                        {
                            var genericMethod = registerMethod.MakeGenericMethod(typeof(T));
                            genericMethod.Invoke(InstanceFinder.ServerManager, new object[] { wrappedHandler, true });
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[FishNetServerManager] RegisterBroadcast 反射调用失败: {ex.Message}");
                    }
                }
            }
#endif
        }
        
        /// <summary>
        /// 注册广播消息处理器（带连接参数）
        /// </summary>
        public void RegisterBroadcast<T>(Action<INetworkConnection, T> handler) where T : struct
        {
#if FISHNET_3_10_8
            if (InstanceFinder.ServerManager != null)
            {
                // 确保 T 实现了 IBroadcast
                if (typeof(global::FishNet.Broadcast.IBroadcast).IsAssignableFrom(typeof(T)))
                {
                    try
                    {
                        Action<NetworkConnection, T> wrappedHandler = (conn, data) =>
                        {
                            var wrappedConn = WrapConnection(conn);
                            handler(wrappedConn, data);
                        };
                        
                        // 使用反射调用，不使用 dynamic（避免 lambda 类型推断问题）
                        var serverManagerType = typeof(global::FishNet.Managing.Server.ServerManager);
                        var registerMethod = serverManagerType.GetMethod("RegisterBroadcast", 
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        
                        if (registerMethod != null && registerMethod.IsGenericMethod)
                        {
                            var genericMethod = registerMethod.MakeGenericMethod(typeof(T));
                            genericMethod.Invoke(InstanceFinder.ServerManager, new object[] { wrappedHandler, true });
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[FishNetServerManager] RegisterBroadcast(带连接参数) 反射调用失败: {ex.Message}");
                    }
                }
            }
#endif
        }
        
        /// <summary>
        /// 向指定客户端广播消息
        /// </summary>
        public void Broadcast<T>(INetworkConnection connection, T message) where T : struct
        {
#if FISHNET_3_10_8
            if (InstanceFinder.ServerManager != null)
            {
                var fishNetConn = UnwrapConnection(connection);
                if (fishNetConn != null && typeof(global::FishNet.Broadcast.IBroadcast).IsAssignableFrom(typeof(T)))
                {
                    try
                    {
                        var serverManagerType = typeof(global::FishNet.Managing.Server.ServerManager);
                        // ServerManager.Broadcast<T>(NetworkConnection, T, bool requireAuthenticated = true, Channel channel = Channel.Reliable)
                        var broadcastMethod = serverManagerType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                            .FirstOrDefault(m => 
                                m.Name == "Broadcast" && 
                                m.IsGenericMethod && 
                                m.GetParameters().Length == 4 &&
                                m.GetParameters()[0].ParameterType == typeof(NetworkConnection));
                        
                        if (broadcastMethod != null)
                        {
                            var genericMethod = broadcastMethod.MakeGenericMethod(typeof(T));
                            // 参数: connection, message, requireAuthenticated, channel
                            genericMethod.Invoke(InstanceFinder.ServerManager, new object[] { fishNetConn, message, true, (byte)0 });
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[FishNetServerManager] Broadcast(指定客户端) 反射调用失败: {ex.Message}");
                    }
                }
            }
#endif
        }
        
        /// <summary>
        /// 向所有客户端广播消息
        /// </summary>
        public void Broadcast<T>(T message) where T : struct
        {
#if FISHNET_3_10_8
            if (InstanceFinder.ServerManager != null && typeof(global::FishNet.Broadcast.IBroadcast).IsAssignableFrom(typeof(T)))
            {
                try
                {
                    var serverManagerType = typeof(global::FishNet.Managing.Server.ServerManager);
                    // ServerManager.Broadcast<T>(T message, bool requireAuthenticated = true, Channel channel = Channel.Reliable)
                    var broadcastMethod = serverManagerType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                        .FirstOrDefault(m => 
                            m.Name == "Broadcast" && 
                            m.IsGenericMethod && 
                            m.GetParameters().Length == 3 &&
                            m.GetParameters()[0].ParameterType.IsGenericParameter);
                    
                    if (broadcastMethod != null)
                    {
                        var genericMethod = broadcastMethod.MakeGenericMethod(typeof(T));
                        // 参数: message, requireAuthenticated, channel
                        genericMethod.Invoke(InstanceFinder.ServerManager, new object[] { message, true, (byte)0 });
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[FishNetServerManager] Broadcast(所有客户端) 反射调用失败: {ex.Message}");
                }
            }
#endif
        }
        
        /// <summary>
        /// 获取所有已连接的客户端字典（只读）
        /// </summary>
        public IReadOnlyDictionary<int, INetworkConnection> Clients
        {
            get
            {
#if FISHNET_3_10_8
                _clientsCache.Clear();
                if (InstanceFinder.ServerManager != null)
                {
                    foreach (var kvp in InstanceFinder.ServerManager.Clients)
                    {
                        _clientsCache[kvp.Key] = WrapConnection(kvp.Value);
                    }
                }
                return _clientsCache;
#else
                return new Dictionary<int, INetworkConnection>();
#endif
            }
        }
        
#if FISHNET_3_10_8
        /// <summary>
        /// 包装 FishNet NetworkConnection 为 INetworkConnection
        /// 使用缓存避免重复创建
        /// </summary>
        private FishNetConnection WrapConnection(NetworkConnection conn)
        {
            if (conn == null)
                return null;
                
            if (!_connectionCache.TryGetValue(conn, out var wrapped))
            {
                wrapped = new FishNetConnection(conn);
                _connectionCache[conn] = wrapped;
            }
            return wrapped;
        }
        
        /// <summary>
        /// 解包 INetworkConnection 获取内部的 FishNet NetworkConnection
        /// </summary>
        private NetworkConnection UnwrapConnection(INetworkConnection conn)
        {
            return (conn as FishNetConnection)?.InternalConnection;
        }
#endif
        
        /// <summary>
        /// 启动服务器连接
        /// </summary>
        public void StartConnection(ushort port)
        {
#if FISHNET_3_10_8
            EnsureEventsInitialized();
            Debug.Log($"[FishNetServerManager] StartConnection 端口: {port}");
            if (InstanceFinder.ServerManager != null)
            {
                InstanceFinder.ServerManager.StartConnection(port);
            }
            else
            {
                Debug.LogError("[FishNetServerManager] InstanceFinder.ServerManager 为 null，无法启动服务器");
            }
#else
            Debug.LogWarning("[FishNet] FishNet 包未安装，无法启动服务器");
#endif
        }
        
        /// <summary>
        /// 在网络中生成对象
        /// </summary>
        public void Spawn(INetworkObject networkObject, INetworkConnection ownerConnection = null)
        {
#if FISHNET_3_10_8
            if (InstanceFinder.ServerManager != null)
            {
                var fishNetObj = (networkObject as FishNetNetworkObject)?.UnderlyingNetworkObject;
                if (fishNetObj != null)
                {
                    var fishNetConn = UnwrapConnection(ownerConnection);
                    InstanceFinder.ServerManager.Spawn(fishNetObj, fishNetConn);
                }
            }
#endif
        }
        
        /// <summary>
        /// 销毁网络对象
        /// </summary>
        public void Despawn(INetworkObject networkObject)
        {
#if FISHNET_3_10_8
            if (InstanceFinder.ServerManager != null)
            {
                var fishNetObj = (networkObject as FishNetNetworkObject)?.UnderlyingNetworkObject;
                if (fishNetObj != null)
                {
                    InstanceFinder.ServerManager.Despawn(fishNetObj);
                }
            }
#endif
        }
        
        /// <summary>
        /// 踢出指定客户端
        /// </summary>
        public void Kick(int clientId, object reason)
        {
#if FISHNET_3_10_8
            if (InstanceFinder.ServerManager != null)
            {
                var kickReason = reason is global::FishNet.Managing.Server.KickReason kr ? kr : global::FishNet.Managing.Server.KickReason.Unset;
                InstanceFinder.ServerManager.Kick(clientId, kickReason);
            }
#endif
        }
        
#if FISHNET_3_10_8
        private void HandleServerConnectionState(global::FishNet.Transporting.ServerConnectionStateArgs args)
        {
            Debug.Log($"[FishNetServerManager] 服务器连接状态变化: {args.ConnectionState}");
            var wrappedArgs = new ServerConnectionStateArgs
            {
                ConnectionState = (LocalConnectionState)args.ConnectionState
            };
            Debug.Log($"[FishNetServerManager] 触发 OnServerConnectionState 事件，订阅者数量: {OnServerConnectionState?.GetInvocationList().Length ?? 0}");
            OnServerConnectionState?.Invoke(wrappedArgs);
        }
        
        private void HandleRemoteConnectionState(NetworkConnection conn, global::FishNet.Transporting.RemoteConnectionStateArgs args)
        {
            Debug.Log($"[FishNetServerManager] 远程连接状态变化: ClientId={conn?.ClientId}, State={args.ConnectionState}");
            var wrappedConn = WrapConnection(conn);
            
            // 枚举值已对齐，可以直接转换
            // FishNet 和 Yomov: Stopped=0, Started=2
            var wrappedArgs = new RemoteConnectionStateArgs
            {
                ConnectionState = (RemoteConnectionState)args.ConnectionState
            };
            Debug.Log($"[FishNetServerManager] 触发 OnRemoteConnectionState 事件，订阅者数量: {OnRemoteConnectionState?.GetInvocationList().Length ?? 0}");
            OnRemoteConnectionState?.Invoke(wrappedConn, wrappedArgs);
        }
#endif
    }
}
