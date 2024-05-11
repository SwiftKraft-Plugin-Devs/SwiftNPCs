using PluginAPI.Core;
using System.Collections.Generic;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIScanner : AIModuleBase
    {
        public float SearchRadius;

        public Player LookTarget => Parent.FollowTarget;

        public override void Init() { }

        public override void OnDisabled() { }

        public override void OnEnabled() { }

        public override void Tick()
        {
            List<Player> players = Player.GetPlayers();

            foreach (Player p in players)
            {
                if ()
            }

            if (!Enabled)
                return;

            if (LookTarget != null)
                Parent.MovementEngine.LookPos = LookTarget.Camera.position;
        }
    }
}
