#if FISHNET_3_10_8
using FishNet.Broadcast;
#endif

namespace Yomov
{
    public enum NetEventType
    {
        S2C,
        C2S,
        TeamEvent,
    }
    
    public struct BroadcastData
#if FISHNET_3_10_8
        : IBroadcast
#endif
    {
        public string type;

        public string data;
        
        public NetEventType netEventType;
        
        public BroadcastData(string data, string type, NetEventType netEventType)
        {
            this.data = data;
            this.type = type;
            this.netEventType = netEventType;
        }

        public BroadcastData(IMessage message)
        {
            data = JsonTool.SerializeObject(message);
            type = message.GetType().AssemblyQualifiedName;
            netEventType = NetEventType.S2C;
        }
        
        public BroadcastData(IMessage message, NetEventType netEventType)
        {
            data = JsonTool.SerializeObject(message);
            type = message.GetType().AssemblyQualifiedName;
            this.netEventType = netEventType;
        }
    }
}
