using SwiftNPCs.Core.World;
using UnityEngine;

namespace SwiftNPCs.Core.Pathing
{
    public class Node
    {
        public float ConnectionRange = 30f;

        public Vector3 Position { get; private set; }

        public Node[] Connected { get; private set; }

        public Node(Vector3 pos, float connectionRange)
        {
            this.Register();
            Position = pos;
            ConnectionRange = connectionRange;
            UpdateConnections();
        }

        public void UpdateConnections()
        {
            Connected = this.GetNodes(ConnectionRange, HasLOS);
        }

        public bool HasLOS(Node other) => !Physics.Linecast(Position, other.Position, AIPlayer.MapLayerMask, QueryTriggerInteraction.Ignore);
    }
}
