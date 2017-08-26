namespace GameConsole.Messages
{
    public class HitMessage
    {
        public HitMessage(int damage)
        {
            Damage = damage;
        }

        public int Damage { get; }
    }
}