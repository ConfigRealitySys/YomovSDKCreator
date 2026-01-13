using System;
using UnityEngine;

namespace Yomov.Network
{
    /// <summary>
    /// 网络服务提供者（单例）
    /// 提供统一的网络服务访问入口，解耦具体网络框架实现
    /// </summary>
    public static class NetworkServiceProvider
    {
        private static INetworkManager _instance;
        
        /// <summary>
        /// 获取网络管理器实例
        /// </summary>
        /// <exception cref="InvalidOperationException">如果未初始化则抛出异常</exception>
        public static INetworkManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[NetworkServiceProvider] 网络服务未初始化！请确保在使用前调用 Initialize() 方法。");
                    throw new InvalidOperationException("NetworkServiceProvider 未初始化！请先调用 Initialize() 方法。");
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// 检查网络服务是否已初始化
        /// </summary>
        public static bool IsInitialized => _instance != null;
        
        /// <summary>
        /// 初始化网络服务（由网络框架适配器调用）
        /// </summary>
        /// <param name="networkManager">网络管理器实现</param>
        /// <exception cref="ArgumentNullException">如果 networkManager 为 null</exception>
        /// <exception cref="InvalidOperationException">如果已经初始化</exception>
        public static void Initialize(INetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException(nameof(networkManager), "网络管理器不能为 null");
            }
            
            if (_instance != null)
            {
                Debug.LogWarning("[NetworkServiceProvider] 网络服务已经初始化，将覆盖现有实例。");
            }
            
            _instance = networkManager;
            Debug.Log("[NetworkServiceProvider] 网络服务初始化完成");
        }
        
        /// <summary>
        /// 重置网络服务（主要用于测试）
        /// </summary>
        internal static void Reset()
        {
            _instance = null;
            Debug.Log("[NetworkServiceProvider] 网络服务已重置");
        }
    }
}

