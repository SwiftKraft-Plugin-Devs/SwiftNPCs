using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049.Zombies;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIZombieModule : AIMeleeScpModuleBase<ZombieRole, ZombieAttackAbility>
    {
        public override RoleTypeId[] Roles => [RoleTypeId.Scp0492];

        public override bool CanAttack() =>
            Parent.GetDistance(Parent.EnemyTarget) <= TryAttackRange
            && Attacker.Cooldown.IsReady;

        public override void Attack()
        {
            base.Attack();
            SendSubroutineMessage();
        }
    }
}
