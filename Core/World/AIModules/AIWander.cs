using PluginAPI.Core;
using PluginAPI.Core.Zones;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIWander : AIPathfind
    {
        public float WanderTimer = 10f;

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

        public override void Tick()
        {
            base.Tick();

            if (timer > 0f)
                timer -= Time.fixedDeltaTime;

            if ((AtDestination || timer <= 0f) && TryGetRandomRoomInZone(out FacilityRoom room))
            {
                timer = WanderTimer;
                Log.Info(room.GameObject.name);
                SetDestination(room.Position);
            }
        }
    }
}
