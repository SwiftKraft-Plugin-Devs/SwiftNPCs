using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Pickups;
using SwiftNPCs.Core.World.Targetables;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIItemPickup : AIModuleBase
    {
        public float PickupDistance = 3f;

        public override bool Condition() => !Parent.IsDisarmed(out _) && Parent.FollowTarget != null && Parent.FollowTarget is TargetableItem it && !InventoryFull(it);

        public override void Init()
        {
            Tags = [AIBehaviorBase.AutonomyTag];
        }

        public override void OnDisabled() { }

        public override void OnEnabled() { }

        public override void Tick()
        {
            if (!Enabled || Parent.FollowTarget is not TargetableItem item)
                return;

            Parent.MovementEngine.LookPos = item.GetHeadPosition(Parent);

            if (Parent.WithinDistance(Parent.FollowTarget, PickupDistance))
                PickupItem(item);
        }

        public ItemBase PickupItem(ItemPickupBase it)
        {
            ItemBase itm = Parent.Inventory.ServerAddItem(it.Info.ItemId, it.Info.Serial, it);
            it.DestroySelf();
            return itm;
        }

        public bool InventoryFull(ItemPickupBase item)
        {
            if (item is AmmoPickup ammo)
                return Parent.Inventory.GetCurAmmo(ammo.Info.ItemId) <= InventoryLimits.GetAmmoLimit(ammo.Info.ItemId, Parent.ReferenceHub);
            else
                return Parent.Inventory.UserInventory.Items.Count < 8;
        }
    }
}
