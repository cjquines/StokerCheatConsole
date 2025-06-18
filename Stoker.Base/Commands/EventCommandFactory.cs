using HarmonyLib;
using Stoker.Base.Builder;
using Stoker.Base.Extension;
using Stoker.Base.Interfaces;
using System.Reflection;
using TrainworksReloaded.Core;

namespace Stoker.Base.Commands
{
    public class EventCommandFactory
    {
        private static Lazy<ConsoleLogger> LoggerLazy { get; set; } = new(() => Railend.GetContainer().GetInstance<ConsoleLogger>());

        public static ICommand Create()
        {
            var command = new CommandBuilder("event")
                .WithDescription("Manage events")
                .WithSubCommand("trigger")
                .WithDescription("Trigger an event")
                    .WithSimpleNameArg()
                        .WithDescription("The name of the event to trigger")
                        .WithSuggestions(() =>
                        {
                            Type type = typeof(CheatManager);
                            FieldInfo field = type.GetField("allGameData", BindingFlags.NonPublic | BindingFlags.Static);

                            if (field != null)
                            {
                                AllGameData? allGameData = field.GetValue(null) as AllGameData;
                                if (allGameData != null)
                                {
                                    return [.. allGameData.GetAllStoryEventData().Select(s => s.name)];
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
                        if (arguments["name"] is not string eventName)
                            throw new Exception("Invalid <name> argument");
                        if (string.IsNullOrEmpty(eventName))
                            throw new Exception("Empty <name> argument");
                        LoggerLazy.Value.Log($"Trigerring event: {eventName}");
                        AccessTools.Method(typeof(CheatManager), "Command_StartEvent").Invoke(null, [eventName]);
                        return Task.CompletedTask;
                    })
                    .UseHelpMiddleware()
                    .Parent()
                .WithSubCommand("list")
                    .WithDescription("List all events")
                    .SetHandler((args) =>
                    {
                        Type type = typeof(CheatManager);
                        FieldInfo field = type.GetField("allGameData", BindingFlags.NonPublic | BindingFlags.Static);

                        if (field != null)
                        {
                            AllGameData? allGameData = field.GetValue(null) as AllGameData;
                            if (allGameData != null)
                            {
                                List<String> names = [.. allGameData.GetAllStoryEventData().Select(s => s.name)];
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
