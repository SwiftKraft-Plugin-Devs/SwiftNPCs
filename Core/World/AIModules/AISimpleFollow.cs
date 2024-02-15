using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AISimpleFollow : AIModuleBase
    {
        public float SprintDistance = 7f;

        public Vector3 Position => Parent.ReferenceHub.transform.position;

        public Player Target
        {
            get => Parent.FollowTarget;
            set => Parent.FollowTarget = value;
        }

        public override void End(AIModuleBase next)
        {
            Parent.MovementEngine.WishDir = Vector3.zero;
            Parent.MovementEngine.State = PlayerMovementState.Walking;
        }

        public override void Init() { }

        public override void Start(AIModuleBase prev) { }

        public override void Tick()
        {
            if (HasTarget)
            {
                Parent.MovementEngine.WishDir = GetMoveDirection();
                Parent.MovementEngine.LookPos = Target.Camera.position;
                if (DistanceToTarget > SprintDistance)
                    Parent.MovementEngine.State = PlayerMovementState.Sprinting;
                else
                    Parent.MovementEngine.State = TargetFpc.CurrentMovementState;
            }

            CheckTransitions();
        }

        public virtual Vector3 GetMoveDirection()
        {
            if (!HasTarget)
                return Vector3.zero;

            return (Target.Position - Position).normalized;
        }

        public override void ReceiveData<T>(T data)
        {
            if (data is not Player p)
                return;

            Target = p;
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
