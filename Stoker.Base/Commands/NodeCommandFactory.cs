using HarmonyLib;
using Stoker.Base.Builder;
using Stoker.Base.Extension;
using Stoker.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TrainworksReloaded.Core;

namespace Stoker.Base.Commands
{
    public class NodeCommandFactory
    {
        private static Lazy<ConsoleLogger> LoggerLazy { get; set; } = new(() => Railend.GetContainer().GetInstance<ConsoleLogger>());

        public static ICommand Create()
        {
            var command = new CommandBuilder("node")
                .WithDescription("Manage nodes")
                .WithSubCommand("next")
                .WithDescription("Move to the next node")
                    .SetHandler((args) =>
                    {
                        LoggerLazy.Value.Log($"Moving to next node");
                        AccessTools.Method(typeof(CheatManager), "Command_NextNode").Invoke(null, []);
                        return Task.CompletedTask;
                    })
                    .UseHelpMiddleware()
                    .Parent()
                .WithSubCommand("prev")
                    .WithDescription("Move to the previous node")
                    .SetHandler((args) =>
                    {
                        LoggerLazy.Value.Log($"Moving to previous node");
                        AccessTools.Method(typeof(CheatManager), "Command_PreviousNode").Invoke(null, []);
                        return Task.CompletedTask;
                    })
                    .UseHelpMiddleware()
                    .Parent()
                .WithSubCommand("final")
                    .WithDescription("Move to the final node")
                    .SetHandler((args) =>
                    {
                        LoggerLazy.Value.Log($"Moving to final node");
                        AccessTools.Method(typeof(CheatManager), "Command_FinalNode").Invoke(null, []);
                        return Task.CompletedTask;
                    })
                    .UseHelpMiddleware()
                    .Parent()
                .WithSubCommand("tfb")
                    .WithDescription("Move to tfb")
                    .SetHandler((args) =>
                    {
                        LoggerLazy.Value.Log($"Moving to tfb");
                        AccessTools.Method(typeof(CheatManager), "Command_JumpToTFB").Invoke(null, []);
                        return Task.CompletedTask;
                    })
                    .UseHelpMiddleware()
                    .Parent()
               .WithSubCommand("reset")
                    .WithDescription("Reset node")
                    .SetHandler((args) =>
                    {
                        LoggerLazy.Value.Log($"Resetting node");
                        AccessTools.Method(typeof(CheatManager), "Command_ResetNode").Invoke(null, []);
                        return Task.CompletedTask;
                    })
                    .UseHelpMiddleware()
                    .Parent()
                .WithSubCommand("side")
                    .WithDescription("Change to the other side of the node")
                    .SetHandler((args) =>
                    {
                        LoggerLazy.Value.Log($"Changing to the other side of the node");
                        AccessTools.Method(typeof(CheatManager), "Command_ChangeSides").Invoke(null, []);
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
