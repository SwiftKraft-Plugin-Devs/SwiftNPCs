using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PluginAPI.Core;
using static UnityEngine.GraphicsBuffer;

namespace SwiftNPCs.Core.World.AIModules
{
    public class AIGrenadeThrow : AIModuleBase
    {
        public Inventory Inventory => Parent.Inventory;

        public Player Target
        {
            get => Parent.EnemyTarget;
            set => Parent.EnemyTarget = value;
        }

        public bool HasLOS(out Vector3 pos) => Parent.HasLOSOnEnemy(out pos, RightClick);

        public float Delay = 1f;

        public bool RightClick;
        public bool InfiniteGrenades = false;

        float delay;

        public override void End(AIModuleBase next) { }

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
            if (delay > 0f)
                delay -= Time.fixedDeltaTime;

            if (TryGetThrowable(out ThrowableItem throwable))
                Throw(throwable);
            else
            {
                foreach (ItemBase item in Parent.Inventory.UserInventory.Items.Values)
                    if (item is ThrowableItem t)
                        Parent.Inventory.ServerSelectItem(t.ItemSerial);
            }

            if (HasLOS(out Vector3 pos))
                Parent.MovementEngine.LookPos = pos;

            CheckTransitions();
        }

        public void Throw(ThrowableItem item)
        {
            if (delay > 0f)
                return;

            delay = Delay;
            if (InfiniteGrenades)
                Parent.Core.Profile.Player.AddItem(item.ItemTypeId);
            item.ServerProcessInitiation();
            item.ServerProcessThrowConfirmation(!RightClick, Parent.CameraPosition, Parent.ReferenceHub.transform.rotation, Vector3.zero);
        }

        public ThrowableItem GetThrowable()
        {
            if (Parent.CurrentItem is ThrowableItem item)
                return item;
            return null;
        }

        public bool TryGetThrowable(out ThrowableItem item)
        {
            item = GetThrowable();
            return item != null;
        }
    }
}
