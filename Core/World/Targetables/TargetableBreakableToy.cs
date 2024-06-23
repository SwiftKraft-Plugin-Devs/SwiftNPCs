using PlayerRoles;
using SwiftAPI.API.BreakableToys;
using UnityEngine;

namespace SwiftNPCs.Core.World.Targetables
{
    public class TargetableBreakableToy(BreakableToyBase toy) : TargetableBase
    {
        public readonly BreakableToyBase Toy = toy;

        public override Vector3 GetPosition(AIModuleRunner module) => Toy.Toy.Position;

        public override Vector3 GetHeadPosition(AIModuleRunner module) => Toy.Toy.Position;

        public override bool IsAlive => Toy != null && Toy.Toy != null;

        public override bool CanFollow(AIModuleRunner module) => false;

        public override bool CanTarget(AIModuleRunner module, out bool cannotAttack)
        {
            cannotAttack = false;
            return
                Toy.MaxHealth >= 0f
                && module.Role.GetFaction() != Toy.Faction
                && module.HasLOS(this, out _, out cannotAttack);
        }

        public static implicit operator BreakableToyBase(TargetableBreakableToy t) => t.Toy;
        public static implicit operator TargetableBreakableToy(BreakableToyBase t)
        {
            if (t != null)
                return new(t);
            return null;
        }
    }
}
