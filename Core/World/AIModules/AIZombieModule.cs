using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerRoles.Subroutines;
using PluginAPI.Core;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIZombieModule : AIMeleeScpModuleBase<ZombieRole, ZombieAttackAbility>
    {
        public override RoleTypeId[] Roles => [RoleTypeId.Scp0492];

        public override bool CanAttack() =>
            Parent.GetDistance(Parent.EnemyTarget) <= TryAttackRange;

        public override void Attack()
        {
            base.Attack();
            SendSubroutineMessage();
            base.Attack();
            UpdateAttackTriggered(false);
        }

        public void SendSubroutineMessage()
        {
            UpdateAttackTriggered(false);
            UpdateClients();
            UpdateAttackTriggered(true);
            UpdateClients();
        }

        public void UpdateAttackTriggered(bool value)
        {
            Attacker.SetBaseProperty("AttackTriggered", value);
        }

        public void UpdateClients()
        {
            NetworkClient.Send(new SubroutineMessage(Attacker, false));
        }
    }
}
