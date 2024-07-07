using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using SwiftNPCs.Core.World.Targetables;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIFollow : AIModuleBase
    {
        public float FollowDistance = 3f;
        public float FollowRandomRange = 1.5f;
        public float SprintDistance = 7f;

        AIPathfinder Pathfinder;

        public TargetableBase Target
        {
            get => Parent.FollowTarget;
            set => Parent.FollowTarget = value;
        }

        public override void Init()
        {
            Pathfinder = Parent.GetModule<AIPathfinder>();
            Tags = [AIBehaviorBase.AutonomyTag];
            FollowDistance -= Random.Range(0f, FollowRandomRange);
        }

        public override void OnDisabled()
        {
            Pathfinder.LookAtWaypoint = true;
            Pathfinder.ClearDestination();
        }

        public override void Tick()
        {
            if (!Enabled || !HasTarget || Parent.WithinDistance(Target, FollowDistance))
                return;

            Pathfinder.LookAtWaypoint = Parent.HasLOS(Target, out _, out bool hasCollider) && !hasCollider;

            if (!Pathfinder.LookAtWaypoint)
                Parent.MovementEngine.LookPos = Target.GetHeadPosition(Parent);

            Pathfinder.SetDestination(Target.GetPosition(Parent));
            if (DistanceToTarget > SprintDistance)
                Parent.MovementEngine.State = PlayerMovementState.Sprinting;
            else if (TargetFpc != null)
                Parent.MovementEngine.State = TargetFpc.CurrentMovementState;
        }

        public override void OnEnabled() { }

        public bool HasTarget => Parent.HasFollowTarget;

        public float DistanceToTarget
        {
            get
            {
                if (HasTarget)
                    return Vector3.Distance(Target.GetPosition(Parent), Parent.Position);
                return 0f;
            }
        }

        public FirstPersonMovementModule TargetFpc
        {
            get
            {
                if (!HasTarget)
                    return null;

                if (Target is TargetablePlayer p && p.Player.RoleBase is IFpcRole fpc)
                    return fpc.FpcModule;

                return null;
            }
        }
    }
}
