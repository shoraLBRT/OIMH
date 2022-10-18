#if EADON_RPG_INVECTOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Eadon.Rpg.Invector.ClothingSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Invector.vItemManager
{
    public class EadonInventory : vInventory
    {
        public ClothingEquipArea[] clothingEquipAreas;
        
        private ClothingEquipArea currentClothingArea;
        private readonly FieldInfo _currentEquipArea = typeof(vInventory).GetField("currentEquipArea", BindingFlags.NonPublic | BindingFlags.Instance);
        private bool _initialised;

        private void Update()
        {
            if (!_initialised)
            {
                SetupClothingAreas();
            }
        }

        private void SetupClothingAreas()
        {
            clothingEquipAreas = GetComponentsInChildren<ClothingEquipArea>(true);
            foreach (ClothingEquipArea equipArea in clothingEquipAreas)
            {
                equipArea.Init();
                equipArea.onSelectClothingEquipArea.AddListener(SetCurrentSelectedClothingArea);
            }

            _initialised = true;
        }

        /// <summary>
        /// Input Button to remove the current selected equipped Item
        /// </summary>
        protected override void RemoveEquipmentInput()
        {
            if (CurrentEquipArea != null && removeEquipment.GetButtonDown())
            {
                CurrentEquipArea.UnequipCurrentItem();
            }
            if (currentClothingArea != null && removeEquipment.GetButtonDown())
            {
                currentClothingArea.UnequipCurrentItem();
            }
        }

        /// <summary>
        /// Input to use a equipped and consumable Item
        /// </summary>
        /// <param name="changeEquip"></param>
        protected override void UseItemInput(ChangeEquipmentControl changeEquip)
        {
            if (changeEquip.display != null && changeEquip.display.item != null && (changeEquip.display.item.type == vItemType.Consumable || changeEquip.display.item.type == vItemType.Spell))
            {
                if (changeEquip.useItemInput.GetButtonDown() && changeEquip.display.item.amount > 0)
                    OnUseItem(changeEquip.display.item);
            }
        }

        private void SetCurrentSelectedClothingArea(ClothingEquipArea equipArea)
        {
            currentClothingArea = equipArea;
        }

        private vEquipArea CurrentEquipArea
        {
            get => (vEquipArea)_currentEquipArea.GetValue(this);
        }

    }
}

#endif
