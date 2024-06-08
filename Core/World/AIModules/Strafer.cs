using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class Strafer(AIModuleRunner parent, float max = 2f, float min = 1f)
    {
        public AIModuleRunner Parent { get; private set; } = parent;

        public float BacktrackDistance = 6f;
        public float StrafeTimerMax = max;
        public float StrafeTimerMin = min;

        private float timer = 0f;
        private StrafeState strafeState;

        public void Disable()
        {
            timer = 0f;
            Parent.MovementEngine.WishDir = Vector3.zero;
        }

        public void Tick() => Parent.Strafe(StrafeTimerMin, StrafeTimerMax, ref timer, ref strafeState, Parent.WithinDistance(Parent.EnemyTarget, BacktrackDistance));
    }
}
