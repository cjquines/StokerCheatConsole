
using HarmonyLib;
using Stoker.Base.Builder;
using Stoker.Base.Extension;
using Stoker.Base.Interfaces;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Enum;
using TrainworksReloaded.Core.Interfaces;

namespace Stoker.Base.Commands;

public class RelicCommandFactory
{
    private static Lazy<ConsoleLogger> LoggerLazy { get; set; } = new(() => Railend.GetContainer().GetInstance<ConsoleLogger>());
    public static ICommand Create()
    {
        var command = new CommandBuilder("relic")
            .WithDescription("Manage relics")
            .WithSubCommand("add")
                .WithDescription("Add a relic to the deck")
                .WithArgument<string>("name")
                    .WithDescription("The name of the relic to add")
                    .WithSuggestions(() => [.. Railend.GetContainer().GetInstance<IRegister<RelicData>>().GetAllIdentifiers(RegisterIdentifierType.ReadableID).Select(c => c.ToString())])
                    .WithParser((xs) => xs)
                    .Parent()
                .SetHandler((args) => { 
                    var arguments = args.Arguments;
                    if (!arguments.ContainsKey("name"))
                        throw new Exception("Missing <name> argument");
                    if (arguments["name"] is not string relicName)
                        throw new Exception("Invalid <name> argument");
                    if (string.IsNullOrEmpty(relicName))
                        throw new Exception("Empty <name> argument");
                    LoggerLazy.Value.Log($"Adding relic: {relicName}");
                    AccessTools.Method(typeof(CheatManager), "Command_AddArtifact").Invoke(null, [relicName]);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .WithSubCommand("remove")
                .WithDescription("Remove a relic from the deck")
                .WithArgument<string>("name")
                    .WithDescription("The name of the relic to remove")
                    .WithSuggestions(() => [.. Railend.GetContainer().GetInstance<IRegister<RelicData>>().GetAllIdentifiers(RegisterIdentifierType.ReadableID).Select(c => c.ToString())])
                    .WithParser((xs) => xs)
                    .Parent()
                .SetHandler((args)   => {
                    var arguments = args.Arguments;
                    if (!arguments.ContainsKey("name"))
                        throw new Exception("Missing <name> argument");
                    if (arguments["name"] is not string relicName)
                        throw new Exception("Invalid <name> argument");
                    if (string.IsNullOrEmpty(relicName))
                        throw new Exception("Empty <name> argument");
                    LoggerLazy.Value.Log($"Removing relic: {relicName}");
                    AccessTools.Method(typeof(CheatManager), "Command_RemoveArtifact").Invoke(null, [relicName]);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .WithSubCommand("list")
                .WithDescription("List all relics in the deck")
                .WithOption<int>("page")
                    .WithDescription("The page number to list")
                    .WithDefaultValue("1")
                    .WithAliases("p")
                    .WithParser((xs) => int.Parse(xs))
                    .Parent()
                .WithOption<int>("page-size")
                    .WithDescription("The number of relics to list per page")
                    .WithDefaultValue("10")
                    .WithAliases("ps")
                    .WithParser((xs) => int.Parse(xs))
                    .Parent()
                .SetHandler((args) => {
                    var options = args.Options;
                    if (!options.ContainsKey("page"))
                        throw new Exception("Missing --page option");
                    if (!options.ContainsKey("page-size"))
                        throw new Exception("Missing --page-size option");
                    if (options["page"] is not int page)
                        throw new Exception("Invalid --page option");
                    if (options["page-size"] is not int pageSize)
                        throw new Exception("Invalid --page-size option");
                    var relics = Railend.GetContainer().GetInstance<IRegister<RelicData>>().GetAllIdentifiers(RegisterIdentifierType.ReadableID);
                    var startIndex = (page - 1) * pageSize;
                    var endIndex = startIndex + pageSize;
                    var pageRelics = relics.Skip(startIndex).Take(pageSize);
                    LoggerLazy.Value.Log("Relics:");
                    foreach (var relic in pageRelics)
                    {
                        LoggerLazy.Value.Log($"  {relic}");
                    }
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .UseHelpMiddleware();
        return command.Build();
    }
}