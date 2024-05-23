using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Management;
using System;

namespace SwiftNPCs.Core.Commands.Utility
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpawnAI : CommandBase
    {
        public override string GetCommandName() => "spawnai";

        public override string GetDescription() => "Spawns a basic AI.";

        public override string[] GetAliases() => ["sai", "spai"];

        public override PlayerPermissions[] GetPerms() => [PlayerPermissions.PlayersManagement];

        public override bool PlayerBasedFunction(Player player, string[] args, out string result)
        {
            RoleTypeId role = RoleTypeId.ClassD;
            ItemType item = ItemType.None;

            if (TryGetArgument(args, 1, out string arg1) && Enum.TryParse(arg1, out RoleTypeId r))
                role = r;

            if (TryGetArgument(args, 2, out string arg2) && Enum.TryParse(arg2, out ItemType i))
                item = i;

            AIPlayerProfile prof = Utilities.CreateBasicAI(role, player.Position);
            prof.Player.AddItem(item);

            result = "Created AI Player! ";

            return true;
        }

        public override bool GetRequirePlayer() => true;
    }
}
