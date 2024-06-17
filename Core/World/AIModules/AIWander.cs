using PluginAPI.Core;
using PluginAPI.Core.Zones;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIWander : AIModuleBase
    {
        public readonly List<FacilityRoom> Blacklist = [];

        public float WanderTimerMin = 10f;
        public float WanderTimerMax = 30f;
        public float SurfaceRadius = 30f;
        public float RoomRadius = 60f;

        public bool ActiveWhenFollow;

        AIPathfinder Pathfinder;

        float timer;

        public FacilityRoom GetRandomRoomInLayer()
        {
            List<FacilityRoom> rooms = [];

            foreach (FacilityRoom room in Facility.Rooms)
                if (room.GameObject.activeSelf && !Blacklist.Contains(room) && Vector3.Distance(Parent.Position, room.Position) <= RoomRadius && RoomIsInLayer(room))
                    rooms.Add(room);

            return rooms.Count > 0 ? rooms.RandomItem() : null;
        }

        public bool RoomIsInLayer(FacilityRoom room)
        {
            switch (Parent.Core.Profile.Player.Zone)
            {
                case MapGeneration.FacilityZone.HeavyContainment:
                    if (room.Zone.ZoneType == MapGeneration.FacilityZone.Entrance)
                        return true;
                    break;
                case MapGeneration.FacilityZone.Entrance:
                    if (room.Zone.ZoneType == MapGeneration.FacilityZone.HeavyContainment)
                        return true;
                    break;
            }

            return room.Zone.ZoneType == Parent.Core.Profile.Player.Zone;
        }

        public bool TryGetRandomRoomInLayer(out FacilityRoom room)
        {
            room = GetRandomRoomInLayer();
            return room != null;
        }

        public override void Init()
        {
            Pathfinder = Parent.GetModule<AIPathfinder>();
            Tags = [AIBehaviorBase.AutonomyTag];
        }

        public override void Tick()
        {
            if (!Enabled || (!ActiveWhenFollow && Parent.HasFollowTarget))
            {
                timer = 0f;
                return;
            }

            if (timer > 0f)
                timer -= Time.fixedDeltaTime;

            if (Pathfinder.AtDestination || timer <= 0f)
                SetDestination();
        }

        public void SetDestination()
        {
            if (TryGetRandomRoomInLayer(out FacilityRoom room) && NavMesh.SamplePosition(room.Position, out NavMeshHit _hit, 50f, NavMesh.AllAreas))
            {
                timer = Random.Range(WanderTimerMin, WanderTimerMax);

                if (room.Identifier.Name == MapGeneration.RoomName.Outside)
                    NavMesh.SamplePosition(Parent.Position + Random.insideUnitSphere * SurfaceRadius, out _hit, 100f, NavMesh.AllAreas);

                Pathfinder.SetDestination(_hit.position);
            }
        }

        public override void OnEnabled()
        {
            SetDestination();
        }

        public override void OnDisabled()
        {
            Pathfinder.ClearDestination();
        }
    }
}
