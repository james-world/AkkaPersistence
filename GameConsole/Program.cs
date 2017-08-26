using Akka.Actor;
using Akka.Configuration;
using GameConsole.Actors;
using GameConsole.Messages;
using Serilog;
using static System.Console;

namespace GameConsole
{
    static class Program
    {
        private static ActorSystem _movieStreamingActorSystem;
        private static IActorRef _playerCoordinator;

        private static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate:
                    "{Timestamp:HH:mm} [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}")
                .CreateLogger();

            var config = ConfigurationFactory.ParseString(@"
                akka {
                  loglevel=INFO,
                  loggers = [""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""],
                  actor {
                    debug {
                      receive = on
                      autoreceive = on
                      lifecycle = on
                      event-stream = on
                      unhandled = on
                    }       
                    serializers {
                      hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                    }
                    serialization-bindings {
                      ""System.Object"" = hyperion
                    }
                  }
                }
            ");

            Log.Information("Creating Game");
            _movieStreamingActorSystem = ActorSystem.Create("Game", config);
            _playerCoordinator = _movieStreamingActorSystem.ActorOf<PlayerCoordinatorActor>("PlayerCoordinator");

            DisplayInstructions();

            while (true)
            {
                var action = ReadLine();
                var playerName = action.Split(' ')[0];

                if (action.Contains("create"))
                {
                    CreatePlayer(playerName);
                }
                else if (action.Contains("hit"))
                {
                    var damage = int.Parse(action.Split(' ')[2]);
                    HitPlayer(playerName, damage);
                }
                else if (action.Contains("display"))
                {
                    DisplayPlayer(playerName);
                }
                else if (action.Contains("error"))
                {
                    ErrorPlayer(playerName);
                }
                else if (action.Contains("quit"))
                {
                    break;
                }
                else
                {
                    WriteLine("Unknown command");
                }
            }

            Log.Information("Terminating game");
            _movieStreamingActorSystem.Terminate().Wait();
        }

        private static void CreatePlayer(string playerName)
        {
            _playerCoordinator.Tell(new CreatePlayerMessage(playerName));
        }

        private static void DisplayPlayer(string playerName)
        {
            _movieStreamingActorSystem.ActorSelection($"/user/PlayerCoordinator/{playerName}")
                .Tell(new DisplayStatusMessage());
        }
        
        private static void HitPlayer(string playerName, int damage)
        {
            _movieStreamingActorSystem.ActorSelection($"/user/PlayerCoordinator/{playerName}")
                .Tell(new HitMessage(damage));
        }
        private static void ErrorPlayer(string playerName)
        {
            _movieStreamingActorSystem.ActorSelection($"/user/PlayerCoordinator/{playerName}")
                .Tell(new CauseErrorMessage());
        }

        private static void DisplayInstructions()
        {
            WriteLine("Available commands:");
            WriteLine("<playername> create");
            WriteLine("<playername> hit");
            WriteLine("<playername> display");
            WriteLine("<playername> error");
        }
    }
}
