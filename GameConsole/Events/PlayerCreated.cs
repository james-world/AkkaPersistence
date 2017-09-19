using MessagePack;

namespace GameConsole.Events
{
    [MessagePackObject]
    public class PlayerCreated : IEvent
    {
        [Key(0)]
        public string PlayerName { get; }

        public PlayerCreated(string playerName)
        {
            PlayerName = playerName;
        }
    }
}