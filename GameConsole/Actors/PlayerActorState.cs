using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace GameConsole.Actors
{
    [MessagePackObject]
    public class PlayerActorState
    {
        [Key(0)]
        public string PlayerName { get; set; }
        [Key(1)]
        public int Health { get; set; }
        public override string ToString()
        {
            return $"[PlayerActorState {PlayerName} {Health}]";
        }
    }
}
