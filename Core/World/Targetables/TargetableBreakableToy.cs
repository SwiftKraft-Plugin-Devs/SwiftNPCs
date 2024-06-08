using SwiftAPI.API.BreakableToys;
using UnityEngine;

namespace SwiftNPCs.Core.World.Targetables
{
    public class TargetableBreakableToy(BreakableToyBase toy) : TargetableBase
    {
        public readonly BreakableToyBase Toy = toy;

        public override Vector3 Position => Toy.Toy.Position;
        public override Vector3 HeadPosition => Toy.Toy.Position;

        public override bool IsAlive => Toy != null;

        public override bool CanFollow(AIModuleRunner module) => false;

        public override bool CanTarget(AIModuleRunner module, out bool cannotAttack)
        {
            cannotAttack = false;
            return true;
        }

        public static implicit operator BreakableToyBase(TargetableBreakableToy t) => t.Toy;
        public static implicit operator TargetableBreakableToy(BreakableToyBase t) => new(t);
    }
}
