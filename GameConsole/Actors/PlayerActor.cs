using System;
using Akka.Actor;
using GameConsole.Messages;
using Serilog;

namespace GameConsole.Actors
{
    public class PlayerActor : ReceiveActor
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<PlayerActor>();

        private readonly string _playerName;
        private int _health;

        public PlayerActor(string playerName, int health)
        {
            _playerName = playerName;
            _health = health;

            Log.Information("{PlayerName} created", _playerName);

            Receive<HitMessage>(msg => HitPlayer(msg));
            Receive<DisplayStatusMessage>(_ => DisplayPlayerStatus());
            Receive<CauseErrorMessage>(_ => SimulateError());
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
            _health -= msg.Damage;
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