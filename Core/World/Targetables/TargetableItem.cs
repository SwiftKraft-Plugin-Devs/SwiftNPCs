using PluginAPI.Core;
using PluginAPI.Core.Items;
using SwiftAPI.API.BreakableToys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftNPCs.Core.World.Targetables
{
    public class TargetableItem(ItemPickup item) : TargetableBase
    {
        public readonly ItemPickup Item = item;

        public override Vector3 Position => Item.Position;

        public override Vector3 HeadPosition => Item.Position;

        public override bool IsAlive => true;

        public override bool CanFollow(AIModuleRunner module) =>
            !module.IsDisarmed(out _)
            && module.WithinDistance(this, module.ItemDistance);

        public override bool CanTarget(AIModuleRunner module, out bool cannotAttack)
        {
            cannotAttack = true;
            return false;
        }

        public static implicit operator ItemPickup(TargetableItem t) => t.Item;
        public static implicit operator TargetableItem(ItemPickup t) => new(t);
    }
}
