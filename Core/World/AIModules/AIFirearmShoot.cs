using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.BasicMessages;
using MEC;
using PluginAPI.Core;
using System.Collections.Generic;
using UnityEngine;
using Utils.Networking;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIFirearmShoot : AIModuleBase
    {
        public Player Target
        {
            get => Parent.EnemyTarget;
            set => Parent.EnemyTarget = value;
        }

        public bool HasLOS(out Vector3 pos) => Parent.HasLOSOnEnemy(out pos);
        public bool HasTarget => Parent.HasEnemyTarget;

        public bool IsAiming
        {
            get
            {
                if (TryGetFirearm(out Firearm f))
                    return f.AdsModule.ServerAds;
                return false;
            }
            set
            {
                if (TryGetFirearm(out Firearm f))
                {
                    if (value == f.AdsModule.ServerAds)
                        return;

                    if (value)
                        new RequestMessage(f.ItemSerial, RequestType.AdsIn).SendToAuthenticated();
                    else
                        new RequestMessage(f.ItemSerial, RequestType.AdsOut).SendToAuthenticated();

                    f.AdsModule.ServerAds = value;
                }
            }
        }

        protected float Timer;

        protected FirearmState State;

        public override void End(AIModuleBase next)
        {
            IsAiming = false;
        }

        public override void Init() { }

        public override void ReceiveData<T>(T data)
        {
            if (data is not Player p)
                return;

            Target = p;
        }

        public override void Start(AIModuleBase prev) { }

        public override void Tick()
        {
            if (TryGetFirearm(out Firearm f))
            {
                if (Timer > 0f)
                    Timer -= Time.fixedDeltaTime;
                else
                    State = FirearmState.Standby;

                if (f.Status.Ammo <= 0)
                    Timing.RunCoroutine(Reload(f));
                else if (HasLOS(out _))
                    Shoot(f);
                else
                    Target = null;

                if (!HasTarget && State == FirearmState.Standby)
                    IsAiming = false;
            }
            else
            {
                foreach (ItemBase item in Parent.Inventory.UserInventory.Items.Values)
                    if (item is Firearm firearm)
                        Parent.Inventory.ServerSelectItem(firearm.ItemSerial);
            }

            if (HasLOS(out Vector3 pos))
                Parent.MovementEngine.LookPos = pos;

            CheckTransitions();
        }

        public Firearm GetFirearm()
        {
            if (Parent.CurrentItem is Firearm f)
                return f;
            return null;
        }

        public bool TryGetFirearm(out Firearm firearm)
        {
            firearm = GetFirearm();
            return firearm != null;
        }

        public IEnumerator<float> Reload(Firearm f)
        {
            IsAiming = false;

            if (StartReload(f))
            {
                yield return Timing.WaitForSeconds(4f);
                EndReload(f);
            }
        }

        public bool EndReload(Firearm f)
        {
            if (f == null)
                return false;

            new RequestMessage(f.ItemSerial, RequestType.ReloadStop).SendToAuthenticated();
            f.Status = new FirearmStatus(f.AmmoManagerModule.MaxAmmo, FirearmStatusFlags.MagazineInserted | FirearmStatusFlags.Chambered, f.Status.Attachments);

            return true;
        }

        public bool StartReload(Firearm f)
        {
            if (f == null || State != FirearmState.Standby || f.Status.Ammo >= f.AmmoManagerModule.MaxAmmo)
                return false;

            new RequestMessage(f.ItemSerial, RequestType.Reload).SendToAuthenticated();
            State = FirearmState.Reloading;

            return true;
        }

        public bool Shoot(Firearm f)
        {
            if (State != FirearmState.Standby || !f.InspectorModule.Standby || !f.EquipperModule.Standby || !f.ActionModule.Standby || !f.AdsModule.Standby || !f.AmmoManagerModule.Standby || !f.HitregModule.Standby || f.Status.Ammo <= 0)
                return false;

            IsAiming = true;

            if (f.HitregModule.ClientCalculateHit(out ShotMessage shot))
            {
                f.HitregModule.ServerProcessShot(shot);
                if (f.ActionModule.ServerAuthorizeShot())
                {
                    State = FirearmState.Shooting;
                    Timer = 1f / f.ActionModule.CyclicRate;
                    f.UpdateAnims();
                    return true;
                }
            }

            return false;
        }

        public enum FirearmState
        {
            Standby,
            Shooting,
            Reloading
        }
    }
}
