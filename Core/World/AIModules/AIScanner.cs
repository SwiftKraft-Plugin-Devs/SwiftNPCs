using PluginAPI.Core;
using SwiftNPCs.Core.World.Targetables;
using System.Collections.Generic;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIScanner : AIModuleBase
    {
        public float SearchRadiusEnemy = 70f;
        public float SearchRadiusFollow = 20f;

        public TargetableBase LookTarget => Parent.FollowTarget;

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

                TargetableBase target = null;
                TargetableBase follow = null;

                foreach (Player p in players)
                {
                    if (Parent.WithinDistance(p, SearchRadiusEnemy) && Parent.CanTarget(p, out _) && Parent.IsInView(p) && (target == null || Parent.GetDistance(target) > Parent.GetDistance(p)))
                        target = p;
                    else if (!Parent.HasFollowTarget && Parent.GetFollowWeight(p) > 0 && Parent.WithinDistance(p, SearchRadiusFollow) && Parent.CanFollow(p) && Parent.IsInView(p) && (follow == null || Parent.GetFollowWeight(follow) < Parent.GetFollowWeight(p) || Parent.GetDistance(follow) > Parent.GetDistance(p)))
                        follow = p;
                }

                Parent.EnemyTarget = target;

                if (follow != null)
                    Parent.FollowTarget = follow;
            }
        }
    }
}
