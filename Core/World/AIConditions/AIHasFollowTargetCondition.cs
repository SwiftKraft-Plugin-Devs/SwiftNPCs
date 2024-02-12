using SwiftNPCs.Core.World.AIModules;

namespace SwiftNPCs.Core.World.AIConditions
{
    public class AIHasFollowTargetCondition : AIConditionBase
    {
        public bool Reverse;

        public override bool Get()
        {
            return Reverse ? !ParentModule.Parent.HasFollowTarget : ParentModule.Parent.HasFollowTarget;
        }

        public override void Pass(AIModuleBase target) { }
    }
}
