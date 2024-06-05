using PlayerRoles;
using PlayerRoles.PlayableScps.Scp106;
using System;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIScp106Module : AIMeleeScpModuleBase<Scp106Role, Scp106Attack>
    {
        public override RoleTypeId[] Roles => [RoleTypeId.Scp106];

        public override bool CanAttack() => 
            !Attacker.IsSubmerged
            && Parent.WithinDistance(Parent.EnemyTarget, Attacker._maxRangeSqr);
    }
}
