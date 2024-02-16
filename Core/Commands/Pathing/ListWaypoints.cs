using CommandSystem;
using PluginAPI.Core;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Pathing;

namespace SwiftNPCs.Core.Commands.Pathing
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ListWaypoints : CommandBase
    {
        public override string[] GetAliases() => ["lwp"];

        public override string GetCommandName() => "listwaypoints";

        public override string GetDescription() => "Lists all waypoints for an AI path.";

        public override PlayerPermissions[] GetPerms() => null;

        public override bool GetRequirePlayer() => true;

        public override bool PlayerBasedFunction(Player player, string[] args, out string result)
        {
            if (!TryGetArgument(args, 1, out string arg1) || !int.TryParse(arg1, out int index) || !PathManager.TryGetPath(index, out Path p))
            {
                result = "Please provide a valid path ID! ";

                return false;
            }

            result = "All waypoints for path ID " + index;

            for (int i = 0; i < p.Waypoints.Count; i++)
                result += "\n" + i + " - " + p.Waypoints[i];

            return true;
        }
    }
}
