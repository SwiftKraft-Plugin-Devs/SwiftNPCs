namespace SwiftNPCs.Core.World.AIModules
{
    public class AIConsumeStrafeItem : AIConsumeItem
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

            if (!Enabled)
                return;

            Strafer.Tick();
        }
    }
}
