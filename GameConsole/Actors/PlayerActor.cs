using System;
using Akka.Persistence;
using GameConsole.Messages;
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

            Command<HitMessage>(msg => HitPlayer(msg));
            Command<DisplayStatusMessage>(_ => DisplayPlayerStatus());
            Command<CauseErrorMessage>(_ => SimulateError());

            Recover<HitMessage>(msg =>
            {
                Log.Information("{PlayerName} replaying HitMessage for {Damage} damage from journal", _playerName, msg.Damage);
                _health -= msg.Damage;
            });
        }

        private void SimulateError()
        {
            Log.Information("{PlayerName} received CauseErrorMessage", _playerName);
            throw new ApplicationException($"Simulated exception in player: {_playerName}");
        }

        private void DisplayPlayerStatus()
        {
            Log.Information("{PlayerName} has {Health} health", _playerName, _health);
        }

        private void HitPlayer(HitMessage msg)
        {
            Log.Information("{PlayerName} received HitMessage for {Damage} damage", _playerName, msg.Damage);
            Log.Information("{PlayerName} persisting HitMessage", _playerName);

            Persist(msg, _ =>
            {
                Log.Information("{PlayerName} persisted HitMessage ok, updating actor state", _playerName);
                _health -= msg.Damage;
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