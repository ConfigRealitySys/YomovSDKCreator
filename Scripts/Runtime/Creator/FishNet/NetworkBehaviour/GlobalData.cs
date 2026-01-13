namespace Yomov
{
    public enum ClientType
    {
        Server,
        Client,
        // Host,
    }
    
    public class FishNetConfig
    {
        //fishnet同步avatar数据时间间隔
        public const float AvatarSyncTime = 0.1f;
        public static ClientType ClientType = ClientType.Server;
    }    
}
