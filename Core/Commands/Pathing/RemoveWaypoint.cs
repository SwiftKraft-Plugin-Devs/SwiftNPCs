using CommandSystem;
using PluginAPI.Core;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Pathing;

namespace SwiftNPCs.Core.Commands.Pathing
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RemoveWaypoint : CommandBase
    {
        public override string[] GetAliases() => ["rwp", "deletewaypoint", "dwp"];

        public override string GetCommandName() => "removewaypoint";

        public override string GetDescription() => "Deletes a waypoint for an AI path.";

        public override PlayerPermissions[] GetPerms() => [PlayerPermissions.RoundEvents];

        public override bool GetRequirePlayer() => true;

        public override bool PlayerBasedFunction(Player player, string[] args, out string result)
        {
            if (!TryGetArgument(args, 1, out string arg1) || !int.TryParse(arg1, out int index) || !PathManager.TryGetPath(index, out Path p))
            {
                result = "Please provide a valid path ID! ";

                return false;
            }

            if (!TryGetArgument(args, 2, out string arg2) || !int.TryParse(arg2, out int wayIndex))
            {
                result = "Please provide a valid waypoint ID! ";

                return false;
            }

            p.RemoveWaypoint(wayIndex);

            result = "Removed waypoint with ID " + wayIndex + " from path ID " + index;

            return true;
        }
    }
}
