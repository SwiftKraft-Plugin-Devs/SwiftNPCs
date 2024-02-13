using CustomPlayerEffects;
using PlayerRoles;
using PluginAPI.Core;

namespace SwiftNPCs.Core.World.AIConditions
{
    public class AIFindEnemyCondition : AIFindPlayerCondition
    {
        public override bool CanTarget(Player p) => p != null && p.IsAlive && !p.IsGodModeEnabled && !(p.EffectsManager.TryGetEffect(out Invisible inv) && inv.IsEnabled) && p.Role.GetFaction() != Runner.Role.GetFaction() && ParentModule.Parent.HasLOS(p, out _);
    }
}
