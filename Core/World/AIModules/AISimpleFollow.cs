using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AISimpleFollow : AIModuleBase
    {
        public float FollowDistance = 5f;
        public float FollowRandomRange = 2f;
        public float SprintDistance = 7f;

        public Vector3 Position => Parent.ReferenceHub.transform.position;

        public Player Target
        {
            get => Parent.FollowTarget;
            set => Parent.FollowTarget = value;
        }

        public override void Init()
        {
            Tags = [AIBehaviorBase.MoverTag];
            FollowDistance -= Random.Range(0f, FollowRandomRange);
        }

        public override void OnDisabled()
        {
            Parent.MovementEngine.WishDir = Vector3.zero;
            Parent.MovementEngine.State = PlayerMovementState.Walking;
        }

        public override void OnEnabled() { }

        public override void Tick()
        {
            if (Enabled && HasTarget && Parent.GetDistance(Target) >= FollowDistance)
            {
                Parent.MovementEngine.WishDir = GetMoveDirection();
                Parent.MovementEngine.LookPos = Target.Camera.position;
                if (DistanceToTarget > SprintDistance)
                    Parent.MovementEngine.State = PlayerMovementState.Sprinting;
                else
                    Parent.MovementEngine.State = TargetFpc.CurrentMovementState;
            }
            else
                Parent.MovementEngine.WishDir = Vector3.zero;
        }

        public virtual Vector3 GetMoveDirection()
        {
            if (!HasTarget)
                return Vector3.zero;

            return (Target.Position - Position).normalized;
        }

        public bool HasTarget => Parent.HasFollowTarget;

        public float DistanceToTarget
        {
            get
            {
                if (HasTarget)
                    return Vector3.Distance(Target.Position, Position);
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
