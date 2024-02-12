using PluginAPI.Core;
using PluginAPI.Events;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AISimpleFollow : AIModuleBase
    {
        public Vector3 Position => Parent.ReferenceHub.transform.position;

        public Player Target
        {
            get => Parent.FollowTarget;
            set => Parent.FollowTarget = value;
        }

        public override void End(AIModuleBase next)
        {
            Parent.MovementEngine.WishDir = Vector3.zero;
        }

        public override void Init() { }

        public override void Start(AIModuleBase prev) { }

        public override void Tick()
        {
            if (HasTarget)
            {
                Vector3 direction = GetMoveDirection();

                Parent.MovementEngine.WishDir = direction;
                Parent.MovementEngine.LookPos = Target.Position;
            }

            CheckTransitions();
        }

        public virtual Vector3 GetMoveDirection()
        {
            if (!HasTarget)
                return Vector3.zero;

            return (Target.Position - Position).normalized;
        }

        public override void ReceiveData<T>(T data)
        {
            if (!(data is Player p))
                return;

            Target = p;
        }

        public bool HasTarget => Parent.HasFollowTarget;
    }
}
