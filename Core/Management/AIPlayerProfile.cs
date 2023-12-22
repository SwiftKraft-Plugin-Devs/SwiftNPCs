using UnityEngine;

namespace SwiftNPCs.Core.Management
{
    public class AIPlayerProfile(int connId, ReferenceHub hub)
    {
        public readonly int ConnectionID = connId;

        public readonly ReferenceHub ReferenceHub = hub;

        public Vector3 Position
        {
            get
            {
                return ReferenceHub.transform.position;
            }
            set
            {
                ReferenceHub.transform.position = value;
            }
        }
    }
}
