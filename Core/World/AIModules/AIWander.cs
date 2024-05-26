using PluginAPI.Core;
using PluginAPI.Core.Zones;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIWander : AIPathfind
    {
        public float WanderTimerMin = 10f;
        public float WanderTimerMax = 30f;

        public bool ActiveWhenFollow;

        float timer;

        public FacilityRoom GetRandomRoomInZone()
        {
            List<FacilityRoom> rooms = [];

            foreach (FacilityRoom room in Facility.Rooms)
                if (room.GameObject.activeSelf && room.Zone.ZoneType == Parent.Core.Profile.Player.Zone)
                    rooms.Add(room);

            return rooms.Count > 0 ? rooms.RandomItem() : null;
        }

        public bool TryGetRandomRoomInZone(out FacilityRoom room)
        {
            room = GetRandomRoomInZone();
            return room != null;
        }

        public override void Init()
        {
            base.Init();
            Tags = [AIBehaviorBase.MoverTag];
        }

        public override void Tick()
        {
            if (!Enabled || (!ActiveWhenFollow && Parent.HasFollowTarget))
            {
                ClearDestination();
                timer = 0f;
                return;
            }

            if (timer > 0f)
                timer -= Time.fixedDeltaTime;

            if ((AtDestination || timer <= 0f) && TryGetRandomRoomInZone(out FacilityRoom room) && NavMesh.SamplePosition(room.Position, out NavMeshHit _hit, 50f, NavMesh.AllAreas))
            {
                timer = Random.Range(WanderTimerMin, WanderTimerMax);
                SetDestination(_hit.position);
            }

            base.Tick();
        }
    }
}
