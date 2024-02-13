﻿using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using SwiftNPCs.Core.Management;
using System.Collections.Generic;

namespace SwiftNPCs
{
    public class EventHandler
    {
        [PluginEvent(ServerEventType.PlayerDamage)]
        public void PlayerDamage(PlayerDamageEvent _event)
        {
            if (_event.Target.TryGetAI(out AIPlayerProfile prof))
                prof.WorldPlayer.Damage(_event.Player);
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
                if (p.IsAI())
                    p.Kick("Remove AI");
        }
    }
}
