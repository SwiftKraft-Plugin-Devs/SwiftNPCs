using Interactables.Interobjects;
using MapGeneration;
using UnityEngine;
using UnityEngine.AI;
using FacilityZone = MapGeneration.FacilityZone;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIPathfinder : AIFollowPath
    {
        public static ElevatorChamber[] Elevators => Object.FindObjectsOfType<ElevatorChamber>();

        public Vector3 TargetLocation { get; private set; }

        public Vector3 NavPosition => NavMesh.SamplePosition(Position, out NavMeshHit _hit, 50f, NavMesh.AllAreas) ? _hit.position : default;

        public float RepathTimer = 0.2f;
        public float DestinationRadius = 3f;

        public bool AtDestination => Path == null || Vector3.Distance(TargetLocation, Position) <= DestinationRadius;

        private float timer;

        public override void Init()
        {
            Tags = [AIBehaviorBase.MoverTag];
        }

        public void SetDestination(Vector3 destination)
        {
            TargetLocation = destination;
        }

        public RoomIdentifier GetRoomAtPosition(Vector3 pos) => RoomIdUtils.RoomAtPosition(pos);

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
