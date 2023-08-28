using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Numerics;
using System.Transactions;

namespace GameServer
{
    class Vector3JsonConverter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            reader.Read();
            var x = reader.GetSingle();
            reader.Read();
            reader.Read();
            var y = reader.GetSingle();
            reader.Read();
            reader.Read();
            var z = reader.GetSingle();
            reader.Read();
            return new Vector3(x, y, z);
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("x", value.X);
            writer.WriteNumber("y", value.Y);
            writer.WriteNumber("z", value.Z);
            writer.WriteEndObject();
        }
    }
    public class Character
    {
        static int nextId = 0;

        [JsonInclude]
        public int id;
        [JsonInclude]
        public Vector3 position;
        [JsonInclude]
        public float aspd;
        [JsonInclude]
        public float speed;
        [JsonInclude]
        public float attackDelay;
        [JsonInclude]
        public Vector3 targetPosition;
        [JsonInclude]
        public Character targetEnemy;
        [JsonInclude]
        public Quaternion rotation;
        [JsonInclude]
        public float range;
        [JsonInclude]
        public float maxLife;
        [JsonInclude]
        public float life;
        [JsonInclude]
        public float attack;

        public Character() {
            id = nextId++;
        }
    }

    [Serializable]
    class World
    {
        [JsonInclude]
        public List<Character> entities;
    }
}
