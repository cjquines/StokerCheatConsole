using Stoker.Base.Builder;
using Stoker.Base.Extension;
using Stoker.Base.Impl;
using Stoker.Base.Interfaces;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Interfaces;

namespace Stoker.Base.Commands
{
    public class HelpCommandFactory
    {
        private static Lazy<ConsoleLogger> LoggerLazy { get; set; } = new(() => Railend.GetContainer().GetInstance<ConsoleLogger>());
        public static ICommand Create(RootCommandExecutor rootCommand)
        {
            var command = new CommandBuilder("help")
                .WithDescription("Display help information")
                .SetHandler((args) => {
                    //format the help string as:
                    // [commandName] - [commandDescription]
                    LoggerLazy.Value.Log("Available commands:");
                    var sortedCommands = rootCommand.Commands.OrderBy(c => c.Name).ToList();
                    foreach (var command in sortedCommands)
                    {
                        LoggerLazy.Value.Log($"  {command.Name, -12} - {command.Description}");
                    }
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware();
            return command.Build();
        }
    }
}
