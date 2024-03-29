using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using SwiftAPI.Commands;

namespace SwiftNPCs.Core.Commands.Utility
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpawnGrenadeAI : CommandBase
    {
        public override string GetCommandName() => "spawngrenadeai";

        public override string GetDescription() => "Spawns a grenade AI.";

        public override string[] GetAliases() => ["spgai"];

        public override PlayerPermissions[] GetPerms() => [PlayerPermissions.PlayersManagement];

        public override bool GetRequirePlayer() => true;

        public override bool PlayerBasedFunction(Player player, string[] args, out string result)
        {
            Utilities.CreateGrenadeAI(RoleTypeId.ClassD, player.Position);

            result = "Created grenade AI! ";

            return true;
        }
    }
}
