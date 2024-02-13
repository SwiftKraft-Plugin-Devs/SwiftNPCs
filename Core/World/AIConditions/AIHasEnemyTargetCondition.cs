using SwiftNPCs.Core.World.AIModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
