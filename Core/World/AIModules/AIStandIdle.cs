using PluginAPI.Core;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIStandIdle : AIModuleBase
    {
        public Player LookTarget;

        public override void End(AIModuleBase next) { }

        public override void Init() { }

        public override void ReceiveData<T>(T data)
        {
            if (!(data is Player p) || !Parent.HasLOS(p))
                return;

            LookTarget = p;
        }

        public override void Start(AIModuleBase prev) { }

        public override void Tick()
        {
            CheckTransitions();

            if (LookTarget != null && !Parent.HasLOS(LookTarget))
                LookTarget = null;
        }
    }
}
