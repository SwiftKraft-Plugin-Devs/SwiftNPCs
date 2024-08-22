using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerRoles.Ragdolls;
using PlayerRoles.Subroutines;
using PluginAPI.Core;
using SwiftNPCs.Core.World.Targetables;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIZombieModule : AIMeleeScpModuleBase<ZombieRole, ZombieAttackAbility>
    {
        public ZombieConsumeAbility Consume => GetSubroutine<ZombieConsumeAbility>();

        public float BodyCheckRadius = 20f;
        public float BodyCheckTime = 3f;
        public float ConsumeRadius = 4f;

        public override RoleTypeId[] Roles => [RoleTypeId.Scp0492];

        BasicRagdoll curRagdoll;

        float _bodyCheckTime;
        float _consumeTime;

        bool Consuming => _consumeTime > 0f;

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
            if (Consuming)
            {
                _consumeTime -= Time.fixedDeltaTime;

                if (_consumeTime <= 0f && curRagdoll != null)
                {
                    curRagdoll = null;
                    Parent.Health.CurValue += ZombieConsumeAbility.ConsumeHeal;
                }

                return;
            }

            base.Tick();

            if (Parent.HasEnemyTarget || Parent.Health.CurValue > Parent.Health.MaxValue / 2f || Consume == null)
                return;

            if (Parent.FollowTarget is TargetableRagdoll ragdoll)
            {
                Log.Info("Follow target is ragdoll");
                if (!ZombieConsumeAbility.ConsumedRagdolls.Contains(ragdoll) && Vector3.Distance(Parent.Position, ragdoll.GetPosition(Parent)) <= ConsumeRadius)
                {
                    Log.Info("Consuming body");
                    curRagdoll = ragdoll;
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
            Log.Info("Checking bodies!");
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
