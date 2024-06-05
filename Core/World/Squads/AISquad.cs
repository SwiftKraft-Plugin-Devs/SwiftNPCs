using PluginAPI.Core;
using SwiftNPCs.Core.Management;
using System.Collections.Generic;

namespace SwiftNPCs.Core.World.Squads
{
    public class AISquad(Player leader, int limit = 5)
    {
        public readonly Player Leader = leader;

        public readonly List<AIPlayerProfile> Members = [];

        public int Limit { get; private set; } = limit;

        public void SetLimit(int limit)
        {
            Limit = limit;
            if (Members.Count > Limit)
                Members.RemoveRange(Limit, Members.Count - Limit);
        }
    }
}
