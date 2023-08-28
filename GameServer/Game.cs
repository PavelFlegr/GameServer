using NetMQ;
using NetMQ.Sockets;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json;

namespace GameServer
{
    internal class Game
    {
        Dictionary<NetMQFrame, Character> players = new Dictionary<NetMQFrame, Character>();
        RouterSocket server = new RouterSocket("@tcp://0.0.0.0:5555");
        JsonSerializerOptions serializeOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new Vector3JsonConverter(),
            }
        };

        Character addNewPlayer(NetMQFrame clientAddress)
        {
            var entity = new Character
            {
                speed = 100,
                position = new Vector3(0, 0, 0),
                targetPosition = new Vector3(0, 0, 0),
                range = 20,
                attack = 10,
                aspd = 1,
                attackDelay = 0,
                maxLife = 100,
                life = 100,
            };

            return entity;
        }

        void SendMsg<T>(NetMQFrame clientAddress, ServerMessageType type, T msg) where T: class
        {
            var serverMsg = new NetMQMessage();
            serverMsg.Append(clientAddress);
            serverMsg.AppendEmptyFrame();
            var serialized = JsonSerializer.Serialize(msg, serializeOptions);
            serverMsg.Append((int)type);
            serverMsg.Append(serialized);
            server.SendMultipartMessage(serverMsg);
        }

        void gameLoop(float deltaTime)
        {
            foreach (var player in players.Values)
            {
                Vector3 distance = player.targetPosition - player.position;
                bool isWalking = player.targetEnemy != null ? distance.Length() > player.range : distance.Length() > 5;
                if (isWalking || player.targetEnemy != null)
                {
                    // player.rotation = Quaternion.LookRotation(distance, Vector3.up);
                }
                if (isWalking)
                {
                    player.position += distance / distance.Length() * player.speed * deltaTime;
                }
            }
        }

        public void Run()
        {
            Stopwatch sw = Stopwatch.StartNew();
            Stopwatch sw2 = Stopwatch.StartNew();
            while (true)
            {
                while (server.HasIn)
                {
                    var clientMsg = server.ReceiveMultipartMessage();
                    if (clientMsg.FrameCount == 4)
                    {
                        var clientAddress = clientMsg[0];
                        ClientMsgType msgType = (ClientMsgType)clientMsg[2].ConvertToInt32();
                        Character player;
                        switch (msgType)
                        {
                            case ClientMsgType.Connect:
                                player = addNewPlayer(clientAddress);
                                var connect = new ConnectMsgS { character = player };
                                SendMsgAll(ServerMessageType.Connect, connect);
                                players[clientAddress] = player;
                                var welcome = new WelcomeMsgS { playerId = player.id, characters = players.Values.ToArray() };
                                SendMsg(clientAddress, ServerMessageType.Welcome, welcome);
                                break;
                            case ClientMsgType.Disconnect:
                                player = players[clientAddress];
                                players.Remove(clientAddress);
                                var disconnect = new DisconnectMsgS { entityId = player.id };
                                SendMsgAll(ServerMessageType.Disconnect, disconnect);
                                break;
                            case ClientMsgType.Move:
                                var payload = JsonSerializer.Deserialize<MoveMsgC>(clientMsg[3].ConvertToString(), serializeOptions);
                                player = players[clientAddress];
                                player.targetPosition = payload.position;
                                var move = new MoveMsgS {
                                    entityId = player.id,
                                    position = player.targetPosition,
                                };
                                SendMsgAll(ServerMessageType.Move, move);
                                break;
                        }
                    }
                }

                var timeDelta = (float)sw.Elapsed.TotalSeconds;
                sw.Restart();
                gameLoop(timeDelta);
                if (sw2.ElapsedMilliseconds > 1000)
                {
                    Console.WriteLine(JsonSerializer.Serialize(players.Values, serializeOptions));
                    sw2.Restart();
                }
            }
        }

        void SendMsgAll<T>(ServerMessageType type, T msg) where T : class
        {
            var serialized = JsonSerializer.Serialize(msg, serializeOptions);
            foreach (var item in players.Keys)
            {
                var serverMsg = new NetMQMessage();
                serverMsg.Append(item);
                serverMsg.AppendEmptyFrame();
                serverMsg.Append((int)type);
                serverMsg.Append(serialized);
                server.SendMultipartMessage(serverMsg);
            }
        }
    }
}
