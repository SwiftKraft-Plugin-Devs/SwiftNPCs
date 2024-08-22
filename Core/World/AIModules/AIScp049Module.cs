using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerRoles.Subroutines;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIScp049Module : AIMeleeScpModuleBase<Scp049Role, Scp049AttackAbility>
    {
        public Scp049ResurrectAbility Resurrect => GetSubroutine<Scp049ResurrectAbility>();

        public override RoleTypeId[] Roles => [RoleTypeId.Scp049];

        public override bool CanAttack() =>
            Parent.WithinDistance(Parent.EnemyTarget, Scp049AttackAbility.AttackDistance)
            && Attacker.Cooldown.IsReady;

        public override void Tick()
        {
            base.Tick();

            if (Parent.HasEnemyTarget)
                return;

            
        }
    }
}
