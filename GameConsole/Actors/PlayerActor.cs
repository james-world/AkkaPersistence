using System;
using Akka.Persistence;
using GameConsole.Commands;
using GameConsole.Events;
using Serilog;

namespace GameConsole.Actors
{
    public class PlayerActor : ReceivePersistentActor
    {
        private new static readonly ILogger Log = Serilog.Log.ForContext<PlayerActor>();

        private PlayerActorState _state;
        private int _eventCount;

        public override string PersistenceId => $"player-{_state.PlayerName}";

        public PlayerActor(string playerName, int health)
        {
            _state = new PlayerActorState
            {
                PlayerName = playerName,
                Health = health
            };

            Log.Information("{PlayerName} created", _state.PlayerName);

            Command<HitPlayer>(msg => HitPlayer(msg));
            Command<DisplayStatus>(_ => DisplayPlayerStatus());
            Command<SimulateError>(_ => SimulateError());

            Recover<PlayerHit>(playerHitEvent =>
            {
                Log.Information("{PlayerName} replaying {EventType} for {Damage} damage from journal", _state.PlayerName, nameof(PlayerHit), playerHitEvent.DamageTaken);
                _state.Health -= playerHitEvent.DamageTaken;
            });

            Recover<SnapshotOffer>(offer =>
            {
                Log.Information("{PlayerName} received SnapshotOffer from snapshot store, updating state", _state.PlayerName);
                _state = (PlayerActorState) offer.Snapshot;
                Log.Information("{PlayerName} state {State} set from snapshot", _state.PlayerName, _state);
            });
        }

        private void SimulateError()
        {
            Log.Information("{PlayerName} received {Command}", _state.PlayerName, nameof(Commands.SimulateError));
            throw new ApplicationException($"Simulated exception in player: {_state.PlayerName}");
        }

        private void DisplayPlayerStatus()
        {
            Log.Information("{PlayerName} has {Health} health", _state.PlayerName, _state.Health);
        }

        private void HitPlayer(HitPlayer command)
        {
            Log.Information("{PlayerName} received {Command} for {Damage} damage", _state.PlayerName, nameof(Commands.HitPlayer), command.Damage);

            var @event = new PlayerHit(command.Damage);

            Log.Information("{PlayerName} persisting {Event}", _state.PlayerName, nameof(PlayerHit));

            Persist(@event, playerHitEvent =>
            {
                Log.Information("{PlayerName} persisted {Event} ok, updating actor state", _state.PlayerName, nameof(PlayerHit));
                _state.Health -= @event.DamageTaken;

                _eventCount++;
                if (_eventCount == 5)
                {
                    Log.Information("{PlayerName} saving snapshot", _state.PlayerName);
                    SaveSnapshot(_state);
                    Log.Information("{PlayerName} resetting event count to 0", _state.PlayerName);
                    _eventCount = 0;
                }
            });
        }

        protected override void PreStart()
        {
            Log.Information("{PlayerName} PreStart", _state.PlayerName);
        }

        protected override void PostStop()
        {
            Log.Information("{PlayerName} PostStop", _state.PlayerName);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            if (message != null)
            {
                Log.Warning(
                    "PreRestart: Message of type {MessageType} caused exception with message {ExceptionMessage}",
                    message.GetType(),
                    reason.Message);
            }
            else
            {
                Log.Warning("PreRestart: {Reason}", reason.Message);
            }

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