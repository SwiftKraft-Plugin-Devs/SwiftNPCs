using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using SwiftNPCs.Core.Management;
using UnityEngine;

namespace SwiftNPCs.Core.World
{
    public class AIPlayer : MonoBehaviour
    {
        /// <summary>
        /// Used for raycasting to check for walls, ground, etc. 
        /// This is just a more accessible and less confusing way of accessing FpcStateProcessor.Mask.
        /// </summary>
        public static LayerMask MapLayerMask => FpcStateProcessor.Mask;

        public AIPlayerProfile Profile;

        public PlayerRoleBase CurrentRole => Profile.ReferenceHub.roleManager.CurrentRole;

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

        private void Start()
        {
            Log.Info("Spawned");

            Profile.ReferenceHub.nicknameSync.DisplayName = Profile.Data.Name;

            gameObject.AddComponent<AIMovementEngine>();
        }
    }
}
