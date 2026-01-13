namespace Yomov.Network
{
    /// <summary>
    /// 场景管理器接口
    /// 提供网络场景管理功能
    /// </summary>
    public interface ISceneManager
    {
        /// <summary>
        /// 将对象的所有者添加到默认场景
        /// </summary>
        /// <param name="networkObject">网络对象</param>
        void AddOwnerToDefaultScene(INetworkObject networkObject);
    }
}

