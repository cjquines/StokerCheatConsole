using HarmonyLib;
using Stoker.Base.Builder;
using Stoker.Base.Extension;
using Stoker.Base.Interfaces;
using System.Reflection;
using TrainworksReloaded.Core;

namespace Stoker.Base.Commands
{
    public class UpgradeCommandFactory
    {
        private static Lazy<ConsoleLogger> LoggerLazy { get; set; } = new(() => Railend.GetContainer().GetInstance<ConsoleLogger>());

        public static ICommand Create()
        {
            var command = new CommandBuilder("upgrade")
                .WithDescription("Manage ugprades")
                .WithSubCommand("card")
                .WithDescription("Apply an upgrade to a card")
                    .WithSimpleNameArg()
                        .WithDescription("The name of the upgrade to apply")
                        .WithSuggestions(() =>
                        {
                            Type type = typeof(CheatManager);
                            FieldInfo field = type.GetField("allGameData", BindingFlags.NonPublic | BindingFlags.Static);

                            if (field != null)
                            {
                                AllGameData? allGameData = field.GetValue(null) as AllGameData;
                                if (allGameData != null)
                                {
                                    return [.. allGameData.GetAllEnhancerData().Select(e => e.Cheat_GetNameEnglish())];
                                }
                            }
                            return [];
                        })
                        .WithParser((xs) => xs)
                        .Parent()
                    .SetHandler((args) =>
                    {
                        var arguments = args.Arguments;
                        if (!arguments.ContainsKey("name"))
                            throw new Exception("Missing <name> argument");
                        if (arguments["name"] is not string upgradeName)
                            throw new Exception("Invalid <name> argument");
                        if (string.IsNullOrEmpty(upgradeName))
                            throw new Exception("Empty <name> argument");
                        LoggerLazy.Value.Log($"Applying upgrade: {upgradeName}");
                        AccessTools.Method(typeof(CheatManager), "Command_UpgradeCard").Invoke(null, [upgradeName]);
                        return Task.CompletedTask;
                    })
                    .UseHelpMiddleware()
                    .Parent()
                .WithSubCommand("list")
                    .WithDescription("List all upgrades")
                    .SetHandler((args) =>
                    {
                        Type type = typeof(CheatManager);
                        FieldInfo field = type.GetField("allGameData", BindingFlags.NonPublic | BindingFlags.Static);

                        if (field != null)
                        {
                            AllGameData? allGameData = field.GetValue(null) as AllGameData;
                            if (allGameData != null)
                            {
                                List<String> names = [.. allGameData.GetAllEnhancerData().Select(s => s.Cheat_GetNameEnglish() + " : " + s.name)];
                                names.Sort();
                                names.ForEach(s => LoggerLazy.Value.Log(s));
                            }
                        }
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
