using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using Microsoft.Extensions.Configuration;
using ShinyShoe.Logging;
using SimpleInjector;
using TrainworksReloaded.Base;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.CardUpgrade;
using TrainworksReloaded.Base.Character;
using TrainworksReloaded.Base.Class;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Base.Prefab;
using TrainworksReloaded.Base.Trait;
using TrainworksReloaded.Base.Trigger;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using Stoker.Plugin.Console;
using Stoker.Base.Impl;
using Stoker.Base.Commands;
using Stoker.Base;

namespace Stoker.Plugin
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new(MyPluginInfo.PLUGIN_GUID);
        public void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            var rootCommandExecutor = new RootCommandExecutor();
            rootCommandExecutor.TryAddCommand(HelpCommandFactory.Create(rootCommandExecutor));
            rootCommandExecutor.TryAddCommand(CardCommandFactory.Create());
            rootCommandExecutor.TryAddCommand(EchoCommandFactory.Create());
            rootCommandExecutor.TryAddCommand(GoldCommandFactory.Create());
            rootCommandExecutor.TryAddCommand(RelicCommandFactory.Create());
            rootCommandExecutor.TryAddCommand(HandCommandFactory.Create());
            rootCommandExecutor.TryAddCommand(PyreCommandFactory.Create());

            Railend.ConfigurePreAction(c =>
            {
                c.RegisterInstance(rootCommandExecutor);
                c.RegisterInstance(new ConsoleLogger(Logger));
            });

            var config = Config.Bind("Console", "Enabled", true, "Enable the console");
            if (config.Value)
            {
                // Create console
                CreateConsole(rootCommandExecutor);
            }
        }

        private void CreateConsole(RootCommandExecutor rootCommandExecutor)
        {
            // Create the console GameObject
            var consoleObject = new GameObject("StokerConsole");
            var stokerConsole = consoleObject.AddComponent<StokerConsole>();
            stokerConsole.CommandExecutor = rootCommandExecutor;
            DontDestroyOnLoad(consoleObject);
        }
    }
}
