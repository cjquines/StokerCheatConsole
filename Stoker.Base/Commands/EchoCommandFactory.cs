using Stoker.Base.Builder;
using Stoker.Base.Extension;
using Stoker.Base.Interfaces;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Interfaces;

namespace Stoker.Base.Commands
{
    public class EchoCommandFactory
    {
        private static Lazy<ConsoleLogger> LoggerLazy { get; set; } = new(() => Railend.GetContainer().GetInstance<ConsoleLogger>());
        public static ICommand Create()
        {
            var builder = new CommandBuilder("echo")
                .WithDescription("Echo a message")
                    .WithArgument<string>("message")
                    .WithDescription("The message to echo")
                    .WithParser((xs) => xs)
                    .Parent()
                .SetHandler((args) => {
                    if (!args.Arguments.ContainsKey("message"))
                        throw new Exception("Missing <message> argument");
                    var message = args.Arguments["message"];
                    if (message is null)
                        return Task.CompletedTask;
                    LoggerLazy.Value.Log(message.ToString());
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware();
            return builder.Build();
        }
    }
}