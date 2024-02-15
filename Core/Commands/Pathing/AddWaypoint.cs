using CommandSystem;
using PluginAPI.Core;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Pathing;

namespace SwiftNPCs.Core.Commands.Pathing
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddWaypoint : CommandBase
    {
        public override string[] GetAliases() => new string[] { "awp" };

        public override string GetCommandName() => "addwaypoint";

        public override string GetDescription() => "Creates a waypoint for an AI path.";

        public override PlayerPermissions[] GetPerms() => new PlayerPermissions[] { PlayerPermissions.RoundEvents };

        public override bool GetRequirePlayer() => true;

        public override bool PlayerBasedFunction(Player player, string[] args, out string result)
        {
            if (!TryGetArgument(args, 1, out string arg1) || !int.TryParse(arg1, out int index) || !PathManager.TryGetPath(index, out Path p))
            {
                result = "Please provide a valid path ID! ";

                return false;
            }

            result = "Added waypoint at " + player.Position + " with index " + (p.Waypoints.Count);

            p.AddWaypoint(player.Position);

            return true;
        }
    }
}
