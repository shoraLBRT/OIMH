#if EADON_RPG_INVECTOR
using Invector.vItemManager;
using UnityEngine;
using UnityEngine.UI;

namespace Eadon.Rpg.Invector.StashSystem
{
    public class EadonStashItemOptionWindow : vItemOptionWindow
    {
        public Button takeItemButton;
        public Button stashItemButton;

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
