using CommandSystem;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Pathing;

namespace SwiftNPCs.Core.Commands.Pathing
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RemovePath : CommandBase
    {
        public override string[] GetAliases() => ["rpath", "deletepath", "dpath"];

        public override string GetCommandName() => "removepath";

        public override string GetDescription() => "Deletes an AI path.";

        public override PlayerPermissions[] GetPerms() => [PlayerPermissions.RoundEvents];

        public override bool Function(string[] args, ICommandSender sender, out string result)
        {
            if (!TryGetArgument(args, 1, out string arg1) || !int.TryParse(arg1, out int index))
            {
                result = "Please provide a path ID! ";

                return false;
            }

            PathManager.RemovePath(index);

            result = "Removed path with ID: " + index;

            return true;
        }
    }
}
