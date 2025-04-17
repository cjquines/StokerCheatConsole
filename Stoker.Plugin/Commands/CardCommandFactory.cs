using HarmonyLib;
using Stoker.Plugin.Builder;
using Stoker.Plugin.Extension;
using Stoker.Plugin.Impl;
using Stoker.Plugin.Interfaces;
using TrainworksReloaded.Base;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Enum;
using TrainworksReloaded.Core.Interfaces;

namespace Stoker.Plugin.Commands
{
    public class AddCardCommandFactory 
    {
        public static ICommand Create()
        {
            var command = new CommandBuilder("card")
                .WithDescription("Manage cards")
                .WithSubCommand("add")
                    .WithDescription("Add a card to the deck")
                    .WithArgument("name")
                        .WithDescription("The name of the card to add")
                        .WithSuggestions(() => [.. Railend.GetContainer().GetInstance<IRegister<CardData>>().GetAllIdentifiers(RegisterIdentifierType.ReadableID).Select(c => c.ToString())])
                        .Parent()
                    .SetHandler((args) => {
                        var arguments = args.Arguments;
                        if (arguments.Count == 0)
                            return Task.CompletedTask;
                        if (arguments[0] is not string cardName)
                            return Task.CompletedTask;
                        if (string.IsNullOrEmpty(cardName))
                            return Task.CompletedTask;
                        CheatManager.Command_AddCard(cardName);
                        return Task.CompletedTask;
                    })
                    .UseHelpMiddleware()
                    .Parent()
                .WithSubCommand("remove")
                    .WithDescription("Remove a card from the deck")
                    .WithArgument("name")
                        .WithDescription("The name of the card to remove")
                        .WithSuggestions(() => [.. Railend.GetContainer().GetInstance<IRegister<CardData>>().GetAllIdentifiers(RegisterIdentifierType.ReadableID).Select(c => c.ToString())])
                        .Parent()
                    .SetHandler((args) => {
                        var arguments = args.Arguments;
                        if (arguments.Count == 0)
                            return Task.CompletedTask;
                        if (arguments[0] is not string cardName)
                            return Task.CompletedTask;
                        if (string.IsNullOrEmpty(cardName))
                            return Task.CompletedTask;
                        AccessTools.Method(typeof(CheatManager), "Command_RemoveCard").Invoke(null, [cardName]);
                        return Task.CompletedTask;
                    })
                    .UseHelpMiddleware()
                    .Parent()
                .UseHelpMiddleware()
                .Build();
            return command;
        }
    }
}
