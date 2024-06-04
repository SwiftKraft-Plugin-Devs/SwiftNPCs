using PlayerRoles;

namespace SwiftNPCs.Core.World.AIModules
{
    public abstract class AIRoleModuleBase : AIModuleBase
    {
        public abstract RoleTypeId[] Roles { get; }

        protected bool Status { get; private set; }

        public override bool Condition() => Status;

        public override void Init()
        {
            RoleChange(Parent.Role);
            Parent.OnRoleChange += RoleChange;
        }

        private void RoleChange(RoleTypeId role) => Status = Roles.Contains(role);
    }
}
