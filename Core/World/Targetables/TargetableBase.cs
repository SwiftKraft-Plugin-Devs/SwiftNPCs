using PluginAPI.Core;
using PluginAPI.Core.Items;
using SwiftAPI.API.BreakableToys;
using UnityEngine;

namespace SwiftNPCs.Core.World.Targetables
{
    public abstract class TargetableBase
    {
        public abstract Vector3 Position { get; }
        public abstract Vector3 HeadPosition { get; }

        public abstract bool IsAlive { get; }

        public abstract bool CanTarget(AIModuleRunner module, out bool cannotAttack);
        public abstract bool CanFollow(AIModuleRunner module);

        public static implicit operator Player(TargetableBase p)
        {
            if (p is TargetablePlayer pl)
                return pl;
            return null;
        }

        public static implicit operator TargetableBase(Player p) => new TargetablePlayer(p);

        public static implicit operator BreakableToyBase(TargetableBase t)
        {
            if (t is TargetableBreakableToy toy)
                return toy;
            return null;
        }

        public static implicit operator TargetableBase(BreakableToyBase t) => new TargetableBreakableToy(t);

        public static implicit operator ItemPickup(TargetableBase it)
        {
            if (it is TargetableItem item)
                return item;
            return null;
        }

        public static implicit operator TargetableBase(ItemPickup it) => new TargetableItem(it);
    }
}
