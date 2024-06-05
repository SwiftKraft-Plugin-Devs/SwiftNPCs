using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;
using System;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIScp049Module : AIMeleeScpModuleBase<Scp049Role, Scp049AttackAbility>
    {
        public override RoleTypeId[] Roles => [RoleTypeId.Scp049];

        public override bool CanAttack() => 
            Parent.WithinDistance(Parent.EnemyTarget, Scp049AttackAbility.AttackDistance)
            && Attacker.Cooldown.IsReady;
    }
}
