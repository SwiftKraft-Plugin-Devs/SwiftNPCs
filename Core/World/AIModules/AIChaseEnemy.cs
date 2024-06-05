using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIChaseEnemy : AIModuleBase
    {
        AIPathfinder Pathfinder;

        Vector3 lastEnemyPosition;

        public override void Init()
        {
            Pathfinder = Parent.GetModule<AIPathfinder>();
            Parent.OnLostEnemy += OnLostEnemy;
            Tags = [AIBehaviorBase.AggroTag];
        }

        private void OnLostEnemy(PluginAPI.Core.Player enemy, Vector3 pos)
        {
            lastEnemyPosition = pos;
        }

        public override void OnDisabled() { }

        public override void OnEnabled()
        {
            Pathfinder.SetDestination(lastEnemyPosition);
            Parent.MovementEngine.State = PlayerRoles.FirstPersonControl.PlayerMovementState.Sprinting;
        }

        public override void Tick() { }
    }
}
