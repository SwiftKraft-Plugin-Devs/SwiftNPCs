using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using SwiftAPI.Utility.Spawners;
using SwiftNPCs.Core.World;
using SwiftNPCs.HarmonyPatching;

namespace SwiftNPCs
{
    public class Plugin
    {
        /// <summary>
        /// Singleton for the SwiftNPC API base plugin class. 
        /// This can be used anywhere to access the base class.
        /// </summary>
        public static Plugin Instance;

        private const string Author = "SwiftKraft";

        private const string Name = "SwiftNPCs";

        private const string Description = "An Open Source NPC API for NWAPI SCP: SL Servers. ";

        private const string Version = "v1.3";

        [PluginPriority(LoadPriority.High)]
        [PluginEntryPoint(Name, Version, Description, Author)]
        public void Init()
        {
            Instance = this;

            HarmonyPatcher.InitHarmony();

            EventManager.RegisterEvents<EventHandler>(this);

            SpawnerManager.RegisterSpawnerType<AISpawner>("NPC");

            Log.Info($"SwiftNPCs {Version} Loaded! ");
        }

        [PluginUnload]
        public void Deinit()
        {
            HarmonyPatcher.DeinitHarmony();
        }
    }
}
