using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Management;
using System;

namespace SwiftNPCs.Core.Commands.Utility
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpawnDummy : CommandBase
    {
        public override string GetCommandName() => "spawndummy";

        public override string GetDescription() => "Spawns a shooting dummy AI.";

        public override string[] GetAliases() => ["ssdai", "spdai"];

        public override PlayerPermissions[] GetPerms() => [PlayerPermissions.PlayersManagement];

        public override bool PlayerBasedFunction(Player player, string[] args, out string result)
        {
            RoleTypeId role = RoleTypeId.ClassD;

            ItemType item = ItemType.None;

            if (TryGetArgument(args, 1, out string arg1) && Enum.TryParse(arg1, out RoleTypeId r))
                role = r;

            if (TryGetArgument(args, 2, out string arg2) && Enum.TryParse(arg2, out ItemType i))
                item = i;

            AIPlayerProfile prof = Utilities.CreateShootingDummy(role, player.Position);
            prof.Player.AddItem(item);

            result = "Created Dummy Target! ";

            return true;
        }

        public override bool GetRequirePlayer() => true;
    }
}
