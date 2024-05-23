﻿using CustomPlayerEffects;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Doors;
using SwiftAPI.API.ServerVariables;
using SwiftNPCs.Core.Management;
using SwiftNPCs.Core.World.AIModules;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwiftNPCs.Core.World
{
    public class AIModuleRunner : AIAddon
    {
        public static bool Frozen => ServerVariableManager.TryGetVar(FrozenVar, out ServerVariable v) && bool.TryParse(v.Value, out bool b) && b;

        public const string FrozenVar = "freezenpcs";
        public const float RetargetTime = 0.25f;

        public float DotViewMinimum = 0.25f;

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

        public KeycardPermissions Permissions
        {
            get
            {
                if (CurrentItem == null || CurrentItem is not KeycardItem item)
                    return KeycardPermissions.None;
                return item.Permissions;
            }
        }

        public HealthStat Health
        {
            get
            {
                if (!ReferenceHub.playerStats.TryGetModule(out HealthStat stat))
                    return null;
                return stat;
            }
        }

        public float RetargetTimer;
        public bool CanStealth = false;

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
            if (Frozen)
                return;

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

        public void ChangeModule(bool status)
        {
            foreach (AIModuleBase mod in Modules)
                ChangeModule(mod, status);
        }

        public bool ChangeModule(int index, bool status)
        {
            if (index < Modules.Count && index >= 0)
                return ChangeModule(Modules[index], status);
            return false;
        }

        public bool ChangeModule(AIModuleBase module, bool status)
        {
            if (status && !module.Condition())
                return false;
            module.Enabled = status;
            return true;
        }

        public void ChangeModule(string tag, bool status)
        {
            foreach (AIModuleBase mod in Modules)
                if (mod.HasTag(tag))
                    ChangeModule(mod, status);
        }

        public AIModuleBase[] GetModulesByTag(string tag) => [.. Modules.FindAll((a) => a.HasTag(tag))];

        public AIModuleBase[] GetModulesByTagRandom(string tag) => GetModulesByTag(tag).OrderBy(_ => Random.Range(int.MinValue, int.MaxValue)).ToArray();

        public AIModuleBase GetRandomModuleByTag(string tag) => Modules.FindAll((a) => a.HasTag(tag)).RandomItem();

        public bool ActivateRandomModuleByTag(string tag, out AIModuleBase mod, bool single = false)
        {
            AIModuleBase[] mods = GetModulesByTagRandom(tag);
            if (single)
                ChangeModule(tag, false);

            foreach (AIModuleBase m in mods)
                if (ChangeModule(m, true))
                {
                    mod = m;
                    return true;
                }
            mod = null;
            return false;
        }

        public bool HasItemOfCategory(ItemCategory cat)
        {
            foreach (ItemBase item in Inventory.UserInventory.Items.Values)
                if (item.Category == cat)
                    return true;
            return false;
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

        public bool IsInView(Player p) => !CanStealth || GetDotProduct(p.Position) >= DotViewMinimum;

        public bool EquipItem<T>(Predicate<T> filter) where T : ItemBase
        {
            if (!HasItem(out T t, filter))
                return false;

            EquipItem(t.ItemSerial);
            return true;
        }

        public bool EquipItem<T>() where T : ItemBase => EquipItem<T>(null);

        public void EquipItem(ushort serial)
        { 
            Inventory.CurInstance = null;
            Inventory.ServerSelectItem(serial); 
        }

        public bool HasItem<T>(out T it, Predicate<T> filter) where T : ItemBase
        {
            foreach (ItemBase item in Inventory.UserInventory.Items.Values)
                if (item is T t && (filter == null || filter.Invoke(t)))
                {
                    it = t;
                    return true;
                }
            it = null;
            return false;
        }

        public bool HasItem<T>(out T it) where T : ItemBase => HasItem(out it, null);

        public bool HasItem(ushort serial)
        {
            foreach (ItemBase item in Inventory.UserInventory.Items.Values)
                if (item.ItemSerial == serial)
                    return true;
            return false;
        }

        public bool HasItem<T>() where T : ItemBase => HasItem<T>(out _);

        public T GetItem<T>() where T : ItemBase
        {
            if (CurrentItem is T t)
                return t;
            return null;
        }

        public bool TryGetItem<T>(out T item) where T : ItemBase
        {
            item = GetItem<T>();
            return item != null;
        }

        public List<T> GetAllItems<T>() where T : ItemBase
        {
            List<T> list = [];
            foreach (ItemBase item in Inventory.UserInventory.Items.Values)
                if (item is T t)
                    list.Add(t);
            return list;
        }

        /// <summary>
        /// Using Vector3.Dot().
        /// </summary>
        /// <param name="position"></param>
        /// <returns>1f if looking exactly at the object, 0f if perpendicular and -1f if opposite.</returns>
        public float GetDotProduct(Vector3 position) => MovementEngine.GetDotProduct(position);

        public bool TrySetDoor(FacilityDoor door, bool state)
        {
            if (!CanAccessDoor(door))
                EquipItem<KeycardItem>((i) => CheckPermissions(i.Permissions, door));

            float st = door.OriginalObject.GetExactState();
            if (!CanAccessDoor(door) || door.OriginalObject.NetworkTargetState == state || (st > 0f && st < 1f))
                return false;

            door.OriginalObject.NetworkTargetState = state;
            return true;
        }

        public bool CanAccessDoor(FacilityDoor door)  => CheckPermissions(Permissions, door);

        public bool CheckPermissions(KeycardPermissions perms, FacilityDoor door) => perms.HasFlagFast(door.Permissions);

        public KeycardItem GetKeycardForDoor(FacilityDoor door)
        {
            List<KeycardItem> keycards = GetAllItems<KeycardItem>();
            foreach (KeycardItem item in keycards)
                if (CheckPermissions(item.Permissions, door))
                    return item;
            return null;
        }

        public bool TryGetKeycardForDoor(FacilityDoor door, out KeycardItem item)
        {
            item = GetKeycardForDoor(door);
            return item != null;
        }

        public bool HasFollowTarget
        {
            get
            {
                if (FollowTarget == null)
                    return false;
                else if (!CanFollow(FollowTarget))
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
                else if (!CanTarget(EnemyTarget))
                {
                    EnemyTarget = null;
                    return false;
                }

                return true;
            }
        }

        public bool HasLOSOnEnemy(out Vector3 pos, bool prioritizeHead = false) { pos = Vector3.zero; return HasEnemyTarget && HasLOS(EnemyTarget, out pos, prioritizeHead); }

        public const string NoKOS = "npckos";

        public static bool DisableKOS => ServerVariableManager.TryGetVar(NoKOS, out ServerVariable svar) && bool.TryParse(svar.Value, out bool v) && !v;

        public bool CanTarget(Player p) =>
            p != null
            && p.ReferenceHub != ReferenceHub
            && p.IsAlive
            && !p.IsDisarmed
            && !p.IsGodModeEnabled
            && !IsInvisible(p)
            && IsEnemy(p)
            && (!DisableKOS || !IsCivilian(p) || IsArmed(p))
            && HasLOS(p, out _);

        public bool CanFollow(Player p) =>
            p != null
            && p.ReferenceHub != ReferenceHub
            && p.IsAlive
            && !p.IsGodModeEnabled
            && !IsInvisible(p)
            && !IsEnemy(p);

        public bool IsCivilian(Player p) => p.Role == RoleTypeId.ClassD || p.Role == RoleTypeId.Scientist;

        public bool IsArmed(Player p) => p.CurrentItem != null && (p.CurrentItem.Category == ItemCategory.Firearm || p.CurrentItem.Category == ItemCategory.Grenade);

        public bool IsInvisible(Player p) => p.EffectsManager.TryGetEffect(out Invisible inv) && inv.IsEnabled;

        public bool IsEnemy(Player p) => p.Role.GetFaction() != Role.GetFaction();

        public float GetDistance(Player p) => Vector3.Distance(Position, p.Position);

        public bool WithinDistance(Player p, float dist) => GetDistance(p) <= dist;
    }
}
