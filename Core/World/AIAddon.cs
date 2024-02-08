using UnityEngine;

namespace SwiftNPCs.Core.World
{
    public abstract class AIAddon : MonoBehaviour
    {
        protected AIPlayer core;

        private void Awake()
        {
            core = GetComponent<AIPlayer>();
            Init();
        }

        public virtual void Init() { }
    }
}