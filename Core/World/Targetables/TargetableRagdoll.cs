using CustomPlayerEffects;
using PlayerRoles.Ragdolls;
using UnityEngine;

namespace SwiftNPCs.Core.World.Targetables
{
    public class TargetableRagdoll(BasicRagdoll d) : TargetableBase
    {
        public BasicRagdoll Ragdoll = d;

        public override bool IsAlive => Ragdoll != null;

        public override bool CanFollow(AIModuleRunner module) =>
            IsAlive
            && !module.IsDisarmed(out _)
            && !module.HasEffect<Blinded>();

        public override bool CanTarget(AIModuleRunner module, out bool cannotAttack)
        {
            cannotAttack = true;
            return false;
        }

        public override Vector3 GetHeadPosition(AIModuleRunner module) => Ragdoll.CenterPoint.transform.position;

        public override Vector3 GetPosition(AIModuleRunner module) => Ragdoll.CenterPoint.transform.position;

        public static implicit operator BasicRagdoll(TargetableRagdoll t) => t.Ragdoll;
        public static implicit operator TargetableRagdoll(BasicRagdoll d) => new(d);
    }
}
