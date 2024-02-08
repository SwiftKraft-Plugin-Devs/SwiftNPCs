using Mirror;
using PluginAPI.Core;
using SwiftNPCs.Core.World;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SwiftNPCs.Core.Management
{
    public static class AIPlayerManager
    {
        /// <summary>
        /// List of registered AI Players.
        /// </summary>
        public static readonly List<AIPlayerProfile> Registered = new List<AIPlayerProfile>();

        /// <summary>
        /// Creates a fake client and adds it to the registered list.
        /// </summary>
        public static AIPlayerProfile CreateAIPlayer(AIDataProfileBase profile)
        {
            int id = 100 + Registered.Count;
            GameObject playerBody = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            var fakeClient = new FakeClient(id);
            NetworkServer.AddPlayerForConnection(fakeClient, playerBody);
            ReferenceHub hub = playerBody.GetComponent<ReferenceHub>();
            AIPlayer aiCont = playerBody.AddComponent<AIPlayer>();
            AIPlayerProfile prof = new AIPlayerProfile(id, hub, aiCont, profile);
            Registered.Add(prof);
            return prof;
        }

        public static AIPlayerProfile GetAIPlayer(int aiId)
        {
            if (aiId >= Registered.Count)
                return null;

            return Registered[aiId];
        }

        public static Player AIIDToPlayer(int aiId)
        {
            if (Player.TryGet(GetAIPlayer(aiId).ReferenceHub, out Player player))
                return player;
            return null;
        }
    }
}
