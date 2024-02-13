using SwiftNPCs.Core.World.AIModules;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIConditions
{
    public abstract class AIConditionBase
    {
        public AIModuleBase ParentModule;

        public AIModuleRunner Runner => ParentModule.Parent;

        public ReferenceHub ReferenceHub => Runner.ReferenceHub;

        public Vector3 Position => ReferenceHub.transform.position;

        public abstract bool Get();

        public abstract void Pass(AIModuleBase target);
    }
}
