using System;
using Akka.Actor;
using Akka.Persistence;
using GameConsole.Messages;
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
            Command<CreatePlayerMessage>(msg =>
            {
                Log.Information("Received CreatePlayerMessage for {PlayerName}", msg.PlayerName);


                Persist(msg, createPlayerMessage =>
                {
                    Log.Information("Persisted a CreatePlayerMessage for {PlayerName}", createPlayerMessage.PlayerName);

                    Context.ActorOf(Props.Create(() => new PlayerActor(createPlayerMessage.PlayerName, DefaultStartingHealth)),
                        createPlayerMessage.PlayerName);
                });
            });

            Recover<CreatePlayerMessage>(createPlayerMessage =>
            {
                Log.Information("Replaying CreatePlayerMessage for {PlayerName}", createPlayerMessage.PlayerName);

                Context.ActorOf(Props.Create(() => new PlayerActor(createPlayerMessage.PlayerName, DefaultStartingHealth)),
                    createPlayerMessage.PlayerName);
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