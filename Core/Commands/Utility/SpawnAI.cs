using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Management;
using SwiftNPCs.Core.World.AIConditions;
using SwiftNPCs.Core.World.AIModules;
using System;

namespace SwiftNPCs.Core.Commands.Utility
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpawnAI : CommandBase
    {
        public override string GetCommandName() => "spawnai";

        public override string GetDescription() => "Spawns a basic AI.";

        public override string[] GetAliases() => new string[] { "sai", "spai" };

        public override PlayerPermissions[] GetPerms() => new PlayerPermissions[] { PlayerPermissions.PlayersManagement };

        public override bool PlayerBasedFunction(Player player, string[] args, out string result)
        {
            RoleTypeId role = RoleTypeId.ClassD;

            if (TryGetArgument(args, 1, out string arg1) && Enum.TryParse(arg1, out RoleTypeId r))
                role = r;

            AIPlayerProfile prof = AIPlayerManager.CreateAIPlayer(new AIDataProfileBase("Bot"));
            prof.DisplayNickname = "Bot " + prof.Player.PlayerId;

            prof.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.None, RoleSpawnFlags.None);
            prof.Position = player.Position;
            prof.Rotation = player.Rotation;

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

            result = "Created AI Player! ";

            return true;
        }

        public override bool GetRequirePlayer() => true;
    }
}
