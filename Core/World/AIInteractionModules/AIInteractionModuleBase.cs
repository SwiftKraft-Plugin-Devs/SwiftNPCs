namespace SwiftNPCs.Core.World.AIInteractionModules
{
    public abstract class AIInteractionModuleBase
    {
        public AIInteractionRunner Parent;

        public abstract void Init();

        public abstract void Tick();
    }
}
