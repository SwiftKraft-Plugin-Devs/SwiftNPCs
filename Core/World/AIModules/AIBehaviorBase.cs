namespace SwiftNPCs.Core.World.AIModules
{
    public class AIBehaviorBase : AIModuleBase
    {
        public const string AttackerTag = "Attacker";
        public const string MoverTag = "Mover";
        public const string DetectorTag = "Detector";

        public override void Init() { }

        public override void OnDisabled() { }

        public override void OnEnabled() { }

        public override void Tick()
        {
            if (!Enabled)
                return;
        }
    }
}
