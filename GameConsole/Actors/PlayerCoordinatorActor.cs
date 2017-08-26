using System;
using Akka.Actor;
using GameConsole.Messages;
using Serilog;

namespace GameConsole.Actors
{
    public class PlayerCoordinatorActor : ReceiveActor
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<PlayerCoordinatorActor>();
        private const int DefaultStartingHealth = 100;

        public PlayerCoordinatorActor()
        {
            Receive<CreatePlayerMessage>(msg =>
            {
                Log.Information("Received CreatePlayerMessage for {PlayerName}", msg.PlayerName);

                Context.ActorOf(Props.Create(() => new PlayerActor(msg.PlayerName, DefaultStartingHealth)),
                    msg.PlayerName);
            });
        }
        protected override void PreStart()
        {
            Log.Information("PreStart");
        }

        protected override void PostStop()
        {
            Log.Information("PostStop");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Log.Warning("PreRestart: Message of type {MessageType} caused exception with message {ExceptionMessage}",
                message.GetType(),
                reason.Message);

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            Log.Information("PostRestart: From exception with message {ExceptionMessage}",
                reason.Message);

            base.PostRestart(reason);
        }
    }
}