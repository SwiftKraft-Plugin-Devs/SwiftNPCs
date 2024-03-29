using InventorySystem.Items;
using SwiftNPCs.Core.World.AIModules;

namespace SwiftNPCs.Core.World.AIConditions
{
    public class AIHasItemCategoryCondition : AIConditionBase
    {
        public ItemCategory Category;

        public override bool Get()
        {
            foreach (ItemBase item in ReferenceHub.inventory.UserInventory.Items.Values)
                if (item.Category == Category)
                    return true;
            return false;
        }

        public override void Pass(AIModuleBase target) { }
    }
}
