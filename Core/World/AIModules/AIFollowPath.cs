using PlayerRoles.FirstPersonControl;
using SwiftNPCs.Core.Pathing;
using UnityEngine;
using UserSettings;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIFollowPath : AIModuleBase
    {
        public AIMovementEngine MovementEngine => Parent.MovementEngine;

        public Vector3 Position => Parent.ReferenceHub.transform.position;

        public Path Path;

        public int CurrentIndex;

        public override void Init()
        {
            Tags = ["Movement"];
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
            if (!Enabled)
                return;

            if (Path == null)
            {
                MovementEngine.WishDir = Vector3.zero;
                return;
            }

            if (Path.TryGetWaypoint(CurrentIndex, out Vector3 waypoint))
            {
                MovementEngine.WishDir = GetDirection(waypoint);
                MovementEngine.LookPos = waypoint;

                if (!Path.TryGetDistance(Position, CurrentIndex, out float dist) || dist < Path.WaypointRadius)
                {
                    CurrentIndex++;
                    if (CurrentIndex >= Path.Waypoints.Count)
                        Path = null;
                }
            }
        }

        public Vector3 GetDirection(Vector3 waypoint) => (waypoint - Position).normalized;

        public void InitPath()
        {
            if (Path != null)
                CurrentIndex = Path.GetNearestIndex(Position);
        }
    }
}
