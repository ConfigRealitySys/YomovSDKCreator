namespace Yomov.Network
{
    /// <summary>
    /// 传输管理器接口
    /// 提供网络传输层的功能
    /// </summary>
    public interface ITransportManager
    {
        /// <summary>
        /// 获取连接状态
        /// </summary>
        /// <param name="asServer">是否作为服务器查询状态</param>
        /// <returns>本地连接状态</returns>
        LocalConnectionState GetConnectionState(bool asServer);
    }
}

