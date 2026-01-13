#if FISHNET_3_10_8
using FishNet;
#endif

namespace Yomov.Network.FishNet
{
    /// <summary>
    /// FishNet 传输管理器适配器
    /// 实现 ITransportManager 接口，包装 FishNet 的 TransportManager
    /// </summary>
    public class FishNetTransportManager : ITransportManager
    {
        /// <summary>
        /// 获取连接状态
        /// </summary>
        /// <param name="asServer">是否作为服务器查询状态</param>
        /// <returns>本地连接状态</returns>
        public LocalConnectionState GetConnectionState(bool asServer)
        {
#if FISHNET_3_10_8
            if (InstanceFinder.TransportManager != null && InstanceFinder.TransportManager.Transport != null)
            {
                var fishNetState = InstanceFinder.TransportManager.Transport.GetConnectionState(asServer);
                return (LocalConnectionState)fishNetState;
            }
#endif
            return LocalConnectionState.Stopped;
        }
    }
}
