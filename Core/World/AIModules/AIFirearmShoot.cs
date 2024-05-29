using InventorySystem;
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

        public float ShootingDotRequirement = 0.6f;
        public float HipfireRange = 7f;
        public float RandomAimRangeFar = 1.25f;
        public float RandomAimRangeFarDistance = 40f;
        public float RandomAimRangeClose = 0.05f;
        public float RandomAimRangeCloseDistance = 10f;
        public float RandomAimTimer = 0.5f;
        public float ShootingTurnSpeed = 30f;

        protected float Timer;

        protected FirearmState State;

        Vector3 randomAim;
        float randomAimTimer;
        float originalTurnSpeed;

        public override bool Condition() => Parent.HasItemOfCategory(ItemCategory.Firearm);

        public override void OnDisabled()
        {
            Parent.MovementEngine.LookSpeed = originalTurnSpeed;
            IsAiming = false;
        }

        public override void OnEnabled()
        {
            originalTurnSpeed = Parent.MovementEngine.LookSpeed;
        }

        public override void Init()
        {
            Tags = [AIBehaviorBase.AttackerTag];
            originalTurnSpeed = Parent.MovementEngine.LookSpeed;
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

            if (randomAimTimer > 0f)
                randomAimTimer -= Time.fixedDeltaTime;
            else
            {
                randomAimTimer = RandomAimTimer;
                randomAim = Random.insideUnitSphere * Mathf.Lerp(RandomAimRangeClose, RandomAimRangeFar, Mathf.InverseLerp(RandomAimRangeCloseDistance, RandomAimRangeFarDistance, Parent.GetDistance(Target)));
            }

            bool hasLOS = HasLOS(out Vector3 pos);

            if (hasLOS)
                Parent.MovementEngine.LookPos = pos + randomAim;

            if (Parent.TryGetItem(out Firearm f))
            {
                if (Timer > 0f)
                    Timer -= Time.fixedDeltaTime;
                else
                {
                    Parent.MovementEngine.LookSpeed = originalTurnSpeed;
                    State = FirearmState.Standby;
                }

                if (f.Status.Ammo <= 0)
                    StartReload(f);
                else if (hasLOS && Parent.GetDotProduct(pos) < ShootingDotRequirement)
                    Shoot(f);
                else
                    Target = null;

                if (!HasTarget && State == FirearmState.Standby)
                    IsAiming = false;
            }
            else
                Parent.EquipItem<Firearm>();
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

            Parent.MovementEngine.LookSpeed = ShootingTurnSpeed;

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
