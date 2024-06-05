using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using System;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIZombieModule : AIMeleeScpModuleBase<ZombieRole, ZombieAttackAbility>
    {
        public override RoleTypeId[] Roles => [RoleTypeId.Scp0492];

        public override bool CanAttack() => 
            Parent.GetDistance(Parent.EnemyTarget) <= Attacker._detectionRadius * 2f
            && Attacker.CanTriggerAbility
            && Attacker.Cooldown.IsReady;
    }
}
