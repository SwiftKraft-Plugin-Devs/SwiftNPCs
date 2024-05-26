using Interactables.Interobjects;
using MapGeneration;
using SwiftNPCs.Core.Pathing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FacilityZone = MapGeneration.FacilityZone;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIPathfind : AIFollowPath
    {
        public static ElevatorChamber[] Elevators => Object.FindObjectsOfType<ElevatorChamber>();

        public Vector3 TargetLocation { get; private set; }

        public Vector3 NavPosition => NavMesh.SamplePosition(Position, out NavMeshHit _hit, 50f, NavMesh.AllAreas) ? _hit.position : default;

        public float RepathTimer = 0.2f;

        public bool AtDestination => Path == null || Vector3.Distance(TargetLocation, Position) <= Path.WaypointRadius;

        private float timer;

        public override void Init()
        {
            base.Init();

            Path ??= new();
        }

        public void SetDestination(Vector3 destination)
        {
            TargetLocation = AdjustDestination(destination);
        }

        public Vector3 AdjustDestination(Vector3 dest)
        {
            RoomIdentifier targetRoom = GetRoomAtPosition(dest);

            if (targetRoom == null)
                return dest;

            if (targetRoom.Zone != Parent.Room.Zone)
                return ZoneSpecificAdjustment(dest, targetRoom.Zone);

            return dest;
        }

        protected virtual Vector3 ZoneSpecificAdjustment(Vector3 dest, FacilityZone zone)
        {
            if (Parent.Room.Zone == zone)
                return dest;

/*            switch (zone)
            {
                case FacilityZone.HeavyContainment:
                    switch (Parent.Room.Zone)
                    {
                        case FacilityZone.Entrance:
                            return dest;
                        case FacilityZone.LightContainment:
                            return LCZElevator();
                        case FacilityZone.Surface:
                            return SurfaceElevator();
                    }
                    break;
                case FacilityZone.Entrance:
                    switch (Parent.Room.Zone)
                    {
                        case FacilityZone.HeavyContainment:
                            return dest;
                        case FacilityZone.LightContainment:
                            return LCZElevator();
                        case FacilityZone.Surface:
                            return SurfaceElevator();
                    }
                    break;
                case FacilityZone.LightContainment:
                    switch (Parent.Room.Zone)
                    {
                        case FacilityZone.HeavyContainment: 
                        case FacilityZone.Entrance:
                            return HCZElevator();
                        case FacilityZone.Surface:
                            return SurfaceElevator();
                    }
                    break;
                case FacilityZone.Surface:
                    switch (Parent.Room.Zone)
                    {
                        case FacilityZone.HeavyContainment:
                        case FacilityZone.Entrance:
                            return EZElevator();
                        case FacilityZone.LightContainment:
                            return LCZElevator();
                    }
                    break;
            }*/

            return dest;
        }

        protected RoomIdentifier GetClosestElevator(FacilityZone zone)
        {
            RoomIdentifier closest = null;
            foreach (RoomIdentifier rid in Utilities.ZoneElevatorRooms[zone])
                if (closest == null || Vector3.Distance(closest.transform.position, Parent.Position) > Vector3.Distance(rid.transform.position, Parent.Position))
                    closest = rid;
            return closest;
        }

        protected bool NotInElevatorRoom(FacilityZone zone, out Vector3 dest)
        {
            if (!Utilities.ZoneElevatorRooms[FacilityZone.LightContainment].Contains(Parent.Room))
            {
                dest = GetClosestElevator(FacilityZone.LightContainment).transform.position;
                return true;
            }
            dest = default;
            return false;
        }

        protected virtual Vector3 LCZElevator()
        {
            if (NotInElevatorRoom(FacilityZone.LightContainment, out Vector3 dest))
                return dest;
            return default;
            /*ElevatorChamber[] chambers = Elevators;
            foreach (ElevatorChamber chamber in chambers)
            {
                if (!chamber.IsReady)
                    continue;

                chamber.
            }*/
        }

        protected virtual Vector3 HCZElevator()
        {
            if (NotInElevatorRoom(FacilityZone.HeavyContainment, out Vector3 dest))
                return dest;
            return default;
        }

        protected virtual Vector3 EZElevator()
        {
            if (NotInElevatorRoom(FacilityZone.Entrance, out Vector3 dest))
                return dest;
            return default;
        }

        protected virtual Vector3 SurfaceElevator()
        {
            return default;
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
