using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIBehaviorBase : AIModuleBase
    {
        public const string AttackerTag = "Attacker";
        public const string MoverTag = "Mover";
        public const string DetectorTag = "Detector";

        protected float Timer;

        public override void Init() { }

        public override void OnDisabled() { }

        public override void OnEnabled() { }

        public override void Tick()
        {
            if (Timer > 0f)
            {
                Timer -= Time.fixedDeltaTime;
                return;
            }

            if (!Enabled)
                return;

            if (Parent.HasEnemyTarget && Parent.ActivateRandomModuleByTag(AttackerTag, out AIModuleBase mod, true))
            {
                Timer = mod.Duration;
                Parent.ChangeModule(MoverTag, false);
            }
            else if (!Parent.HasEnemyTarget && Parent.HasFollowTarget)
            {
                Parent.ChangeModule(AttackerTag, false);
                Parent.ChangeModule(MoverTag, true);
            }
            else
            {
                Parent.ChangeModule(AttackerTag, true);
                Parent.ChangeModule(MoverTag, true);
            }
        }
    }
}
