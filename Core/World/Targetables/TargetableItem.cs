using CustomPlayerEffects;
using InventorySystem.Items.Pickups;
using UnityEngine;
using UnityEngine.AI;

namespace SwiftNPCs.Core.World.Targetables
{
    public class TargetableItem(ItemPickupBase item) : TargetableBase
    {
        public readonly ItemPickupBase Item = item;

        public override Vector3 GetPosition(AIModuleRunner module)
        {
            if (NavMesh.SamplePosition(Item.transform.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                return hit.position;
            return Item.transform.position;
        }

        public override Vector3 GetHeadPosition(AIModuleRunner module) => Item.transform.position;

        public override bool IsAlive => Item != null;

        public override bool CanFollow(AIModuleRunner module) =>
            IsAlive
            && !module.IsDisarmed(out _)
            && !module.HasEffect<Blinded>()
            && module.WithinDistance(this, module.ItemDistance);

        public override bool CanTarget(AIModuleRunner module, out bool cannotAttack)
        {
            cannotAttack = true;
            return false;
        }

        public static implicit operator ItemPickupBase(TargetableItem t) => t.Item;
        public static implicit operator TargetableItem(ItemPickupBase t) => new(t);
    }
}
