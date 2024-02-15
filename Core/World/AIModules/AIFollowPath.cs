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

        public override void End(AIModuleBase next)
        {
            MovementEngine.WishDir = Vector3.zero;
            MovementEngine.State = PlayerMovementState.Walking;
        }

        public override void Init()
        {
            InitPath();
        }

        public override void ReceiveData<T>(T data)
        {
            if (data is not Path p)
                return;

            Path = p;
            InitPath();
        }

        public override void Start(AIModuleBase prev) { }

        public override void Tick()
        {
            CheckTransitions();

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
