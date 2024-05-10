using PlayerRoles;
using SwiftNPCs.Core.Management;
using SwiftNPCs.Core.Pathing;
using SwiftNPCs.Core.World.AIConditions;
using SwiftNPCs.Core.World.AIModules;
using UnityEngine;
using UserSettings;

namespace SwiftNPCs
{
    public static class Utilities
    {
        public static AIPlayerProfile CreateBasicAI(RoleTypeId role, Vector3 position, float enemyDistance = 50f, float followDistance = 10f, float startFollowDistance = 3f, float stopFollowDistance = 1f)
        {
            AIPlayerProfile prof = new AIDataProfileBase("Bot").CreateAIPlayer();
            prof.DisplayNickname = "Bot " + prof.Player.PlayerId;

            prof.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.None, RoleSpawnFlags.None);
            prof.Position = position;

            AIStandIdle i = prof.WorldPlayer.ModuleRunner.AddModule<AIStandIdle>();
            AISimpleFollow f = prof.WorldPlayer.ModuleRunner.AddModule<AISimpleFollow>();
            AIFirearmShoot s = prof.WorldPlayer.ModuleRunner.AddModule<AIFirearmShoot>();
            AIGrenadeThrow g = prof.WorldPlayer.ModuleRunner.AddModule<AIGrenadeThrow>();

            i.AddTransition(f, new AIHasFollowTargetCondition() { Reverse = true }, new AIFindPlayerCondition() { SearchDistance = followDistance });
            i.AddTransition(f, new AIHasFollowTargetCondition(), new AIFollowDistanceCondition() { Reverse = true, Distance = startFollowDistance });
            i.AddTransition(s, new AIHasItemCategoryCondition() { Category = ItemCategory.Firearm }, new AIHasEnemyTargetCondition() { Reverse = true }, new AIFindEnemyCondition() { SearchDistance = enemyDistance });
            i.AddTransition(s, new AIHasItemCategoryCondition() { Category = ItemCategory.Firearm }, new AIHasEnemyTargetCondition(), new AIEnemyDistanceCondition() { Distance = enemyDistance }, new AIEnemyLOSCondition());
            i.AddTransition(g, new AIHasItemCategoryCondition() { Category = ItemCategory.Grenade }, new AIHasEnemyTargetCondition() { Reverse = true }, new AIFindEnemyCondition() { SearchDistance = enemyDistance });
            i.AddTransition(g, new AIHasItemCategoryCondition() { Category = ItemCategory.Grenade }, new AIEnemyDistanceCondition() { Distance = enemyDistance }, new AIEnemyLOSCondition());

            f.AddTransition(i, new AIHasFollowTargetCondition() { Reverse = true });
            f.AddTransition(i, new AIFollowDistanceCondition() { Distance = stopFollowDistance });
            f.AddTransition(s, new AIHasItemCategoryCondition() { Category = ItemCategory.Firearm }, new AIHasEnemyTargetCondition() { Reverse = true }, new AIFindEnemyCondition() { SearchDistance = enemyDistance });
            f.AddTransition(s, new AIHasItemCategoryCondition() { Category = ItemCategory.Firearm }, new AIHasEnemyTargetCondition(), new AIEnemyDistanceCondition() { Distance = enemyDistance }, new AIEnemyLOSCondition());
            f.AddTransition(g, new AIHasItemCategoryCondition() { Category = ItemCategory.Grenade }, new AIHasEnemyTargetCondition() { Reverse = true }, new AIFindEnemyCondition() { SearchDistance = enemyDistance });
            f.AddTransition(g, new AIHasItemCategoryCondition() { Category = ItemCategory.Grenade }, new AIEnemyDistanceCondition() { Distance = enemyDistance }, new AIEnemyLOSCondition());

            s.AddTransition(i, new AIEnemyDistanceCondition() { Reverse = true, Distance = enemyDistance });
            s.AddTransition(i, new AIEnemyLOSCondition() { Reverse = true });
            s.AddTransition(i, new AIHasEnemyTargetCondition() { Reverse = true });

            g.AddTransition(i, new AIEnemyDistanceCondition() { Reverse = true, Distance = enemyDistance });
            g.AddTransition(i, new AIEnemyLOSCondition() { Reverse = true });
            g.AddTransition(i, new AIHasEnemyTargetCondition() { Reverse = true });

            prof.WorldPlayer.ModuleRunner.ActivateModule(i);

            return prof;
        }

        public static AIPlayerProfile CreateShootingDummy(RoleTypeId role, Vector3 position)
        {
            AIPlayerProfile p = CreateStaticAI(role, position);
            p.Player.Health = 999999f;
            return p;
        }

        public static AIPlayerProfile CreateStaticAI(RoleTypeId role, Vector3 position, float enemyDistance = 50f)
        {
            AIPlayerProfile prof = new AIDataProfileBase("Static Bot").CreateAIPlayer();
            prof.DisplayNickname = "Static Bot " + prof.Player.PlayerId;

            prof.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.None, RoleSpawnFlags.None);
            prof.Position = position;

            AIStandIdle i = prof.WorldPlayer.ModuleRunner.AddModule<AIStandIdle>();
            AIFirearmShoot s = prof.WorldPlayer.ModuleRunner.AddModule<AIFirearmShoot>();
            AIGrenadeThrow g = prof.WorldPlayer.ModuleRunner.AddModule<AIGrenadeThrow>();

            g.InfiniteGrenades = true;

            i.AddTransition(s, new AIHasItemCategoryCondition() { Category = ItemCategory.Firearm }, new AIHasEnemyTargetCondition() { Reverse = true }, new AIFindEnemyCondition() { SearchDistance = enemyDistance });
            i.AddTransition(s, new AIHasItemCategoryCondition() { Category = ItemCategory.Firearm }, new AIEnemyDistanceCondition() { Distance = enemyDistance }, new AIEnemyLOSCondition());

            i.AddTransition(g, new AIHasItemCategoryCondition() { Category = ItemCategory.Grenade }, new AIHasEnemyTargetCondition() { Reverse = true }, new AIFindEnemyCondition() { SearchDistance = enemyDistance });
            i.AddTransition(g, new AIHasItemCategoryCondition() { Category = ItemCategory.Grenade }, new AIEnemyDistanceCondition() { Distance = enemyDistance }, new AIEnemyLOSCondition());

            s.AddTransition(i, new AIEnemyDistanceCondition() { Reverse = true, Distance = enemyDistance });
            s.AddTransition(i, new AIEnemyLOSCondition() { Reverse = true });
            s.AddTransition(i, new AIHasEnemyTargetCondition() { Reverse = true });

            g.AddTransition(i, new AIEnemyDistanceCondition() { Reverse = true, Distance = enemyDistance });
            g.AddTransition(i, new AIEnemyLOSCondition() { Reverse = true });
            g.AddTransition(i, new AIHasEnemyTargetCondition() { Reverse = true });

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

        public static AIPlayerProfile CreateGrenadeAI(RoleTypeId role, Vector3 position, float enemyDistance = 50f, float followDistance = 10f, float startFollowDistance = 3f, float stopFollowDistance = 1f)
        {
            AIPlayerProfile prof = new AIDataProfileBase("Grenade Bot").CreateAIPlayer();
            prof.DisplayNickname = "Grenade Bot " + prof.Player.PlayerId;

            prof.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.None, RoleSpawnFlags.None);
            prof.Position = position;

            AIStandIdle i = prof.WorldPlayer.ModuleRunner.AddModule<AIStandIdle>();
            AISimpleFollow f = prof.WorldPlayer.ModuleRunner.AddModule<AISimpleFollow>();
            AIGrenadeThrow s = prof.WorldPlayer.ModuleRunner.AddModule<AIGrenadeThrow>();

            i.AddTransition(f, new AIHasFollowTargetCondition() { Reverse = true }, new AIFindPlayerCondition() { SearchDistance = followDistance });
            i.AddTransition(f, new AIHasFollowTargetCondition(), new AIFollowDistanceCondition() { Reverse = true, Distance = startFollowDistance });
            i.AddTransition(s, new AIHasEnemyTargetCondition() { Reverse = true }, new AIFindEnemyCondition() { SearchDistance = enemyDistance });
            i.AddTransition(s, new AIHasEnemyTargetCondition(), new AIEnemyDistanceCondition() { Distance = enemyDistance }, new AIEnemyLOSCondition());

            f.AddTransition(i, new AIHasFollowTargetCondition() { Reverse = true });
            f.AddTransition(i, new AIFollowDistanceCondition() { Distance = stopFollowDistance });

            s.AddTransition(i, new AIEnemyDistanceCondition() { Reverse = true, Distance = enemyDistance });

            s.AddTransition(i, new AIEnemyLOSCondition() { Reverse = true });
            s.AddTransition(i, new AIHasEnemyTargetCondition() { Reverse = true });

            prof.WorldPlayer.ModuleRunner.ActivateModule(i);

            return prof;
        }
    }
}
