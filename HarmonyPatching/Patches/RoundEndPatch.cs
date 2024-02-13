using HarmonyLib;
using NorthwoodLib.Pools;
using SwiftNPCs.Core.World;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SwiftNPCs.HarmonyPatching.Patches
{
    [HarmonyPatch]
    public static class RoundEndPatch
    {
        public static IEnumerable<MethodInfo> TargetMethods() => new[]
{
            typeof(RoundSummary)
                .GetNestedType("<_ProcessServerSideCode>d__48", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .GetMethod("MoveNext", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        };

        public static bool Skip(ReferenceHub hub) => hub.TryGetComponent(out AIPlayer _);

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var list = ListPool<CodeInstruction>.Shared.Rent(instructions);
            var index = list.FindIndex(i => i.operand is MethodInfo { Name: "GetTeam" }) + 2;
            var label = (Label)list[index + 2].operand;
            list.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc, 10),
                CodeInstruction.Call(typeof(RoundEndPatch), nameof(Skip)),
                new CodeInstruction(OpCodes.Brtrue, label)
            });
            foreach (var codeInstruction in list)
                yield return codeInstruction;
            ListPool<CodeInstruction>.Shared.Return(list);
        }
    }
}
