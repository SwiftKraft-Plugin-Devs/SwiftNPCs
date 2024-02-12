using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Management;
using SwiftNPCs.Core.World.AIConditions;
using SwiftNPCs.Core.World.AIModules;

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
            AIPlayerProfile prof = AIPlayerManager.CreateAIPlayer(new AIDataProfileBase("Test Dummy"));

            prof.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.ClassD, RoleChangeReason.None, RoleSpawnFlags.None);
            prof.Position = player.Position;
            prof.Rotation = player.Rotation;

            AIStandIdle i = prof.WorldPlayer.ModuleRunner.AddModule<AIStandIdle>();
            AISimpleFollow f = prof.WorldPlayer.ModuleRunner.AddModule<AISimpleFollow>();

            i.AddTransition(f, new AIHasFollowTargetCondition() { Reverse = true }, new AIFindPlayerCondition() { SearchDistance = 10f });
            i.AddTransition(f, new AIHasFollowTargetCondition(), new AIFollowDistanceCondition() { Reverse = true, Distance = 3f });

            f.AddTransition(i, new AIHasFollowTargetCondition() { Reverse = true });
            f.AddTransition(i, new AIFollowDistanceCondition() { Distance = 1f });

            prof.WorldPlayer.ModuleRunner.ActivateModule(i);

            result = "Created AI Player! ";

            return true;
        }

        public override bool GetRequirePlayer() => true;
    }
}
