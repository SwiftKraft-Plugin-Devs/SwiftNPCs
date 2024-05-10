using PluginAPI.Core;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIScanner : AIModuleBase
    {
        public Player LookTarget => Parent.FollowTarget;

        public override void Init() { }

        public override void OnDisabled() { }

        public override void OnEnabled() { }

        public override void Tick()
        {
            if (LookTarget != null)
                Parent.MovementEngine.LookPos = LookTarget.Camera.position;
        }
    }
}
