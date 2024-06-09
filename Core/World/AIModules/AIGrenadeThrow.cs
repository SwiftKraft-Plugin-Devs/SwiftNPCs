using InventorySystem;
using InventorySystem.Items.ThrowableProjectiles;
using SwiftNPCs.Core.World.Targetables;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIGrenadeThrow : AIModuleBase
    {
        public override float Duration => 2f;

        public Inventory Inventory => Parent.Inventory;

        public TargetableBase Target
        {
            get => Parent.EnemyTarget;
            set => Parent.EnemyTarget = value;
        }

        public bool HasLOS(out Vector3 pos, out bool hasCollider) => Parent.HasLOSOnEnemy(out pos, out hasCollider);

        public float Delay = 1f;
        public float DistanceOffsetScaler = 0.25f;
        public float DistanceOffsetCap = 10f;
        public float RightClickDistance = 5f;

        public bool InfiniteGrenades = false;

        float delay;

        public override void Init()
        {
            Tags = [AIBehaviorBase.AttackerTag];
        }

        public override bool Condition() => delay <= 0f && Parent.HasEnemyTarget && Parent.HasItemOfCategory(ItemCategory.Grenade) && HasLOS(out _, out bool hasCollider) && !hasCollider;

        public override void OnDisabled() { }

        public override void OnEnabled() { }

        public override void Tick()
        {
            if (delay > 0f)
                delay -= Time.fixedDeltaTime;

            if (!Enabled || !Parent.HasEnemyTarget)
                return;

            bool hasLOS = HasLOS(out Vector3 pos, out bool hasCollider);

            if (hasLOS)
                Parent.MovementEngine.LookPos = pos + Mathf.Clamp(Vector3.Distance(Parent.EnemyTarget.GetPosition(Parent), Parent.CameraPosition) * DistanceOffsetScaler, 0f, DistanceOffsetCap) * Vector3.up;

            if (Parent.TryGetItem(out ThrowableItem throwable))
            {
                if (hasLOS && !hasCollider)
                    Throw(throwable);
            }
            else
                Parent.EquipItem<ThrowableItem>();
        }

        public void Throw(ThrowableItem item)
        {
            if (delay > 0f)
                return;

            delay = Delay;

            bool rClick = Parent.GetDistance(Parent.EnemyTarget) <= RightClickDistance;

            if (InfiniteGrenades)
                Parent.Core.Profile.Player.AddItem(item.ItemTypeId);

            item.ServerProcessInitiation();
            item.ServerProcessThrowConfirmation(!rClick, Parent.CameraPosition, Parent.ReferenceHub.transform.rotation, Vector3.zero);
        }
    }
}
