using PluginAPI.Core;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIConditions
{
    public abstract class AIDistanceCondition : AIConditionBase
    {
        public float Distance;

        public bool Reverse;

        public float GetDistance(Player p)
        {
            if (p == null)
                return Mathf.Infinity;

            return Vector3.Distance(Position, p.Position);
        }

        public bool Get(Player p)
        {
            return Reverse ? GetDistance(p) > Distance : GetDistance(p) <= Distance;
        }
    }
}
