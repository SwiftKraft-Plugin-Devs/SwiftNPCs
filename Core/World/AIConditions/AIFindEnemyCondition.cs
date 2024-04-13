using CustomPlayerEffects;
using PlayerRoles;
using PluginAPI.Core;
using SwiftAPI.API.ServerVariables;

namespace SwiftNPCs.Core.World.AIConditions
{
    public class AIFindEnemyCondition : AIFindPlayerCondition
    {
        public const string NoKOS = "npckos";

        public static bool DisableKOS => ServerVariableManager.TryGetVar(NoKOS, out ServerVariable svar) && bool.TryParse(svar.Value, out bool v) && !v;

        public override bool CanTarget(Player p) =>
            p != null
            && p.IsAlive
            && !p.IsDisarmed
            && !p.IsGodModeEnabled
            && !IsInvisible(p)
            && IsEnemy(p)
            && (!DisableKOS || !IsCivilian(p) || IsArmed(p))
            && ParentModule.Parent.HasLOS(p, out _);

        public bool IsCivilian(Player p) => p.Role == RoleTypeId.ClassD || p.Role == RoleTypeId.Scientist;

        public bool IsArmed(Player p) => p.CurrentItem != null && p.CurrentItem.Category == ItemCategory.Firearm;

        public bool IsInvisible(Player p) => p.EffectsManager.TryGetEffect(out Invisible inv) && inv.IsEnabled;

        public bool IsEnemy(Player p) => p.Role.GetFaction() != Runner.Role.GetFaction();
    }
}
