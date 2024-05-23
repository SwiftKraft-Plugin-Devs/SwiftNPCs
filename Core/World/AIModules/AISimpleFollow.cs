using CommandSystem.Commands.RemoteAdmin.Doors;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using PluginAPI.Core.Doors;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AISimpleFollow : AIModuleBase
    {
        public float FollowDistance = 4f;
        public float FollowRandomRange = 2f;
        public float SprintDistance = 7f;
        public float DoorDistance = 1.5f;
        public float DoorFacing = 1.5f;

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
                Parent.MovementEngine.LookPos = Target.Camera.position;
                Parent.MovementEngine.WishDir = GetMoveDirection();
                if (DistanceToTarget > SprintDistance)
                    Parent.MovementEngine.State = PlayerMovementState.Sprinting;
                else
                    Parent.MovementEngine.State = TargetFpc.CurrentMovementState;

                if (TryGetDoor(out FacilityDoor door))
                    Parent.TrySetDoor(door, true);
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

        public FacilityDoor GetDoor()
        {
            FacilityDoor door = null;
            float doorDist = Mathf.Infinity;
            foreach (FacilityDoor d in Facility.Doors)
            {
                float dist = GetDoorDistance(d);
                if (dist <= DoorDistance && Parent.GetDotProduct(d.Position) > 0f && (door == null || dist < doorDist))
                {
                    door = d;
                    doorDist = dist;
                }
            }
            return door;
        }

        public bool TryGetDoor(out FacilityDoor door)
        {
            door = GetDoor();
            return door != null;
        }

        public float GetDoorDistance(FacilityDoor door) => Vector3.Distance(door.Position, Parent.Position);

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
