using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerRoles.Ragdolls;
using PlayerRoles.Subroutines;
using SwiftNPCs.Core.World.Targetables;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIZombieModule : AIMeleeScpModuleBase<ZombieRole, ZombieAttackAbility>
    {
        public ZombieConsumeAbility Consume => GetSubroutine<ZombieConsumeAbility>();

        public float BodyCheckRadius = 20f;
        public float BodyCheckTime = 3f;

        public override RoleTypeId[] Roles => [RoleTypeId.Scp0492];

        float _bodyCheckTime;
        float _consumeTime;

        bool _consuming => _consumeTime > 0f;

        public override bool CanAttack() => Parent.GetDistance(Parent.EnemyTarget) <= TryAttackRange;

        public override void Attack()
        {
            base.Attack();
            SendSubroutineMessage();
            base.Attack();
            UpdateAttackTriggered(false);
        }

        public override void Init()
        {
            base.Init();
            _bodyCheckTime = BodyCheckTime;
        }

        public override void Tick()
        {
            if (_consuming)
            {
                _consumeTime -= Time.fixedDeltaTime;
                return;
            }

            base.Tick();

            if (Parent.HasEnemyTarget || Parent.Health.CurValue <= Parent.Health.MaxValue / 2f || Consume == null)
                return;

            if (Parent.FollowTarget is TargetableRagdoll ragdoll)
            {
                if (!ZombieConsumeAbility.ConsumedRagdolls.Contains(ragdoll) && Consume.IsCloseEnough(Parent.Position, ragdoll.GetPosition(Parent)))
                {
                    Consume.CurRagdoll = ragdoll;
                    Consume.ServerSendRpc(true);
                    _consumeTime = Consume.Duration;
                    return;
                }
                else
                    Parent.FollowTarget = null;
            }

            _bodyCheckTime -= Time.fixedDeltaTime;

            if (_bodyCheckTime <= 0f)
            {
                _bodyCheckTime = BodyCheckTime;
                CheckBodies();
            }
        }

        public void CheckBodies()
        {
            BasicRagdoll[] ragdolls = Parent.Position.GetRagdolls(BodyCheckRadius);
            BasicRagdoll target = null;
            foreach (BasicRagdoll ragdoll in ragdolls)
            {
                if (!ZombieConsumeAbility.ConsumedRagdolls.Contains(ragdoll))
                {
                    target = ragdoll;
                    break;
                }
            }

            if (target == null)
                return;

            Parent.FollowTarget = target;
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
