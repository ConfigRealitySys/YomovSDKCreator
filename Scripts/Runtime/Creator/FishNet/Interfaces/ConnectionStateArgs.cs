namespace Yomov.Network
{
    /// <summary>
    /// 本地连接状态枚举
    /// </summary>
    public enum LocalConnectionState
    {
        /// <summary>
        /// 未启动
        /// </summary>
        Stopped = 0,
        
        /// <summary>
        /// 正在启动
        /// </summary>
        Starting = 1,
        
        /// <summary>
        /// 已启动
        /// </summary>
        Started = 2,
        
        /// <summary>
        /// 正在停止
        /// </summary>
        Stopping = 3
    }
    
    /// <summary>
    /// 远程连接状态枚举
    /// 注意：枚举值与 FishNet.Transporting.RemoteConnectionState 保持一致
    /// </summary>
    public enum RemoteConnectionState
    {
        /// <summary>
        /// 已停止
        /// </summary>
        Stopped = 0,
        
        /// <summary>
        /// 已启动
        /// </summary>
        Started = 2  // 与 FishNet 保持一致
    }
    
    /// <summary>
    /// 服务器连接状态参数
    /// </summary>
    public struct ServerConnectionStateArgs
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public LocalConnectionState ConnectionState;
    }
    
    /// <summary>
    /// 远程连接状态参数
    /// </summary>
    public struct RemoteConnectionStateArgs
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public RemoteConnectionState ConnectionState;
    }
    
    /// <summary>
    /// 客户端连接状态参数
    /// </summary>
    public struct ClientConnectionStateArgs
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public LocalConnectionState ConnectionState;
    }
}


