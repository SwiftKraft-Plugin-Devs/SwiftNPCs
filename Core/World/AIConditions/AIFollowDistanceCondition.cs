using PluginAPI.Core;
using SwiftNPCs.Core.World.AIModules;

namespace SwiftNPCs.Core.World.AIConditions
{
    public class AIFollowDistanceCondition : AIDistanceCondition
    {
        public Player FollowTarget => Runner.FollowTarget;

        public override bool Get()
        {
            return Get(FollowTarget);
        }

        public override void Pass(AIModuleBase target) { }
    }
}
