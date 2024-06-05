namespace SwiftNPCs.Core.World.Modules
{
    public class AIFirearmStrafeShoot : AIFirearmShoot
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

            if (!Enabled || !HasTarget)
                return;

            Strafer.Tick();
        }
    }
}
