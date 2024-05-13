using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using PluginAPI.Core;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIGrenadeThrow : AIModuleBase
    {
        public override float Duration => 2f;

        public Inventory Inventory => Parent.Inventory;

        public Player Target
        {
            get => Parent.EnemyTarget;
            set => Parent.EnemyTarget = value;
        }

        public bool HasLOS(out Vector3 pos) => Parent.HasLOSOnEnemy(out pos);

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

        public override bool Condition() => Parent.HasItemOfCategory(ItemCategory.Grenade) && delay <= 0f;

        public override void OnDisabled() { }

        public override void OnEnabled() { }

        public override void Tick()
        {
            if (delay > 0f)
                delay -= Time.fixedDeltaTime;

            if (!Enabled || !Parent.HasEnemyTarget)
                return;

            if (Parent.TryGetItem(out ThrowableItem throwable))
                Throw(throwable);
            else
                Parent.EquipItem<ThrowableItem>();

            if (HasLOS(out Vector3 pos))
                Parent.MovementEngine.LookPos = pos + Mathf.Clamp(Vector3.Distance(Parent.EnemyTarget.Position, Parent.CameraPosition) * DistanceOffsetScaler, 0f, DistanceOffsetCap) * Vector3.up;
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
