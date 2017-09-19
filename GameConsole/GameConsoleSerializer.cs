using System;
using Akka.Actor;
using Akka.Serialization;
using MessagePack;

namespace GameConsole
{
    public class GameConsoleSerializer : Serializer
    {
        public GameConsoleSerializer(ExtendedActorSystem system) : base(system)
        {
        }

        public override byte[] ToBinary(object obj)
        {
            return MessagePackSerializer.Typeless.Serialize(obj);
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            return MessagePackSerializer.Typeless.Deserialize(bytes);
        }

        public override bool IncludeManifest => false;

        public override int Identifier => 3007;

        
    }
}
