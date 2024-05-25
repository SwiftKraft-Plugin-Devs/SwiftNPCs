using PlayerRoles.FirstPersonControl;
using PluginAPI.Core.Doors;
using SwiftNPCs.Core.Pathing;
using UnityEngine;
using UnityEngine.AI;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIPathfind : AIFollowPath
    {
        public Vector3 TargetLocation { get; private set; }

        public float RepathTimer = 0.2f;

        public bool AtDestination => Vector3.Distance(TargetLocation, Position) <= Path.WaypointRadius;

        private float timer;

        public override void Init()
        {
            base.Init();

            Path ??= new();
        }

        public void SetDestination(Vector3 destination)
        {
            TargetLocation = destination;
        }

        public void Pathfind()
        {
            Path ??= new();

            if (AtDestination)
                return;

            NavMeshPath path = new();
            NavMesh.CalculatePath(Position, TargetLocation, NavMesh.AllAreas, path);
            Path?.OverridePath(path, TargetLocation);
            CurrentIndex = 0;
        }

        public void ClearDestination()
        {
            Path = null;
            TargetLocation = Position;
        }

        public override void Tick()
        {
            if (timer > 0f)
                timer -= Time.fixedDeltaTime;
            else
            {
                timer = RepathTimer;
                Pathfind();
            }

            base.Tick();
        }
    }
}
