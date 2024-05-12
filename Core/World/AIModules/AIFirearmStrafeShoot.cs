using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIFirearmStrafeShoot : AIFirearmShoot
    {
        public float StrafeTimerMax = 2f;
        public float StrafeTimerMin = 1f;

        private float timer = 0f;

        private StrafeState strafeState;

        public override void OnDisabled()
        {
            base.OnDisabled();

            timer = 0f;
            Parent.MovementEngine.WishDir = Vector3.zero;
        }

        public override void Tick()
        {
            base.Tick();

            if (!Enabled || !HasTarget)
                return;

            if (timer > 0f)
            {
                timer -= Time.fixedDeltaTime;

                Parent.MovementEngine.WishDir = strafeState switch
                {
                    StrafeState.Left => -Parent.transform.right,
                    StrafeState.Right => Parent.transform.right,
                    _ => Vector3.zero,
                };

                return;
            }

            timer = Random.Range(StrafeTimerMin, StrafeTimerMax);
            strafeState = Enum.GetValues(typeof(StrafeState)).ToArray<StrafeState>().RandomItem();
        }
    }

    public enum StrafeState : byte
    {
        Left = 0,
        Right = 1,
        Stop = 2
    }
}
