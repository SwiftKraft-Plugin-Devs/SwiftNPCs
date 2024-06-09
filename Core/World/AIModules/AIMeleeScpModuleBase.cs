using MEC;
using Mirror;
using PlayerRoles.PlayableScps;
using PlayerRoles.Subroutines;
using PluginAPI.Core;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public abstract class AIMeleeScpModuleBase<TRole, TAttacker> : AIRoleModuleBase where TRole : FpcStandardScp where TAttacker : KeySubroutine<TRole>
    {
        public float TryAttackRange = 2f;
        public float MinAttackDistance = 0.3f;

        public TRole RoleBase
        {
            get
            {
                if (roleBase == null && Parent.RoleBase is TRole t)
                    roleBase = t;
                return roleBase;
            }
        }

        public TAttacker Attacker
        {
            get
            {
                if (RoleBase == null || RoleBase is not ISubroutinedRole role || !role.SubroutineModule.TryGetSubroutine(out TAttacker ability))
                    return null;

                return ability;
            }
        }

        protected AIPathfinder Pathfinder { get; private set; }

        private TRole roleBase;

        public override void Init()
        {
            base.Init();
            Tags = [AIBehaviorBase.AttackerTag];
            Pathfinder = Parent.GetModule<AIPathfinder>();
        }

        public override void OnDisabled()
        {
            Pathfinder.OverrideWishDir = Vector3.zero;
        }

        public override void OnEnabled() { }

        public override void Tick()
        {
            if (Parent.EnemyTarget == null || !Parent.HasEnemyTarget || !Roles.Contains(Parent.Role))
                return;

            bool hasLOS = Parent.HasLOS(Parent.EnemyTarget, out Vector3 pos, out bool hasCollider);

            if (hasLOS)
                Parent.MovementEngine.LookPos = pos;

            if (!Parent.WithinDistance(Parent.EnemyTarget, MinAttackDistance))
                Pathfinder.OverrideWishDir = (Parent.EnemyTarget.GetPosition(Parent) - Parent.Position).normalized;
            else
                Pathfinder.OverrideWishDir = Vector3.zero;

            if (!hasLOS || hasCollider || Attacker == null || !CanAttack())
            {
                Pathfinder.OverrideWishDir = Vector3.zero;
                Pathfinder.SetDestination(Parent.EnemyTarget.GetPosition(Parent));
            }
            else
                Attack();
        }

        public virtual void Attack()
        {
            NetworkWriter writer = new();
            Attacker.ClientWriteCmd(writer);
            Attacker.ServerProcessCmd(new(writer));
        }

        public abstract bool CanAttack();
    }
}
