using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using SwiftNPCs.Core.Management;
using UnityEngine;

namespace SwiftNPCs.Core.World
{
    public delegate void OnAIPlayerDamage(Player attacker);
    public delegate void OnAIPlayerChangeRole(RoleTypeId role);

    public class AIPlayer : MonoBehaviour
    {
        /// <summary>
        /// Used for raycasting to check for walls, ground, etc. 
        /// This is just a more accessible and less confusing way of accessing FpcStateProcessor.Mask.
        /// </summary>
        public static LayerMask MapLayerMask => FpcStateProcessor.Mask;

        public AIPlayerProfile Profile;

        public PlayerRoleBase CurrentRole => ReferenceHub.roleManager.CurrentRole;

        public ReferenceHub ReferenceHub => Profile.ReferenceHub;

        public IFpcRole FirstPersonController
        {
            get
            {
                if (CurrentRole is IFpcRole fpc)
                    return fpc;
                else
                    return null;
            }
        }

        public OnAIPlayerDamage OnDamage;
        public OnAIPlayerChangeRole OnRoleChange;

        public AIMovementEngine MovementEngine;
        public AIModuleRunner ModuleRunner;

        private void Awake()
        {
            MovementEngine = gameObject.AddComponent<AIMovementEngine>();
            ModuleRunner = gameObject.AddComponent<AIModuleRunner>();
        }

        public void Damage(Player attacker) => OnDamage?.Invoke(attacker);
        public void RoleChange(RoleTypeId role) => OnRoleChange?.Invoke(role);
    }
}
