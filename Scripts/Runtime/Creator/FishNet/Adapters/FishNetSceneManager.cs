#if FISHNET_3_10_8
using FishNet;
#endif

namespace Yomov.Network.FishNet
{
    /// <summary>
    /// FishNet 场景管理器适配器
    /// 实现 ISceneManager 接口，包装 FishNet 的 SceneManager
    /// </summary>
    public class FishNetSceneManager : ISceneManager
    {
        /// <summary>
        /// 将对象的所有者添加到默认场景
        /// </summary>
        /// <param name="networkObject">网络对象</param>
        public void AddOwnerToDefaultScene(INetworkObject networkObject)
        {
#if FISHNET_3_10_8
            if (InstanceFinder.NetworkManager != null && InstanceFinder.NetworkManager.SceneManager != null)
            {
                var fishNetObj = (networkObject as FishNetNetworkObject)?.UnderlyingNetworkObject;
                if (fishNetObj != null)
                {
                    InstanceFinder.NetworkManager.SceneManager.AddOwnerToDefaultScene(fishNetObj);
                }
            }
#else
            // 无 FishNet 时不执行
#endif
        }
    }
}
