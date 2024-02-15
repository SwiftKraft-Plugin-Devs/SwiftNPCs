using CommandSystem;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Pathing;

namespace SwiftNPCs.Core.Commands.Pathing
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddPath : CommandBase
    {
        public override string[] GetAliases() => new string[] { "apath" };

        public override string GetCommandName() => "addpath";

        public override string GetDescription() => "Creates an AI path.";

        public override PlayerPermissions[] GetPerms() => new PlayerPermissions[] { PlayerPermissions.RoundEvents };

        public override bool Function(string[] args, ICommandSender sender, out string result)
        {
            PathManager.AddPath(out int index);

            result = "Created new path with ID: " + index;

            return true;
        }
    }
}
