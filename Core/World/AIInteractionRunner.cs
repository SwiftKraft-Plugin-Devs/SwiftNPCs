using InventorySystem;
using InventorySystem.Items;
using PlayerRoles;
using SwiftNPCs.Core.World.AIInteractionModules;
using System;
using System.Collections.Generic;

namespace SwiftNPCs.Core.World
{
    public class AIInteractionRunner : AIAddon
    {
        public readonly List<AIInteractionModuleBase> CurrentModules = new List<AIInteractionModuleBase>();

        public Inventory Inventory => ReferenceHub.inventory;

        public ItemBase CurrentItem => ReferenceHub.inventory.CurInstance;

        public PlayerRoleBase RoleBase => ReferenceHub.roleManager.CurrentRole;

        public RoleTypeId Role => ReferenceHub.roleManager.CurrentRole.RoleTypeId;

        private void Start()
        {
            foreach (AIInteractionModuleBase module in CurrentModules)
            {
                module.Parent = this;
                module.Init();
            }
        }

        private void FixedUpdate()
        {
            foreach (AIInteractionModuleBase module in CurrentModules)
                module.Tick();
        }

        public T AddModule<T>() where T : AIInteractionModuleBase
        {
            T obj = Activator.CreateInstance<T>();
            CurrentModules.Add(obj);
            obj.Parent = this;
            obj.Init();
            return obj;
        }

        public T GetModule<T>() where T : AIInteractionModuleBase
        {
            foreach (AIInteractionModuleBase obj in CurrentModules)
                if (obj is T t)
                    return t;

            return null;
        }

        public bool TryGetModule<T>(out T output) where T : AIInteractionModuleBase
        {
            output = GetModule<T>();
            if (output == null)
                return false;
            return true;
        }

        public void RemoveModule(AIInteractionModuleBase module)
        {
            CurrentModules.Remove(module);
        }
    }
}
