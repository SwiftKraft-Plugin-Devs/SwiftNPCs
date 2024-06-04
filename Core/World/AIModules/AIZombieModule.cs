using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PluginAPI.Roles;
using System;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIZombieModule : AIRoleModuleBase
    {
        public ZombieRole ZombieRole
        {
            get
            {
                if (zombie == null && Parent.RoleBase is ZombieRole z)
                    zombie = z;
                return zombie;
            }
        }

        public ZombieAttackAbility Attacker
        {
            get
            {
                if (ZombieRole == null || !ZombieRole.SubroutineModule.TryGetSubroutine(out ZombieAttackAbility ability))
                    return null;

                return ability;
            }
        }

        ZombieRole zombie;

        AIPathfinder Pathfinder;

        public override RoleTypeId[] Roles => [RoleTypeId.Scp0492];

        public override void Init()
        {
            base.Init();
            Tags = [AIBehaviorBase.AttackerTag];
            Pathfinder = Parent.GetModule<AIPathfinder>();
        }

        public override void OnDisabled() { }

        public override void OnEnabled() { }

        public override void Tick()
        {
            if (!Parent.HasEnemyTarget || Attacker == null)
                return;

            bool hasLOS = Parent.HasLOS(Parent.EnemyTarget, out Vector3 pos, out bool hasCollider);

            if (hasLOS)
                Parent.MovementEngine.LookPos = pos;

            if (hasLOS && !hasCollider && Parent.GetDistance(Parent.EnemyTarget) <= Attacker._detectionRadius * 2f && Attacker.CanTriggerAbility && Attacker.Cooldown.IsReady)
            {
                Attacker.ServerPerformAttack();
                Attacker.Cooldown.Trigger(Attacker.BaseCooldown);
            }
        }
    }
}
