using CommandSystem;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Pathing;

namespace SwiftNPCs.Core.Commands.Pathing
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ClearPaths : CommandBase
    {
        public override string[] GetAliases() => ["cpath"];

        public override string GetCommandName() => "clearpaths";

        public override string GetDescription() => "Clears all AI paths.";

        public override PlayerPermissions[] GetPerms() => [PlayerPermissions.RoundEvents];

        public override bool Function(string[] args, ICommandSender sender, out string result)
        {
            PathManager.ClearPaths();

            result = "Cleared all AI paths. ";

            return true;
        }
    }
}
