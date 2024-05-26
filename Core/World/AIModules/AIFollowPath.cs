using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using PluginAPI.Core.Doors;
using SwiftAPI.API.BreakableToys;
using SwiftNPCs.Core.Pathing;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIFollowPath : AIModuleBase
    {
        public float DoorDistance = 1.5f;
        public float DoorFacing = 1.5f;

        public AIMovementEngine MovementEngine => Parent.MovementEngine;

        public Vector3 Position => Parent.ReferenceHub.transform.position;

        public Path Path;

        public int CurrentIndex;

        public bool LookAtWaypoint = true;
        public bool EnableMovement = true;

        public static bool DebugMode = false;

        public readonly List<BreakableToyBase> Markers = [];

        public override void Init()
        {
            Tags = [AIBehaviorBase.MoverTag];
            InitPath();
        }

        public override void OnDisabled()
        {
            MovementEngine.WishDir = Vector3.zero;
            MovementEngine.State = PlayerMovementState.Walking;
        }

        public override void OnEnabled() { }

        public override void Tick()
        {
            if (Path == null)
            {
                MovementEngine.WishDir = Vector3.zero;
                return;
            }

            if (Path.TryGetWaypoint(CurrentIndex, out Vector3 waypoint))
            {
                if (EnableMovement)
                    MovementEngine.WishDir = GetDirection(waypoint);

                if (LookAtWaypoint)
                {
                    Vector3 w = waypoint;
                    w.y = Parent.CameraPosition.y;
                    MovementEngine.LookPos = w;
                }

                if (TryGetDoor(out FacilityDoor door))
                {
                    Parent.TrySetDoor(door, true);
                    OnSetDoor(door);
                }

                if (!Path.TryGetDistance(Position, CurrentIndex, out float dist) || dist < Path.WaypointRadius)
                {
                    CurrentIndex++;
                    if (CurrentIndex >= Path.Waypoints.Count)
                        OnEndPath();
                }
            }
            else
                MovementEngine.WishDir = Vector3.zero;

            DebugUpdatePath();
        }

        public void DebugUpdatePath()
        {
            if (!DebugMode)
                return;

            foreach (BreakableToyBase toy in Markers)
                toy.Destroy();

            if (Path == null)
                return;

            foreach (Vector3 pos in Path.Waypoints)
                Markers.Add(BreakableToyManager.SpawnBreakableToy<BreakableToyBase>(null, PrimitiveType.Sphere, pos, Quaternion.identity, new(-0.1f, -0.1f, -0.1f), Color.red));
        }

        protected virtual void OnEndPath()
        {
            Path = null;
        }

        protected virtual void OnSetDoor(FacilityDoor door) { }

        public Vector3 GetDirection(Vector3 waypoint)
        {
            Vector3 pos1 = waypoint;
            pos1.y = Position.y;
            return (pos1 - Position).normalized;
        }

        public void InitPath()
        {
            if (Path != null)
                CurrentIndex = Path.GetNearestIndex(Position);
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

        public Vector3 GetDoorDirection(FacilityDoor door)
        {
            Vector3 pos = door.Position;
            pos.y = Parent.Position.y;
            return (pos - Parent.Position).normalized;
        }
    }
}
