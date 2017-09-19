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

        private readonly string _playerName;
        private int _health;

        public override string PersistenceId => $"player-{_playerName}";

        public PlayerActor(string playerName, int health)
        {
            _playerName = playerName;
            _health = health;

            Log.Information("{PlayerName} created", _playerName);

            Command<HitPlayer>(msg => HitPlayer(msg));
            Command<DisplayStatus>(_ => DisplayPlayerStatus());
            Command<SimulateError>(_ => SimulateError());

            Recover<PlayerHit>(playerHitEvent =>
            {
                Log.Information("{PlayerName} replaying {EventType} for {Damage} damage from journal", _playerName, nameof(PlayerHit), playerHitEvent.DamageTaken);
                _health -= playerHitEvent.DamageTaken;
            });
        }

        private void SimulateError()
        {
            Log.Information("{PlayerName} received {Command}", _playerName, nameof(Commands.SimulateError));
            throw new ApplicationException($"Simulated exception in player: {_playerName}");
        }

        private void DisplayPlayerStatus()
        {
            Log.Information("{PlayerName} has {Health} health", _playerName, _health);
        }

        private void HitPlayer(HitPlayer command)
        {
            Log.Information("{PlayerName} received {Command} for {Damage} damage", _playerName, nameof(Commands.HitPlayer), command.Damage);

            var @event = new PlayerHit(command.Damage);

            Log.Information("{PlayerName} persisting {Event}", _playerName, nameof(PlayerHit));

            Persist(@event, playerHitEvent =>
            {
                Log.Information("{PlayerName} persisted {Event} ok, updating actor state", _playerName, nameof(PlayerHit));
                _health -= @event.DamageTaken;
            });
        }

        protected override void PreStart()
        {
            Log.Information("{PlayerName} PreStart", _playerName);
        }

        protected override void PostStop()
        {
            Log.Information("{PlayerName} PostStop", _playerName);
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