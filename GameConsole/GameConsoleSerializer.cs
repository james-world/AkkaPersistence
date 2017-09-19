using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Akka.Actor;
using Akka.Serialization;
using GameConsole.Actors;
using MessagePack;

namespace GameConsole
{
    public class GameConsoleSerializer : Serializer
    {
        private static readonly MethodInfo Deserializer =
            typeof(MessagePackSerializer).GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static, null, CallingConventions.Any, new Type[] { typeof(byte[]) }, null );

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
