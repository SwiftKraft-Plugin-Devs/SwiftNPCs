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
        public float MinAttackDistance = 0.2f;

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
            if (!Parent.HasEnemyTarget || !Roles.Contains(Parent.Role))
                return;

            bool hasLOS = Parent.HasLOS(Parent.EnemyTarget, out Vector3 pos, out bool hasCollider);

            if (hasLOS)
                Parent.MovementEngine.LookPos = pos;

            if (!hasLOS || hasCollider || Attacker == null || !CanAttack())
            {
                Pathfinder.OverrideWishDir = Vector3.zero;
                Pathfinder.SetDestination(Parent.EnemyTarget.Position);
            }
            else
            {
                if (!Parent.WithinDistance(Parent.EnemyTarget, MinAttackDistance))
                    Pathfinder.OverrideWishDir = (Parent.EnemyTarget.Position - Parent.Position).normalized;
                Attack();
            }
        }

        public virtual void Attack()
        {
            Log.Info("Attacking1!");
            NetworkWriter writer = new();
            Attacker.ClientWriteCmd(writer);
            Attacker.ServerProcessCmd(new(writer));

        }

        public void SendSubroutineMessage()
        {
            UpdateAttackTriggered(false);
            UpdateClients();
            UpdateAttackTriggered(true);
            UpdateClients();
            Timing.CallDelayed(0.2f, UpdateAttackTriggeredFalse);
        }

        public void UpdateAttackTriggered(bool value)
        {
            Attacker.SetBaseProperty("AttackTriggered", value);
        }

        private void UpdateAttackTriggeredFalse()
        {
            Attacker.SetBaseProperty("AttackTriggered", false);
        }

        private void UpdateAttackTriggeredTrue()
        {
            Attacker.SetBaseProperty("AttackTriggered", false);
        }

        public void UpdateClients()
        {
            NetworkClient.Send(new SubroutineMessage(Attacker, false));
        }

        public abstract bool CanAttack();
    }
}
