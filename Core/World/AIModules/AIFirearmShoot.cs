using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.BasicMessages;
using PluginAPI.Core;
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

        public bool HasLOS(out Vector3 pos) => Parent.HasLOSOnEnemy(out pos, Headshots);
        public bool HasTarget => Parent.HasEnemyTarget;

        public bool IsAiming
        {
            get
            {
                if (Parent.TryGetItem(out Firearm f))
                    return f.AdsModule.ServerAds;
                return false;
            }
            set
            {
                if (Parent.TryGetItem(out Firearm f))
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

        public bool Headshots;
        public bool InfiniteAmmo = true;

        public float HipfireRange = 7f;

        protected float Timer;

        protected FirearmState State;

        public override bool Condition() => Parent.HasItemOfCategory(ItemCategory.Firearm);

        public override void OnDisabled()
        {
            IsAiming = false;
        }

        public override void OnEnabled() { }

        public override void Init()
        {
            Tags = [AIBehaviorBase.AttackerTag];
            Headshots = Random.Range(0, 2) == 0;
        }

        public override void Tick()
        {
            if (!Enabled)
                return;

            if (!HasTarget)
            {
                IsAiming = false;
                return;
            }

            if (Parent.TryGetItem(out Firearm f))
            {
                if (Timer > 0f)
                    Timer -= Time.fixedDeltaTime;
                else
                    State = FirearmState.Standby;

                if (f.Status.Ammo <= 0)
                    StartReload(f);
                else if (HasLOS(out _))
                    Shoot(f);
                else
                    Target = null;

                if (!HasTarget && State == FirearmState.Standby)
                    IsAiming = false;
            }
            else
                Parent.EquipItem<Firearm>();

            if (HasLOS(out Vector3 pos))
                Parent.MovementEngine.LookPos = pos;
        }

        public bool StartReload(Firearm f)
        {
            IsAiming = false;

            if (f == null || State != FirearmState.Standby)
                return false;

            if (InfiniteAmmo && Parent.Inventory.GetCurAmmo(f.AmmoType) < f.AmmoManagerModule.MaxAmmo)
                Parent.Inventory.ServerAddAmmo(f.AmmoType, f.AmmoManagerModule.MaxAmmo);

            if (f.AmmoManagerModule.ServerTryReload())
            {
                new RequestMessage(f.ItemSerial, RequestType.Reload).SendToAuthenticated();
                return true;
            }

            return false;
        }

        public bool Shoot(Firearm f)
        {
            if (f.Status.Ammo <= 0 || State != FirearmState.Standby || !f.EquipperModule.Standby || !f.ActionModule.Standby || !f.AdsModule.Standby || !f.AmmoManagerModule.Standby || !f.HitregModule.Standby)
                return false;

            IsAiming = HasTarget && (Vector3.Distance(Target.Position, Parent.CameraPosition) > HipfireRange);

            State = FirearmState.Shooting;
            Timer = 1f / f.ActionModule.CyclicRate;

            if (f.ActionModule.ServerAuthorizeShot() && f.HitregModule.ClientCalculateHit(out ShotMessage shot))
            {
                f.HitregModule.ServerProcessShot(shot);
                f.UpdateAnims();
                return true;
            }

            return false;
        }

        public enum FirearmState
        {
            Standby,
            Shooting
        }
    }
}
