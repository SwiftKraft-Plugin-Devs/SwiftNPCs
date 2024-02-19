using CommandSystem;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Management;
using System.Collections.Generic;

namespace SwiftNPCs.Core.Commands.Utility
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ClearAI : CommandBase
    {
        public override string[] GetAliases() => ["cai"];

        public override string GetCommandName() => "clearais";

        public override string GetDescription() => "Disconnects all AI players.";

        public override PlayerPermissions[] GetPerms() => [PlayerPermissions.PlayersManagement];

        public override bool Function(string[] args, ICommandSender sender, out string result)
        {
            result = "Disconnected " + AIPlayerManager.Registered.Count + " AI Players. ";

            AIPlayerProfile[] profs = [.. AIPlayerManager.Registered];
            foreach (AIPlayerProfile prof in profs)
                prof.Delete();

            return true;
        }
    }
}
