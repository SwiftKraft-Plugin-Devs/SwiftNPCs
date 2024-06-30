using MapGeneration;
using Mirror;
using PlayerRoles;
using PluginAPI.Core;
using SwiftNPCs.Core.Management;
using SwiftNPCs.Core.Pathing;
using SwiftNPCs.Core.World.AIModules;
using System.Collections.Generic;
using UnityEngine;
using static UnityStandardAssets.CinematicEffects.TonemappingColorGrading;

namespace SwiftNPCs
{
    public static class Utilities
    {
        public static readonly Dictionary<FacilityZone, RoomIdentifier[]> ZoneElevatorRooms = [];

        public static readonly Dictionary<RoleTypeId, int> ClassWeights = new()
        {
            { RoleTypeId.None, int.MinValue },
            { RoleTypeId.Spectator, int.MinValue },
            { RoleTypeId.Filmmaker, int.MinValue },
            { RoleTypeId.Overwatch, int.MinValue },
            { RoleTypeId.CustomRole, int.MinValue },
            { RoleTypeId.Scp079, int.MinValue },
            { RoleTypeId.Tutorial, 0 },
            { RoleTypeId.ClassD, 0 },
            { RoleTypeId.Scientist, 0 },
            { RoleTypeId.FacilityGuard, 1 },
            { RoleTypeId.NtfPrivate, 2 },
            { RoleTypeId.NtfSergeant, 3 },
            { RoleTypeId.NtfSpecialist, 3 },
            { RoleTypeId.NtfCaptain, 4 },
            { RoleTypeId.ChaosRifleman, 2 },
            { RoleTypeId.ChaosConscript, 2 },
            { RoleTypeId.ChaosMarauder, 3 },
            { RoleTypeId.ChaosRepressor, 4 },
            { RoleTypeId.Scp0492, 0 },
            { RoleTypeId.Scp049, 3 },
            { RoleTypeId.Scp096, 3 },
            { RoleTypeId.Scp173, 3 },
            { RoleTypeId.Scp106, 3 },
            { RoleTypeId.Scp939, 3 },
            { RoleTypeId.Scp3114, 0 },
        };

        public static Quaternion SmoothDampQuaternion(this Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
        {
            Vector3 c = current.eulerAngles;
            Vector3 t = target.eulerAngles;
            return Quaternion.Euler(
              Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
              Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
              Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
            );
        }

        public static bool TryGetComponentInParent<T>(this Component go, out T component) where T : Component
        {
            component = go.GetComponentInParent<T>();
            return component != null;
        }

        public static AIPlayerProfile CreateBasicAI(RoleTypeId role, Vector3 position, float enemyDistance = 50f, float followDistance = 10f, float startFollowDistance = 3f, float stopFollowDistance = 1f)
        {
            AIPlayerProfile prof = new AIDataProfileBase("Bot").CreateAIPlayer();
            prof.DisplayNickname = "Bot " + prof.Player.PlayerId;

            prof.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.None, RoleSpawnFlags.None);
            prof.Position = position;

            AIScanner i = prof.WorldPlayer.ModuleRunner.AddModule<AIScanner>();
            AIPathfinder p = prof.WorldPlayer.ModuleRunner.AddModule<AIPathfinder>();
            AIFollow f = prof.WorldPlayer.ModuleRunner.AddModule<AIFollow>();
            AIWander w = prof.WorldPlayer.ModuleRunner.AddModule<AIWander>();
            AIChaseEnemy ce = prof.WorldPlayer.ModuleRunner.AddModule<AIChaseEnemy>();
            AIFirearmShoot s = prof.WorldPlayer.ModuleRunner.AddModule<AIFirearmStrafeShoot>();
            AIGrenadeStrafeThrow g = prof.WorldPlayer.ModuleRunner.AddModule<AIGrenadeStrafeThrow>();
            AIItemConsume c = prof.WorldPlayer.ModuleRunner.AddModule<AIItemConsume>();
            AIItemPickup ip = prof.WorldPlayer.ModuleRunner.AddModule<AIItemPickup>();
            prof.WorldPlayer.ModuleRunner.AddModule<AIZombieModule>();
            prof.WorldPlayer.ModuleRunner.AddModule<AIScp049Module>();
            prof.WorldPlayer.ModuleRunner.AddModule<AIScp106Module>();
            prof.WorldPlayer.ModuleRunner.AddModule<AIScp939Module>();

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
            AIItemConsume c = prof.WorldPlayer.ModuleRunner.AddModule<AIItemConsume>();
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
            AIPathfinder p = prof.WorldPlayer.ModuleRunner.AddModule<AIPathfinder>();
            AIFollow f = prof.WorldPlayer.ModuleRunner.AddModule<AIFollow>();
            AIGrenadeThrow s = prof.WorldPlayer.ModuleRunner.AddModule<AIGrenadeThrow>();
            AIItemConsume c = prof.WorldPlayer.ModuleRunner.AddModule<AIItemConsume>();
            prof.WorldPlayer.ModuleRunner.AddModule<AIBehaviorBase>();

            return prof;
        }
    }
}
