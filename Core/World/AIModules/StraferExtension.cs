using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwiftNPCs.Core.World.AIModules
{
    public static class StraferExtension
    {
        public static void Strafe(this AIModuleRunner parent, float strafeTimerMin, float strafeTimerMax, ref float timer, ref StrafeState strafeState, bool backtrack = false)
        {
            if (timer > 0f)
            {
                timer -= Time.fixedDeltaTime;

                if (!backtrack)
                    parent.MovementEngine.WishDir = strafeState switch
                    {
                        StrafeState.Left => -parent.transform.right,
                        StrafeState.Right => parent.transform.right,
                        _ => default,
                    };
                else
                    parent.MovementEngine.WishDir = -parent.transform.forward;
                
                return;
            }

            timer = Random.Range(strafeTimerMin, strafeTimerMax);
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
