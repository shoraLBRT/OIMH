#if EADON_RPG_INVECTOR
using Invector.vItemManager;
using UnityEngine.UI;

namespace Eadon.Rpg.Invector.VendorSystem
{
    public class EadonMerchantItemOptionWindow : vItemOptionWindow
    {
        public Button buyItemButton;
        public Button sellItemButton;

        public override void EnableOptions(vItemSlot slot)
        {
            //if (slot ==null || slot.item==null) return;
            //useItemButton.interactable = itemsCanBeUsed.Contains(slot.item.type);
        }

        protected override void ValidateButtons(vItem item, out bool result)
        {
            result = true;
        }

        public override bool CanOpenOptions(vItem item)
        {
            return item != null;
        }
    }
}

#endif
