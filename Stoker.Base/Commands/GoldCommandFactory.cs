using HarmonyLib;
using Stoker.Base.Builder;
using Stoker.Base.Extension;
using Stoker.Base.Interfaces;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Interfaces;

namespace Stoker.Base.Commands;

public class GoldCommandFactory
{
    private static Lazy<ConsoleLogger> LoggerLazy { get; set; } = new(() => Railend.GetContainer().GetInstance<ConsoleLogger>());
    public static ICommand Create()
    {
        var command = new CommandBuilder("gold")
            .WithDescription("Manage gold")
            .WithArgument<string>("amount")
                .WithDescription("The amount of gold to add")
                .WithParser((xs) => xs)
                .WithDefaultValue("100")
                .WithSuggestions(() => [.. new string[] { "100", "+100", "-100"}])
                .Parent()
            .SetHandler((args) => {
                var arguments = args.Arguments;
                if (!arguments.ContainsKey("amount"))
                    throw new Exception("Missing <amount> argument");
                if (arguments["amount"] is not string amount)
                    throw new Exception("Invalid <amount> argument");
                if (string.IsNullOrEmpty(amount))
                    throw new Exception("Empty <amount> argument");
                LoggerLazy.Value.Log($"Adding {amount} gold to the player");
                AccessTools.Method(typeof(CheatManager), "Command_AdjustGold").Invoke(null, [amount]);
                return Task.CompletedTask;
            })
            .UseHelpMiddleware();
        return command.Build();
    }
}
