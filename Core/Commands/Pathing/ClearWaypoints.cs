using CommandSystem;
using PluginAPI.Core;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Pathing;

namespace SwiftNPCs.Core.Commands.Pathing
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ClearWaypoints : CommandBase
    {
        public override string[] GetAliases() => new string[] { "cwp" };

        public override string GetCommandName() => "clearwaypoints";

        public override string GetDescription() => "Clears all waypoints for an AI path.";

        public override PlayerPermissions[] GetPerms() => new PlayerPermissions[] { PlayerPermissions.RoundEvents };

        public override bool GetRequirePlayer() => true;

        public override bool PlayerBasedFunction(Player player, string[] args, out string result)
        {
            if (!TryGetArgument(args, 1, out string arg1) || !int.TryParse(arg1, out int index) || !PathManager.TryGetPath(index, out Path p))
            {
                result = "Please provide a valid path ID! ";

                return false;
            }

            result = "Cleared waypoints for path ID " + index;

            p.ClearWaypoints();

            return true;
        }
    }
}
