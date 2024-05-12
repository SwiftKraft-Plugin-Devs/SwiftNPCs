using System;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftNPCs.Core.Pathing
{
    public static class NodeManager
    {
        public static readonly List<Node> Nodes = [];

        public static void Register(this Node node)
        {
            Nodes.Add(node);
        }

        public static float GetDistance(this Node node, Vector3 pos) => Vector3.Distance(node.Position, pos);

        public static Node[] GetNodes(this Node node, float distance, Predicate<Node> condition = null)
        {
            List<Node> nodes = [];
            foreach (Node n in Nodes)
                if (Vector3.Distance(n.Position, node.Position) <= distance && (condition == null || condition.Invoke(n)))
                    nodes.Add(n);
            return [.. nodes];
        }

        public static Node GetClosestNode(this Vector3 pos)
        {
            Node closest = null;
            foreach (Node n in Nodes)
                if (closest == null || n.GetDistance(pos) < closest.GetDistance(pos))
                    closest = n;
            return closest;
        }
    }
}
