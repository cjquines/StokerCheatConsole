using Stoker.Plugin.Builder;

namespace Stoker.Plugin.Extension
{
    public static class BuilderExtensions
    {
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
            builder.UseHandlerMiddleware((handler) =>
            {
                if (handler == null)
                    return (args) =>
                    {
                        if (args.Options.ContainsKey("help") && args.Options["help"] is bool showHelp && showHelp)
                        {
                            System.Console.WriteLine(builder.GetCommandHelpString());
                        }
                        return Task.CompletedTask;
                    };
                else
                    return async (args) =>
                    {
                        if (args.Options.ContainsKey("help") && args.Options["help"] is bool showHelp && showHelp)
                        {
                            System.Console.WriteLine(builder.GetCommandHelpString());
                            return;
                        }
                        await handler(args);
                    };
            });
            return builder;
        }
    }
}
