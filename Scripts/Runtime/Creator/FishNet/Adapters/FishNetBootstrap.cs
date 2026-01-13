using UnityEngine;

namespace Yomov.Network.FishNet
{
    /// <summary>
    /// FishNet 网络服务初始化器
    /// 在游戏启动时自动初始化 NetworkServiceProvider
    /// </summary>
    public static class FishNetBootstrap
    {
        /// <summary>
        /// 在场景加载前自动初始化网络服务
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
#if FISHNET_3_10_8
            Debug.Log("[FishNetBootstrap] 开始初始化 FishNet 网络服务...");
            
            try
            {
                // 创建 FishNet 网络管理器实例
                var networkManager = new FishNetNetworkManager();
                
                // 注册到 NetworkServiceProvider
                NetworkServiceProvider.Initialize(networkManager);
                
                Debug.Log("[FishNetBootstrap] FishNet 网络服务初始化完成！");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[FishNetBootstrap] FishNet 网络服务初始化失败: {ex.Message}\n{ex.StackTrace}");
            }
#else
            Debug.LogWarning("[FishNetBootstrap] FishNet 包未安装，跳过网络服务初始化");
#endif
        }
    }
}
