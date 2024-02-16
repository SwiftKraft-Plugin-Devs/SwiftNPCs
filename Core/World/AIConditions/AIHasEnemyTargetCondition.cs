using SwiftNPCs.Core.World.AIModules;

namespace SwiftNPCs.Core.World.AIConditions
{
    public class AIHasEnemyTargetCondition : AIConditionBase
    {
        public bool Reverse;

        public override bool Get()
        {
            return Reverse ? !ParentModule.Parent.HasEnemyTarget : ParentModule.Parent.HasEnemyTarget;
        }

        public override void Pass(AIModuleBase target) { }
    }
}
