namespace SwiftNPCs.Core.World.AIModules
{
    public class AIGrenadeStrafeThrow : AIGrenadeThrow
    {
        public Strafer Strafer { get; private set; }

        public override void Init()
        {
            base.Init();

            Strafer = new(Parent);
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            Strafer.Disable();
        }

        public override void Tick()
        {
            base.Tick();

            if (!Enabled || !Parent.HasEnemyTarget)
                return;

            Strafer.Tick();
        }
    }
}
