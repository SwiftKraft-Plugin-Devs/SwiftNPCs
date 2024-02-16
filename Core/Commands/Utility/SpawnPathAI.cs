using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Pathing;
using System;

namespace SwiftNPCs.Core.Commands.Utility
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpawnPathAI : CommandBase
    {
        public override string GetCommandName() => "spawnpathai";

        public override string GetDescription() => "Spawns a path AI.";

        public override string[] GetAliases() => ["sptai"];

        public override PlayerPermissions[] GetPerms() => [PlayerPermissions.PlayersManagement];

        public override bool PlayerBasedFunction(Player player, string[] args, out string result)
        {
            if (!TryGetArgument(args, 1, out string arg1) || !int.TryParse(arg1, out int pathID) || !PathManager.TryGetPath(pathID, out Path p))
            {
                result = "Please input a valid path ID! ";

                return false;
            }

            RoleTypeId role = RoleTypeId.ClassD;

            if (TryGetArgument(args, 2, out string arg2) && Enum.TryParse(arg2, out RoleTypeId r))
                role = r;

            Utilities.CreatePathAI(role, player.Position, p);

            result = "Created AI Path Player! ";

            return true;
        }

        public override bool GetRequirePlayer() => true;
    }
}
