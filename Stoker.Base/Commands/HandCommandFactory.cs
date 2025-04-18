using HarmonyLib;
using Stoker.Base.Builder;
using Stoker.Base.Extension;
using Stoker.Base.Interfaces;

namespace Stoker.Base.Commands;

public static class HandCommandFactory
{
    public static ICommand Create(){
        var command = new CommandBuilder("hand")
            .WithDescription("Manage the hand")
            .WithSubCommand("draw")
                .WithDescription("Draw cards")
                .WithArgument<int>("amount")
                    .WithDescription("The amount of cards to draw")
                    .WithSuggestions(() => [.. new[] { "1", "2", "3" }])
                    .WithDefaultValue("1")
                    .WithParser((xs) => int.Parse(xs))
                    .Parent()
                .SetHandler((args) => {
                    var arguments = args.Arguments;
                    if (!arguments.ContainsKey("amount"))
                        throw new Exception("Missing <amount> argument");
                    if (arguments["amount"] is not int amount)
                        throw new Exception("Invalid <amount> argument");
                    if (amount <= 0)
                        throw new Exception("Invalid <amount> argument. Must be greater than 0");
                    AccessTools.Method(typeof(CheatManager), "Command_DrawCards").Invoke(null, [amount.ToString()]);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .WithSubCommand("discard")
                .WithDescription("Discard a card")
                .WithArgument<int>("index")
                    .WithDescription("The index of the card to discard")
                    .WithSuggestions(() => [.. new[] { "0", "1", "2", "3" }])
                    .WithDefaultValue("0")
                    .WithParser((xs) => int.Parse(xs))
                    .Parent()
                .SetHandler((args) => {
                    var arguments = args.Arguments;
                    if (!arguments.ContainsKey("index"))
                        throw new Exception("Missing <index> argument");
                    if (arguments["index"] is not int index)
                        throw new Exception("Invalid <index> argument");

                    AccessTools.Method(typeof(CheatManager), "Command_Discard").Invoke(null, [index.ToString()]);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .WithSubCommand("discard-all")
                .WithDescription("Discard all cards")
                .SetHandler((args) => {
                    AccessTools.Method(typeof(CheatManager), "Command_DiscardAll").Invoke(null, []);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .UseHelpMiddleware();
        return command.Build();
    }
    
}
