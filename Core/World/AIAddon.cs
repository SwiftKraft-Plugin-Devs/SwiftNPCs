using UnityEngine;

namespace SwiftNPCs.Core.World
{
    public abstract class AIAddon : MonoBehaviour
    {
        public AIPlayer Core;

        public ReferenceHub ReferenceHub => Core.ReferenceHub;

        private void Awake()
        {
            Core = GetComponent<AIPlayer>();
            Init();
        }

        public virtual void Init() { }
    }
}