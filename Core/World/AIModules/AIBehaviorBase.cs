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

            if (Parent.HasEnemyTarget)
            {
                Parent.ChangeModule(AttackerTag, true);
                Parent.ChangeModule(MoverTag, false);
            }
            else if (Parent.HasFollowTarget)
            {
                Parent.ChangeModule(AttackerTag, false);
                Parent.ChangeModule(MoverTag, true);
            }
            else
            {
                Parent.ChangeModule(AttackerTag, false);
                Parent.ChangeModule(MoverTag, false);
            }
        }
    }
}
