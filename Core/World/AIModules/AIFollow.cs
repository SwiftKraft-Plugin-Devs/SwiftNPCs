using CommandSystem.Commands.RemoteAdmin.Doors;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using PluginAPI.Core.Doors;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIFollow : AIPathfind
    {
        public float FollowDistance = 4f;
        public float FollowRandomRange = 2f;
        public float SprintDistance = 7f;

        public Player Target
        {
            get => Parent.FollowTarget;
            set => Parent.FollowTarget = value;
        }

        public override void Init()
        {
            base.Init();
            Tags = [AIBehaviorBase.MoverTag];
            FollowDistance -= Random.Range(0f, FollowRandomRange);
            LookAtWaypoint = false;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            Parent.MovementEngine.WishDir = Vector3.zero;
            Parent.MovementEngine.State = PlayerMovementState.Walking;
        }

        public override void Tick()
        {
            if (Enabled && HasTarget && Parent.GetDistance(Target) >= FollowDistance)
            {
                Parent.MovementEngine.LookPos = Target.Camera.position;
                SetDestination(Target.Position);
                if (DistanceToTarget > SprintDistance)
                    Parent.MovementEngine.State = PlayerMovementState.Sprinting;
                else
                    Parent.MovementEngine.State = TargetFpc.CurrentMovementState;
            }
            else
                ClearDestination();

            base.Tick();
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
