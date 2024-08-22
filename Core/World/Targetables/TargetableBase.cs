using InventorySystem.Items.Pickups;
using PlayerRoles.Ragdolls;
using PluginAPI.Core;
using PluginAPI.Core.Items;
using SwiftAPI.API.BreakableToys;
using UnityEngine;

namespace SwiftNPCs.Core.World.Targetables
{
    public abstract class TargetableBase
    {
        public abstract Vector3 GetPosition(AIModuleRunner module);

        public abstract Vector3 GetHeadPosition(AIModuleRunner module);

        public abstract bool IsAlive { get; }

        public abstract bool CanTarget(AIModuleRunner module, out bool cannotAttack);
        public abstract bool CanFollow(AIModuleRunner module);

        public static implicit operator Player(TargetableBase p) => p is TargetablePlayer pl ? (Player)pl : null;

        public static implicit operator TargetableBase(Player p) => new TargetablePlayer(p);

        public static implicit operator BreakableToyBase(TargetableBase t) => t is TargetableBreakableToy toy ? (BreakableToyBase)toy : null;

        public static implicit operator TargetableBase(BreakableToyBase t) => new TargetableBreakableToy(t);

        public static implicit operator ItemPickupBase(TargetableBase it) => it is TargetableItem item ? (ItemPickupBase)item : null;

        public static implicit operator TargetableBase(ItemPickupBase it) => new TargetableItem(it);

        public static implicit operator BasicRagdoll(TargetableBase r) => r is TargetableRagdoll ragdoll ? (BasicRagdoll)ragdoll : null;

        public static implicit operator TargetableBase(BasicRagdoll r) => new TargetableRagdoll(r);
    }
}
