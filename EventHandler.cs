using HarmonyLib;
using MapGeneration;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using SwiftNPCs.Core.Management;
using SwiftNPCs.Core.Pathing;
using System.Collections.Generic;

namespace SwiftNPCs
{
    public class EventHandler
    {
        [PluginEvent(ServerEventType.PlayerDamage)]
        public bool PlayerDamage(PlayerDamageEvent _event)
        {
            if (_event.Target.TryGetAI(out AIPlayerProfile prof))
            {
                prof.WorldPlayer.Damage(_event.Player);

                if (_event.DamageHandler is UniversalDamageHandler dmg && dmg.TranslationId == DeathTranslations.Falldown.Id)
                    return false;
            }

            return true;
        }

        [PluginEvent(ServerEventType.PlayerDying)]
        public void PlayerDying(PlayerDyingEvent _event)
        {
            if (_event.Player.TryGetAI(out AIPlayerProfile prof))
                prof.ReferenceHub.transform.eulerAngles = new(0f, prof.ReferenceHub.transform.eulerAngles.y, 0f);
        }

        [PluginEvent(ServerEventType.PlayerChangeRole)]
        public void PlayerChangeRole(PlayerChangeRoleEvent _event)
        {
            if (_event.Player.TryGetAI(out AIPlayerProfile prof))
                prof.WorldPlayer.RoleChange(_event.NewRole);
        }

        [PluginEvent(ServerEventType.RoundRestart)]
        public void RoundRestart(RoundRestartEvent _event)
        {
            List<Player> players = Player.GetPlayers();

            foreach (Player p in players)
                if (p.TryGetAI(out AIPlayerProfile prof))
                    prof.Delete();
        }

        [PluginEvent(ServerEventType.MapGenerated)]
        public void MapGenerated(MapGeneratedEvent _event)
        {
            NavMeshManager.InitializeMap();

            foreach (RoomIdentifier rid in RoomIdentifier.AllRoomIdentifiers)
            {
                switch (rid.Name)
                {
                    case RoomName.LczCheckpointA:
                    case RoomName.LczCheckpointB:
                    case RoomName.HczCheckpointA:
                    case RoomName.HczCheckpointB:
                    case RoomName.EzGateA:
                    case RoomName.EzGateB:
                        Add(rid);
                        break;
                }
            }

            static void Add(RoomIdentifier rid)
            {
                if (Utilities.ZoneElevatorRooms.ContainsKey(rid.Zone))
                    Utilities.ZoneElevatorRooms[rid.Zone].AddItem(rid);
                else
                    Utilities.ZoneElevatorRooms.Add(rid.Zone, [rid]);
            }
        }
    }
}
