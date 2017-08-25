using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Serilog;

namespace GameConsole
{
    static class Program
    {
        private static ActorSystem _movieStreamingActorSystem;

        static void Main()
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

            Log.Information("Creating GameConsoleActorSystem");
            _movieStreamingActorSystem = ActorSystem.Create("GameConsoleActorSystem", config);

            Pause();
            _movieStreamingActorSystem.Terminate().Wait();

        }

        private static void Pause(string message = null)
        {
            const string pauseMessage = "Paused - Press any key";
            Console.WriteLine(message ?? pauseMessage);
            Console.ReadKey(true);
        }
    }
}
