using Mirror;
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
        public static void CreateAIPlayer()
        {
            int id = 100 + Registered.Count;
            GameObject playerBody = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            var fakeClient = new FakeClient(id);
            NetworkServer.AddPlayerForConnection(fakeClient, playerBody);
            ReferenceHub hub = playerBody.GetComponent<ReferenceHub>();
            ServerConsole.PlayersListRaw.objects.Remove(hub.authManager.UserId);
            hub.authManager.InstanceMode = CentralAuth.ClientInstanceMode.DedicatedServer;
            Registered.Add(new(id, hub));
        }
    }
}
