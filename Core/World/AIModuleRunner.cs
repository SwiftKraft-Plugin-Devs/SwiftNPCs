using InventorySystem;
using InventorySystem.Items;
using PlayerRoles;
using PluginAPI.Core;
using SwiftNPCs.Core.Management;
using SwiftNPCs.Core.World.AIModules;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwiftNPCs.Core.World
{
    public class AIModuleRunner : AIAddon
    {
        public const float RetargetTime = 2f;

        public readonly List<AIModuleBase> Modules = [];

        public Player EnemyTarget;
        public Player FollowTarget;

        public AIMovementEngine MovementEngine => Core.MovementEngine;

        public Inventory Inventory => ReferenceHub.inventory;

        public ItemBase CurrentItem => ReferenceHub.inventory.CurInstance;

        public PlayerRoleBase RoleBase => ReferenceHub.roleManager.CurrentRole;

        public Vector3 Position => ReferenceHub.transform.position;
        public Vector3 CameraPosition => ReferenceHub.PlayerCameraReference.position;

        public RoleTypeId Role => ReferenceHub.roleManager.CurrentRole.RoleTypeId;

        protected float RetargetTimer;
        protected float AimOffset;

        private void Start()
        {
            foreach (AIModuleBase module in Modules)
            {
                module.Parent = this;
                module.Init();
            }

            Core.OnDamage -= OnDamage;
            Core.OnRoleChange -= OnRoleChange;
            Core.OnDamage += OnDamage;
            Core.OnRoleChange += OnRoleChange;

            AimOffset = Random.Range(0.18f, 0.31f);
        }

        private void FixedUpdate()
        {
            if (RetargetTimer > 0f)
                RetargetTimer -= Time.fixedDeltaTime;

            foreach (AIModuleBase module in Modules)
                module.Tick();
        }

        private void OnDestroy()
        {
            Core.OnDamage -= OnDamage;
            Core.OnRoleChange -= OnRoleChange;
        }

        public virtual void OnDamage(Player attacker)
        {
            if (RetargetTimer <= 0f && attacker != null)
            {
                EnemyTarget = attacker;
                RetargetTimer = RetargetTime;
            }
        }

        public virtual void OnRoleChange(RoleTypeId role)
        {
            FollowTarget = null;
            EnemyTarget = null;
            RetargetTimer = 0f;

            if (role == RoleTypeId.None || role == RoleTypeId.Spectator)
                Core.Profile.Delete();
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

        public void ActivateModule(int index, bool status)
        {
            if (index < Modules.Count && index >= 0)
                ActivateModule(Modules[index], status);
        }

        public void ActivateModule(AIModuleBase module, bool status)
        {
            if (!Modules.Contains(module))
                return;

            module.Enabled = status;
        }

        public bool HasLOS(Player p, out Vector3 position, bool prioritizeHead = false)
        {
            if (p == null)
            {
                position = Vector3.zero;
                return false;
            }

            if (!prioritizeHead)
            {
                if (!Physics.Linecast(CameraPosition, p.Position, AIPlayer.MapLayerMask, QueryTriggerInteraction.Ignore))
                {
                    position = p.Position + Vector3.up * AimOffset;
                    return true;
                }
                else if (!Physics.Linecast(CameraPosition, p.Camera.position, AIPlayer.MapLayerMask, QueryTriggerInteraction.Ignore))
                {
                    position = p.Camera.position + Vector3.down * AimOffset;
                    return true;
                }
            }
            else
            {
                if (!Physics.Linecast(CameraPosition, p.Camera.position, AIPlayer.MapLayerMask, QueryTriggerInteraction.Ignore))
                {
                    position = p.Camera.position + Vector3.down * AimOffset;
                    return true;
                }
                else if (!Physics.Linecast(CameraPosition, p.Position, AIPlayer.MapLayerMask, QueryTriggerInteraction.Ignore))
                {
                    position = p.Position + Vector3.up * AimOffset;
                    return true;
                }
            }

            position = Vector3.zero;
            return false;
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

        public bool HasEnemyTarget
        {
            get
            {
                if (EnemyTarget == null)
                    return false;
                else if (EnemyTarget.ReferenceHub == null || EnemyTarget.GameObject == null || !EnemyTarget.IsAlive || EnemyTarget.Role.GetFaction() == Role.GetFaction())
                {
                    EnemyTarget = null;
                    return false;
                }

                return true;
            }
        }

        public bool HasLOSOnEnemy(out Vector3 pos, bool prioritizeHead = false) { pos = Vector3.zero; return HasEnemyTarget && HasLOS(EnemyTarget, out pos, prioritizeHead); }

        public bool IsEnemy(Player p) => 
    }
}
