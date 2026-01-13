using System;
#if FISHNET_3_10_8
using FishNet;
using FishNet.Broadcast;
using FishNet.Managing.Client;
using FishNet.Transporting;
#endif
using UnityEngine;

namespace Yomov.Network.FishNet
{
    /// <summary>
    /// FishNet 客户端管理器适配器
    /// 实现 IClientManager 接口，包装 FishNet 的 ClientManager
    /// </summary>
    public class FishNetClientManager : IClientManager
    {
#if FISHNET_3_10_8
        private FishNetConnection _cachedConnection;
        private bool _eventsInitialized = false;
#endif
        
        public event Action<ClientConnectionStateArgs> OnClientConnectionState;
        
        /// <summary>
        /// 确保事件已初始化（延迟初始化）
        /// </summary>
        private void EnsureEventsInitialized()
        {
#if FISHNET_3_10_8
            if (!_eventsInitialized && InstanceFinder.ClientManager != null)
            {
                Debug.Log("[FishNetClientManager] 延迟初始化客户端事件订阅");
                InstanceFinder.ClientManager.OnClientConnectionState += HandleClientConnectionState;
                _eventsInitialized = true;
            }
#endif
        }
        
        /// <summary>
        /// 注册广播消息处理器
        /// </summary>
        public void RegisterBroadcast<T>(Action<T> handler) where T : struct
        {
#if FISHNET_3_10_8
            EnsureEventsInitialized();
            if (InstanceFinder.ClientManager != null)
            {
                // 确保 T 实现了 IBroadcast（在适配器层处理）
                if (typeof(global::FishNet.Broadcast.IBroadcast).IsAssignableFrom(typeof(T)))
                {
                    try
                    {
                        var clientManagerType = typeof(global::FishNet.Managing.Client.ClientManager);
                        var registerMethod = clientManagerType.GetMethod("RegisterBroadcast", 
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        
                        if (registerMethod != null && registerMethod.IsGenericMethod)
                        {
                            var genericMethod = registerMethod.MakeGenericMethod(typeof(T));
                            genericMethod.Invoke(InstanceFinder.ClientManager, new object[] { handler });
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[FishNetClientManager] RegisterBroadcast 反射调用失败: {ex.Message}");
                    }
                }
            }
#else
            // 无 FishNet 时不执行
#endif
        }
        
        /// <summary>
        /// 向服务器广播消息
        /// </summary>
        public void Broadcast<T>(T message) where T : struct
        {
#if FISHNET_3_10_8
            if (InstanceFinder.ClientManager != null && typeof(global::FishNet.Broadcast.IBroadcast).IsAssignableFrom(typeof(T)))
            {
                try
                {
                    var clientManagerType = typeof(global::FishNet.Managing.Client.ClientManager);
                    var broadcastMethod = clientManagerType.GetMethod("Broadcast", 
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    
                    if (broadcastMethod != null && broadcastMethod.IsGenericMethod)
                    {
                        var genericMethod = broadcastMethod.MakeGenericMethod(typeof(T));
                        // ClientManager.Broadcast<T>(T message, Channel channel = Channel.Reliable)
                        // 参数: message, channel (使用默认值 Channel.Reliable = 0)
                        genericMethod.Invoke(InstanceFinder.ClientManager, new object[] { message, (byte)0 });
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[FishNetClientManager] Broadcast 反射调用失败: {ex.Message}");
                }
            }
#else
            // 无 FishNet 时不执行
#endif
        }
        
        /// <summary>
        /// 当前客户端的连接
        /// </summary>
        public INetworkConnection Connection
        {
            get
            {
#if FISHNET_3_10_8
                if (InstanceFinder.ClientManager != null && InstanceFinder.ClientManager.Connection != null)
                {
                    // 使用缓存，避免频繁创建对象
                    if (_cachedConnection == null || _cachedConnection.InternalConnection != InstanceFinder.ClientManager.Connection)
                    {
                        _cachedConnection = new FishNetConnection(InstanceFinder.ClientManager.Connection);
                    }
                    return _cachedConnection;
                }
#endif
                return null;
            }
        }
        
        /// <summary>
        /// 连接到服务器
        /// </summary>
        public void StartConnection(string address, ushort port)
        {
#if FISHNET_3_10_8
            EnsureEventsInitialized();
            Debug.Log($"[FishNetClientManager] StartConnection 地址: {address}:{port}");
            if (InstanceFinder.ClientManager != null)
            {
                InstanceFinder.ClientManager.StartConnection(address, port);
            }
            else
            {
                Debug.LogError("[FishNetClientManager] InstanceFinder.ClientManager 为 null，无法连接服务器");
            }
#else
            Debug.LogWarning("[FishNet] FishNet 包未安装，无法连接服务器");
#endif
        }
        
        /// <summary>
        /// 连接到服务器（使用默认配置）
        /// </summary>
        public void StartConnection()
        {
#if FISHNET_3_10_8
            EnsureEventsInitialized();
            Debug.Log("[FishNetClientManager] StartConnection (使用默认配置)");
            if (InstanceFinder.ClientManager != null)
            {
                InstanceFinder.ClientManager.StartConnection();
            }
            else
            {
                Debug.LogError("[FishNetClientManager] InstanceFinder.ClientManager 为 null，无法连接服务器");
            }
#else
            Debug.LogWarning("[FishNet] FishNet 包未安装，无法连接服务器");
#endif
        }
        
#if FISHNET_3_10_8
        private void HandleClientConnectionState(global::FishNet.Transporting.ClientConnectionStateArgs args)
        {
            Debug.Log($"[FishNetClientManager] 客户端连接状态变化: {args.ConnectionState}");
            var wrappedArgs = new ClientConnectionStateArgs
            {
                ConnectionState = (LocalConnectionState)args.ConnectionState
            };
            Debug.Log($"[FishNetClientManager] 触发 OnClientConnectionState 事件，订阅者数量: {OnClientConnectionState?.GetInvocationList().Length ?? 0}");
            OnClientConnectionState?.Invoke(wrappedArgs);
        }
#endif
    }
}
