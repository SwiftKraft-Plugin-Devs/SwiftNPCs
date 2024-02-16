using PlayerRoles;
using SwiftNPCs.Core.Management;
using SwiftNPCs.Core.Pathing;
using SwiftNPCs.Core.World.AIConditions;
using SwiftNPCs.Core.World.AIModules;
using UnityEngine;

namespace SwiftNPCs
{
    public static class Utilities
    {
        public static AIPlayerProfile CreateBasicAI(RoleTypeId role, Vector3 position)
        {
            AIPlayerProfile prof = new AIDataProfileBase("Bot").CreateAIPlayer();
            prof.DisplayNickname = "Bot " + prof.Player.PlayerId;

            prof.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.None, RoleSpawnFlags.None);
            prof.Position = position;

            AIStandIdle i = prof.WorldPlayer.ModuleRunner.AddModule<AIStandIdle>();
            AISimpleFollow f = prof.WorldPlayer.ModuleRunner.AddModule<AISimpleFollow>();
            AIFirearmShoot s = prof.WorldPlayer.ModuleRunner.AddModule<AIFirearmShoot>();

            i.AddTransition(f, new AIHasFollowTargetCondition() { Reverse = true }, new AIFindPlayerCondition() { SearchDistance = 10f });
            i.AddTransition(f, new AIHasFollowTargetCondition(), new AIFollowDistanceCondition() { Reverse = true, Distance = 3f });
            i.AddTransition(s, new AIHasEnemyTargetCondition() { Reverse = true }, new AIFindEnemyCondition() { SearchDistance = 50f });
            i.AddTransition(s, new AIHasEnemyTargetCondition(), new AIEnemyDistanceCondition() { Distance = 50f }, new AIEnemyLOSCondition());

            f.AddTransition(i, new AIHasFollowTargetCondition() { Reverse = true });
            f.AddTransition(i, new AIFollowDistanceCondition() { Distance = 1f });

            s.AddTransition(i, new AIEnemyDistanceCondition() { Reverse = true, Distance = 50f });
            s.AddTransition(i, new AIEnemyLOSCondition() { Reverse = true });
            s.AddTransition(i, new AIHasEnemyTargetCondition() { Reverse = true });

            prof.WorldPlayer.ModuleRunner.ActivateModule(i);

            return prof;
        }

        public static AIPlayerProfile CreateStaticAI(RoleTypeId role, Vector3 position)
        {
            AIPlayerProfile prof = new AIDataProfileBase("Static Bot").CreateAIPlayer();
            prof.DisplayNickname = "Static Bot " + prof.Player.PlayerId;

            prof.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.None, RoleSpawnFlags.None);
            prof.Position = position;

            AIStandIdle i = prof.WorldPlayer.ModuleRunner.AddModule<AIStandIdle>();
            AIFirearmShoot s = prof.WorldPlayer.ModuleRunner.AddModule<AIFirearmShoot>();

            i.AddTransition(s, new AIHasEnemyTargetCondition() { Reverse = true }, new AIFindEnemyCondition() { SearchDistance = 50f });
            i.AddTransition(s, new AIHasEnemyTargetCondition(), new AIEnemyDistanceCondition() { Distance = 50f }, new AIEnemyLOSCondition());

            s.AddTransition(i, new AIEnemyDistanceCondition() { Reverse = true, Distance = 50f });
            s.AddTransition(i, new AIEnemyLOSCondition() { Reverse = true });
            s.AddTransition(i, new AIHasEnemyTargetCondition() { Reverse = true });

            prof.WorldPlayer.ModuleRunner.ActivateModule(i);

            return prof;
        }

        public static AIPlayerProfile CreatePathAI(RoleTypeId role, Vector3 position, Path p)
        {
            AIPlayerProfile prof = new AIDataProfileBase("Path Bot").CreateAIPlayer();
            prof.DisplayNickname = "Path Bot " + prof.Player.PlayerId;

            prof.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.None, RoleSpawnFlags.None);
            prof.Position = position;

            AIFollowPath f = prof.WorldPlayer.ModuleRunner.AddModule<AIFollowPath>();
            f.Path = p;
            f.InitPath();

            prof.WorldPlayer.ModuleRunner.ActivateModule(f);

            return prof;
        }
    }
}
