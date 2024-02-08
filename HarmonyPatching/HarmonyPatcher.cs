using HarmonyLib;
using PluginAPI.Core;
using System;

namespace SwiftNPCs.HarmonyPatching
{
    public static class HarmonyPatcher
    {
        public static Harmony Instance;

        public static void InitHarmony()
        {
            if (Instance == null)
                Instance = new Harmony("com.SwiftKraft.SwiftNPCs.API");

            try { Instance.PatchAll(); }
            catch (Exception e) { Log.Error("Harmony Patching Failed! \n" + e?.ToString()); }
        }

        public static void DeinitHarmony()
        {
            Instance?.UnpatchAll();
        }
    }
}
