using System.Numerics;
using System.Text.Json.Serialization;

namespace GameServer
{
    public enum ServerMessageType
    {
        Welcome,
        Move,
        Connect,
        Disconnect,
    }

    class WelcomeMsgS
    {
        [JsonInclude]
        public int playerId;
        [JsonInclude]
        public Character[] characters;
    }

    class MoveMsgS
    {
        [JsonInclude]
        public int entityId;
        [JsonInclude]
        public Vector3 position;
    }

    class DisconnectMsgS
    {
        [JsonInclude]
        public int entityId;
    }

    public class ConnectMsgS
    {
        [JsonInclude]
        public Character character;
    }
}
