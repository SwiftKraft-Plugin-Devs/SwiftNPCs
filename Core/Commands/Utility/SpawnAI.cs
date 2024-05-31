using CommandSystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using PlayerRoles;
using PluginAPI.Core;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Management;
using System;
using System.Collections.Generic;

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
            List<ItemType> items = [];

            if (TryGetArgument(args, 1, out string arg1) && Enum.TryParse(arg1, out RoleTypeId r))
                role = r;

            if (args.Length > 2)
            {
                List<string> item = [.. args];
                item.RemoveRange(0, 2);
                foreach (string s in item)
                {
                    if (Enum.TryParse(s, out ItemType i))
                        items.Add(i);
                }
            }

            AIPlayerProfile prof = Utilities.CreateBasicAI(role, player.Position);

            result = "Created AI Player! Inventory: ";

            foreach (ItemType i in items)
            {
                ItemBase ite = prof.Player.AddItem(i);
                if (ite is Firearm f)
                    f.Status = new FirearmStatus(f.AmmoManagerModule.MaxAmmo, f.Status.Flags, AttachmentsUtils.GetRandomAttachmentsCode(i));
            }

            result += string.Join(", ", items);

            return true;
        }

        public override bool GetRequirePlayer() => true;
    }
}
