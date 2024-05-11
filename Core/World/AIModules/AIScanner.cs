using PluginAPI.Core;
using System.Collections.Generic;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIScanner : AIModuleBase
    {
        public float SearchRadiusEnemy = 70f;
        public float SearchRadiusFollow = 20f;

        public Player LookTarget => Parent.FollowTarget;

        public override void Init()
        {
            Tags = [AIBehaviorBase.DetectorTag];
        }

        public override void OnDisabled() { }

        public override void OnEnabled() { }

        public override void Tick()
        {
            if (Parent.RetargetTimer <= 0f)
            {
                Parent.RetargetTimer = AIModuleRunner.RetargetTime;

                List<Player> players = Player.GetPlayers();

                Player target = null;
                Player follow = null;

                foreach (Player p in players)
                {
                    if (Parent.WithinDistance(p, SearchRadiusEnemy) && Parent.CanTarget(p) && (target == null || Parent.GetDistance(target) > Parent.GetDistance(p)))
                        target = p;
                    else if (!Parent.HasFollowTarget && Parent.WithinDistance(p, SearchRadiusFollow) && Parent.CanFollow(p) && (follow == null || Parent.GetDistance(follow) > Parent.GetDistance(p)))
                        follow = p;
                }

                Parent.EnemyTarget = target;

                if (!Parent.HasFollowTarget)
                    Parent.FollowTarget = follow;
            }

            if (!Enabled)
                return;

            if (LookTarget != null)
                Parent.MovementEngine.LookPos = LookTarget.Camera.position;
        }
    }
}
