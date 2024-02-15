using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using SwiftAPI.Commands;
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

            Utilities.CreateBasicAI(role, player.Position);

            result = "Created AI Player! ";

            return true;
        }

        public override bool GetRequirePlayer() => true;
    }
}
