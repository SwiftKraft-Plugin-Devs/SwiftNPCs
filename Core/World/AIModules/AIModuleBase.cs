namespace SwiftNPCs.Core.World.AIModules
{
    public abstract class AIModuleBase
    {
        public virtual float Duration => 5f;

        protected string[] Tags = [];

        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled == value)
                    return;

                enabled = value;

                if (enabled)
                    OnEnabled();
                else
                    OnDisabled();
            }
        }

        public AIModuleRunner Parent { get; set; }

        private bool enabled = true;

        public abstract void Init();

        public abstract void OnEnabled();

        public abstract void OnDisabled();

        public abstract void Tick();

        public virtual bool Condition() => true;

        public bool HasTag(string tag) => Tags.Contains(tag);
    }
}
