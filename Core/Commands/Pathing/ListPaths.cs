using CommandSystem;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Pathing;

namespace SwiftNPCs.Core.Commands.Pathing
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ListPaths : CommandBase
    {
        public override string[] GetAliases() => ["lpath"];

        public override string GetCommandName() => "listpath";

        public override string GetDescription() => "Lists all AI paths";

        public override PlayerPermissions[] GetPerms() => null;

        public override bool Function(string[] args, ICommandSender sender, out string result)
        {
            result = "All AI Paths: ";

            for (int i = 0; i < PathManager.Paths.Count; i++)
                result += "\n" + i + " - " + PathManager.Paths[i];

            return true;
        }
    }
}
