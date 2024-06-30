using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using PluginAPI.Core;
using SwiftNPCs.Core.World.Targetables;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIItemPickup : AIModuleBase
    {
        public float PickupDistance = 3f;

        public override bool Condition() => Parent.FollowTarget != null && Parent.FollowTarget is TargetableItem;

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
    }
}
