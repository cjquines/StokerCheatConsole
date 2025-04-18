using HarmonyLib;
using Stoker.Base.Builder;
using Stoker.Base.Extension;
using Stoker.Base.Interfaces;

namespace Stoker.Base.Commands;

public static class ToggleCommandFactory
{
    public static ICommand Create()
    {
        var command = new CommandBuilder("toggle")
            .WithDescription("Toggle a feature")
            .WithSubCommand("god-pyre")
                .WithDescription("Make the Pyre invincible")
                .SetHandler((args) =>
                {
                    AccessTools.Method(typeof(CheatManager), "Command_GodPyre").Invoke(null, []);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .WithSubCommand("god-enemy")
                .WithDescription("Make enemies invincible")
                .SetHandler((args) =>
                {
                    AccessTools.Method(typeof(CheatManager), "Command_GodEnemies").Invoke(null, []);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .WithSubCommand("god-monster")
                .WithDescription("Make monsters invincible")
                .SetHandler((args) =>
                {
                    AccessTools.Method(typeof(CheatManager), "Command_GodMonsters").Invoke(null, []);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .WithSubCommand("god-ember")
                .WithDescription("Give Unlimited Ember")
                .SetHandler((args) =>
                {
                    AccessTools.Method(typeof(CheatManager), "Command_GodEmber").Invoke(null, []);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .WithSubCommand("timing-display")
                .WithDescription("Toggle Timing Window")
                .SetHandler((args) =>
                {
                    AccessTools.Method(typeof(CheatManager), "Command_ToggleTimingDisplay").Invoke(null, []);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .WithSubCommand("unit-display")
                .WithDescription("Toggle Unit Display")
                .SetHandler((args) =>
                {
                    AccessTools.Method(typeof(CheatManager), "Command_ToggleUnitInfoDisplay").Invoke(null, []);
                    return Task.CompletedTask;
                })
                .UseHelpMiddleware()
                .Parent()
            .UseHelpMiddleware();
        return command.Build();
    }

}
