using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using UnityEngine;

namespace SwiftNPCs.Core.World.Modules
{
    public class AIFollow : AIModuleBase
    {
        public float FollowDistance = 4f;
        public float FollowRandomRange = 2f;
        public float SprintDistance = 7f;

        AIPathfinder Pathfinder;

        public Player Target
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
        }

        public override void Tick()
        {
            if (!Enabled || !HasTarget || Parent.GetDistance(Target) < FollowDistance)
                return;

            Pathfinder.LookAtWaypoint = Parent.HasLOS(Parent.FollowTarget, out _, out _);

            if (!Pathfinder.LookAtWaypoint)
                Parent.MovementEngine.LookPos = Target.Camera.position;

            Pathfinder.SetDestination(Target.Position);
            if (DistanceToTarget > SprintDistance)
                Parent.MovementEngine.State = PlayerMovementState.Sprinting;
            else
                Parent.MovementEngine.State = TargetFpc.CurrentMovementState;
        }

        public override void OnEnabled() { }

        public bool HasTarget => Parent.HasFollowTarget;

        public float DistanceToTarget
        {
            get
            {
                if (HasTarget)
                    return Vector3.Distance(Target.Position, Parent.Position);
                return 0f;
            }
        }

        public FirstPersonMovementModule TargetFpc
        {
            get
            {
                if (!HasTarget)
                    return null;

                if (Target.RoleBase is IFpcRole fpc)
                    return fpc.FpcModule;

                return null;
            }
        }
    }
}
