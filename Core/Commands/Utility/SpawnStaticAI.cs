﻿using PlayerRoles;
using PluginAPI.Core;
using SwiftAPI.Commands;
using SwiftNPCs.Core.Management;
using SwiftNPCs.Core.Pathing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftNPCs.Core.Commands.Utility
{
    public class SpawnStaticAI : CommandBase
    {
        public override string GetCommandName() => "spawnstaticai";

        public override string GetDescription() => "Spawns a static AI.";

        public override string[] GetAliases() => ["spsai"];

        public override PlayerPermissions[] GetPerms() => [PlayerPermissions.PlayersManagement];

        public override bool PlayerBasedFunction(Player player, string[] args, out string result)
        {
            RoleTypeId role = RoleTypeId.ClassD;

            ItemType item = ItemType.None;

            if (TryGetArgument(args, 1, out string arg1) && Enum.TryParse(arg1, out RoleTypeId r))
                role = r;

            if (TryGetArgument(args, 2, out string arg2) && Enum.TryParse(arg2, out ItemType i))
                item = i;

            AIPlayerProfile prof = Utilities.CreateStaticAI(role, player.Position);
            prof.Player.AddItem(item);

            result = "Created AI Static Player! ";

            return true;
        }

        public override bool GetRequirePlayer() => true;
    }
}
