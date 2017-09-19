using MessagePack;

namespace GameConsole.Events
{
    [MessagePackObject]
    public class PlayerHit : IEvent
    {
        [Key(0)]
        public int DamageTaken { get; }

        public PlayerHit(int damageTaken)
        {
            DamageTaken = damageTaken;
        }
    }
}