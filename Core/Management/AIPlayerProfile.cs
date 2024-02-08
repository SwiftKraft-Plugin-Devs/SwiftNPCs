﻿using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using SwiftNPCs.Core.World;
using UnityEngine;

namespace SwiftNPCs.Core.Management
{
    public class AIPlayerProfile
    {
        public readonly AIDataProfileBase Data;

        public readonly int ConnectionID;

        public readonly ReferenceHub ReferenceHub;

        public readonly Player Player;

        /// <summary>
        /// The component on the world player
        /// </summary>
        public AIPlayer WorldPlayer;

        public AIPlayerProfile(int connId, ReferenceHub hub, AIPlayer world, AIDataProfileBase data)
        {
            ConnectionID = connId;
            ReferenceHub = hub;
            WorldPlayer = world;
            WorldPlayer.Profile = this;
            Data = data;
            Player = Player.Get(ReferenceHub);
        }

        public Vector3 Position
        {
            get
            {
                return ReferenceHub.transform.position;
            }
            set
            {
                ReferenceHub.TryOverridePosition(value, Vector3.zero);
            }
        }

        public Vector3 Rotation
        {
            get
            {
                return ReferenceHub.transform.eulerAngles;
            }
            set
            {
                ReferenceHub.TryOverridePosition(Position, value);
            }
        }
    }
}
