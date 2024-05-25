using CommandSystem;
using PluginAPI.Core;
using PluginAPI.Core.Zones;
using SwiftAPI.Commands;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SwiftNPCs.Core.Commands.Utility
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class TestNavMesh : CommandBase
    {
        public override string[] GetAliases() => ["testnav", "tnm"];

        public override string GetCommandName() => "testnavmesh";

        public override string GetDescription() => "Tests the NavMesh";

        public override PlayerPermissions[] GetPerms() => null;

        public override bool Function(string[] args, ICommandSender sender, out string result)
        {
            Test();
            return base.Function(args, sender, out result);
        }

        public static void Test()
        {
            NavMeshPath path = new();
            FacilityRoom room1 = GetRandomActiveRoom();
            FacilityRoom room2 = GetRandomActiveRoom();

            Vector3 start = room1.Position;
            Log.Info(start.ToString());
            Vector3 end = room2.Position;
            Log.Info(end.ToString());

            Log.Info("Testing path calculation with rooms: \"" + room1.GameObject.name + "\" (activeSelf: " + room1.GameObject.activeSelf + "), \"" + room2.GameObject.name + "\" (activeSelf: " + room2.GameObject.activeSelf + ")...");
            NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path);
            Log.Info("Path calculation ended! Path corners: " + path.corners.Length);
        }

        public static FacilityRoom GetRandomActiveRoom()
        {
            List<FacilityRoom> list = new(Facility.Rooms);
            list.RemoveAll((r) => !r.GameObject.activeSelf);
            return list.RandomItem();
        }
    }
}
