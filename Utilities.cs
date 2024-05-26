using PlayerRoles;
using SwiftNPCs.Core.Management;
using SwiftNPCs.Core.Pathing;
using SwiftNPCs.Core.World.AIModules;
using UnityEngine;

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

            AIScanner i = prof.WorldPlayer.ModuleRunner.AddModule<AIScanner>();
            AIFollow f = prof.WorldPlayer.ModuleRunner.AddModule<AIFollow>();
            AIFirearmShoot s = prof.WorldPlayer.ModuleRunner.AddModule<AIFirearmStrafeShoot>();
            AIGrenadeThrow g = prof.WorldPlayer.ModuleRunner.AddModule<AIGrenadeThrow>();
            AIItemStrafeConsume c = prof.WorldPlayer.ModuleRunner.AddModule<AIItemStrafeConsume>();
            c.Enabled = false;
            prof.WorldPlayer.ModuleRunner.AddModule<AIBehaviorBase>();

            i.SearchRadiusEnemy = 70f;
            i.SearchRadiusFollow = 20f;

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

            AIScanner i = prof.WorldPlayer.ModuleRunner.AddModule<AIScanner>();
            AIFirearmShoot s = prof.WorldPlayer.ModuleRunner.AddModule<AIFirearmShoot>();
            AIGrenadeThrow g = prof.WorldPlayer.ModuleRunner.AddModule<AIGrenadeThrow>();
            prof.WorldPlayer.ModuleRunner.AddModule<AIBehaviorBase>();

            g.InfiniteGrenades = true;

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

            return prof;
        }

        public static AIPlayerProfile CreateGrenadeAI(RoleTypeId role, Vector3 position, float enemyDistance = 50f, float followDistance = 10f, float startFollowDistance = 3f, float stopFollowDistance = 1f)
        {
            AIPlayerProfile prof = new AIDataProfileBase("Grenade Bot").CreateAIPlayer();
            prof.DisplayNickname = "Grenade Bot " + prof.Player.PlayerId;

            prof.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.None, RoleSpawnFlags.None);
            prof.Position = position;

            AIScanner i = prof.WorldPlayer.ModuleRunner.AddModule<AIScanner>();
            AIFollow f = prof.WorldPlayer.ModuleRunner.AddModule<AIFollow>();
            AIGrenadeThrow s = prof.WorldPlayer.ModuleRunner.AddModule<AIGrenadeThrow>();
            prof.WorldPlayer.ModuleRunner.AddModule<AIBehaviorBase>();

            return prof;
        }
    }
}
