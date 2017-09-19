using System;
using Akka.Actor;
using Akka.Persistence;
using GameConsole.Commands;
using GameConsole.Events;
using Serilog;

namespace GameConsole.Actors
{
    public class PlayerCoordinatorActor : ReceivePersistentActor
    {
        private new static readonly ILogger Log = Serilog.Log.ForContext<PlayerCoordinatorActor>();
        private const int DefaultStartingHealth = 100;

        public override string PersistenceId => "player-coordinator";

        public PlayerCoordinatorActor()
        {
            Command<CreatePlayer>(command =>
            {
                Log.Information("Received {CommandType} for {PlayerName}", nameof(CreatePlayer), command.PlayerName);

                var @event = new PlayerCreated(command.PlayerName);

                Persist(@event, playerCreatedEvent =>
                {
                    Log.Information("Persisted a {EventType} for {PlayerName}", nameof(PlayerCreated), playerCreatedEvent.PlayerName);

                    Context.ActorOf(Props.Create(() => new PlayerActor(playerCreatedEvent.PlayerName, DefaultStartingHealth)),
                        playerCreatedEvent.PlayerName);
                });
            });

            Recover<PlayerCreated>(playerCreatedEvent =>
            {
                Log.Information("Replaying {EventType} for {PlayerName}", nameof(PlayerCreated), playerCreatedEvent.PlayerName);

                Context.ActorOf(Props.Create(() => new PlayerActor(playerCreatedEvent.PlayerName, DefaultStartingHealth)),
                    playerCreatedEvent.PlayerName);
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