namespace SwiftNPCs.Core.World.AIModules
{
    public class AIStandIdle : AIModuleBase
    {
        public override void End(AIModuleBase next) { }

        public override void Init() { }

        public override void ReceiveData<T>(T data) { }

        public override void Start(AIModuleBase prev) { }

        public override void Tick()
        {
            CheckTransitions();
        }
    }
}
