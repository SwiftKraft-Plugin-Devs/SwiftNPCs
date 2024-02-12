using InventorySystem;
using InventorySystem.Items;
using PlayerRoles;
using PluginAPI.Core;
using SwiftNPCs.Core.World.AIModules;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace SwiftNPCs.Core.World
{
    public class AIModuleRunner : AIAddon
    {
        public readonly List<AIModuleBase> Modules = new List<AIModuleBase>();

        public AIModuleBase CurrentModule;

        public Player EnemyTarget;
        public Player FollowTarget;

        public AIMovementEngine MovementEngine => Core.MovementEngine;

        public Inventory Inventory => ReferenceHub.inventory;

        public ItemBase CurrentItem => ReferenceHub.inventory.CurInstance;

        public PlayerRoleBase RoleBase => ReferenceHub.roleManager.CurrentRole;

        public Vector3 Position => ReferenceHub.transform.position;

        public RoleTypeId Role => ReferenceHub.roleManager.CurrentRole.RoleTypeId;

        private void Start()
        {
            foreach (AIModuleBase module in Modules)
            {
                module.Parent = this;
                module.Init();
            }
        }

        private void FixedUpdate()
        {
            CurrentModule?.Tick();
        }

        public T AddModule<T>() where T : AIModuleBase
        {
            T obj = Activator.CreateInstance<T>();
            Modules.Add(obj);
            obj.Parent = this;
            obj.Init();
            return obj;
        }

        public T GetModule<T>() where T : AIModuleBase
        {
            foreach (AIModuleBase obj in Modules)
                if (obj is T t)
                    return t;

            return null;
        }

        public bool TryGetModule<T>(out T output) where T : AIModuleBase
        {
            output = GetModule<T>();
            return output != null;
        }

        public void RemoveModule(AIModuleBase module)
        {
            Modules.Remove(module);
        }

        public void ActivateModule(int index)
        {
            if (index < Modules.Count && index >= 0)
                ActivateModule(Modules[index]);
        }

        public void ActivateModule(AIModuleBase module)
        {
            if (!Modules.Contains(module))
                return;

            AIModuleBase temp = CurrentModule;
            CurrentModule = module;
            temp?.End(CurrentModule);
            CurrentModule.Start(temp);
        }

        public bool HasLOS(Player p)
        {
            if (p == null)
                return true;

            return !Physics.Linecast(Position, p.Position, AIPlayer.MapLayerMask, QueryTriggerInteraction.Ignore);
        }

        public bool HasFollowTarget
        {
            get
            {
                if (FollowTarget == null)
                    return false;
                else if (!FollowTarget.IsAlive)
                {
                    FollowTarget = null;
                    return false;
                }

                return true;
            }
        }
    }
}
