using Stoker.Base.Builder;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Interfaces;

namespace Stoker.Base.Extension
{
    public static class BuilderExtensions
    {
        private static Lazy<ConsoleLogger> LoggerLazy { get; set; } = new(() => Railend.GetContainer().GetInstance<ConsoleLogger>());

        public static CommandBuilder AddHelpOptions(this CommandBuilder builder)
        {
            var rebuilder = builder
                .WithOption<bool>("help")
                    .WithDescription("Show help information")
                    .WithAliases("h")
                    .WithDefaultValue("")
                    .WithParser((value) => value != null)
                .Parent();
            return rebuilder;
        }

        public static CommandBuilder UseHelpMiddleware(this CommandBuilder builder)
        {
            builder
                .AddHelpOptions()
                .UseHandlerMiddleware((handler) =>
                {
                    if (handler == null)
                        return (args) =>
                        {
                            var containsHelp = args.Options.ContainsKey("help");
                            if (containsHelp && args.Options["help"] is bool showHelp && showHelp)
                            {
                                LoggerLazy.Value.Log(builder.GetCommandHelpString());
                            }
                            return Task.CompletedTask;
                        };
                    else
                        return async (args) =>
                        {
                            var containsHelp = args.Options.ContainsKey("help");
                            if (containsHelp && args.Options["help"] is bool showHelp && showHelp)
                            {
                                LoggerLazy.Value.Log(builder.GetCommandHelpString());
                                return;
                            }
                            await handler(args);
                        };
                });
            return builder;
        }
    }
}
