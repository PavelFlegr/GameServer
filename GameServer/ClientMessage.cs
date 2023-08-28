using System.Numerics;
using System.Text.Json.Serialization;

namespace GameServer
{
    public enum ClientMsgType
    {
        Connect,
        Move,
        Disconnect,
    }

    class MoveMsgC
    {
        [JsonInclude]
        public Vector3 position;
    }
}
