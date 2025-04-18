using HarmonyLib;
using Stoker.Base.Builder;
using Stoker.Base.Extension;
using Stoker.Base.Interfaces;

namespace Stoker.Base.Commands;

public static class PyreCommandFactory
{
    public static ICommand Create()
    {
        var command = new CommandBuilder("pyre")
            .WithDescription("Do things to the Pyre")
            .WithSubCommand("health")
                .WithDescription("Adjust the Pyre's health")
                .WithArgument<string>("amount")
                    .WithDescription("The amount to adjust the Pyre's health")
                    .WithSuggestions(() => [.. new[] { "+50", "-50", "50" }])
                    .WithParser((xs) => xs)
                    .Parent()
                .SetHandler((args) =>
                {
                    var arguments = args.Arguments;
                    if (!arguments.ContainsKey("health"))
                        throw new Exception("Missing <health> argument");
                    if (arguments["health"] is not string health)
                        throw new Exception("Invalid <health> argument");
                    AccessTools.Method(typeof(CheatManager), "Command_AdjustTowerHP").Invoke(null, [health]);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .WithSubCommand("max-health")
                .WithDescription("Set the Pyre's max health")
                .WithArgument<string>("amount")
                    .WithDescription("The amount to set the Pyre's max health")
                    .WithSuggestions(() => [.. new[] { "50", "100", "200" }])
                    .WithParser((xs) => xs)
                    .Parent()
                .SetHandler((args) =>
                {
                    var arguments = args.Arguments;
                    if (!arguments.ContainsKey("amount"))
                        throw new Exception("Missing <amount> argument");
                    if (arguments["amount"] is not string amount)
                        throw new Exception("Invalid <amount> argument");
                    AccessTools.Method(typeof(CheatManager), "Command_AdjustTowerMaxHP").Invoke(null, [amount]);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .UseHelpMiddleware();
        return command.Build();
    }
}