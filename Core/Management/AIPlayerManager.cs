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
        public static readonly List<AIPlayerProfile> Registered = [];

        /// <summary>
        /// Creates a fake client and adds it to the registered list.
        /// </summary>
        public static AIPlayerProfile CreateAIPlayer(this AIDataProfileBase profile)
        {
            int id = 100 + Registered.Count;
            GameObject playerBody = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            var fakeClient = new FakeClient(id);
            NetworkServer.AddPlayerForConnection(fakeClient, playerBody);
            ReferenceHub hub = playerBody.GetComponent<ReferenceHub>();
            AIPlayer aiCont = playerBody.AddComponent<AIPlayer>();
            AIPlayerProfile prof = new(fakeClient, id, hub, aiCont, profile);
            Registered.Add(prof);
            return prof;
        }

        public static AIPlayerProfile GetAIPlayer(this int aiId)
        {
            if (aiId >= Registered.Count)
                return null;

            return Registered[aiId];
        }

        public static Player AIIDToPlayer(this int aiId)
        {
            if (Player.TryGet(GetAIPlayer(aiId).ReferenceHub, out Player player))
                return player;
            return null;
        }

        public static AIPlayerProfile GetAI(this Player p) => p.ReferenceHub.GetAI();

        public static AIPlayerProfile GetAI(this ReferenceHub p)
        {
            foreach (AIPlayerProfile prof in Registered)
                if (prof.ReferenceHub == p)
                    return prof;
            return null;
        }

        public static bool TryGetAI(this Player p, out AIPlayerProfile ai) => p.ReferenceHub.TryGetAI(out ai);

        public static bool TryGetAI(this ReferenceHub p, out AIPlayerProfile ai)
        {
            ai = p.GetAI();
            return ai != null;
        }

        public static bool IsAI(this Player p) => p.ReferenceHub.IsAI();

        public static bool IsAI(this ReferenceHub p) => p.TryGetAI(out _);

        public static void Delete(this AIPlayerProfile prof)
        {
            Registered.Remove(prof);
            NetworkServer.RemovePlayerForConnection(prof.Connection, true);
            Object.Destroy(prof.WorldPlayer.gameObject);
        }
    }
}
