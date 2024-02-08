using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Management;

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

            result = "Created AI Player! ";

            return true;
        }

        public override bool GetRequirePlayer() => true;
    }
}
