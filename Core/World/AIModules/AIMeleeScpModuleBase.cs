using Mirror;
using PlayerRoles.PlayableScps;
using PlayerRoles.Subroutines;
using System;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public abstract class AIMeleeScpModuleBase<TRole, TAttacker> : AIRoleModuleBase where TRole : FpcStandardScp where TAttacker : KeySubroutine<TRole>
    {
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

        public override void OnDisabled() { }

        public override void OnEnabled() { }

        public override void Tick()
        {
            if (!Parent.HasEnemyTarget)
                return;

            bool hasLOS = Parent.HasLOS(Parent.EnemyTarget, out Vector3 pos, out bool hasCollider);

            if (hasLOS)
                Parent.MovementEngine.LookPos = pos;

            if (!hasLOS || hasCollider || Attacker == null || !CanAttack())
                Pathfinder.SetDestination(Parent.EnemyTarget.Position);
            else
                Attack();
        }

        public virtual void Attack()
        {
            try { Attacker.OnKeyDown(); }
            catch (InvalidOperationException) { }
            NetworkWriter writer = new();
            Attacker.ClientWriteCmd(writer);
            Attacker.ServerProcessCmd(new(writer));
        }

        public abstract bool CanAttack();
    }
}
