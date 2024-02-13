using PluginAPI.Core;
using SwiftNPCs.Core.World.AIModules;

namespace SwiftNPCs.Core.World.AIConditions
{
    public class AIEnemyDistanceCondition : AIDistanceCondition
    {
        public Player EnemyTarget => Runner.EnemyTarget;

        public override bool Get()
        {
            return Get(EnemyTarget);
        }

        public override void Pass(AIModuleBase target) { }
    }
}
