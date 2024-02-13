using SwiftNPCs.Core.World.AIModules;

namespace SwiftNPCs.Core.World.AIConditions
{
    public class AIEnemyLOSCondition : AIConditionBase
    {
        public bool Reverse;

        public override bool Get() => Reverse ? !ParentModule.Parent.HasLOSOnEnemy(out _) : ParentModule.Parent.HasLOSOnEnemy(out _);

        public override void Pass(AIModuleBase target) { }
    }
}
