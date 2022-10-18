#if EADON_RPG_INVECTOR
using Invector.vItemManager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Eadon.Rpg.Invector.VendorSystem
{
    public class EadonMerchantItemWindowDisplay : MonoBehaviour
    {
        public vInventory inventory;
        public vItemWindow itemWindow;
        public EadonMerchantItemOptionWindow optionWindow;
        public EadonMerchantTransactionManager transactionManager;
        [HideInInspector]
        public vItemSlot currentSelectedSlot;
        [HideInInspector]
        public int amount;        
       
        public virtual void OnEnable()
        {
            if (inventory == null)
            {
                var merchant = EadonVendorManager.Instance.currentMerchant;
                if (merchant)
                {
                    inventory = merchant.GetComponentInChildren<vInventory>();
                }
            }

            if (!inventory || !itemWindow) return;
            inventory.onDestroyItem.RemoveListener(OnDestroyItem);               
            inventory.onDestroyItem.AddListener(OnDestroyItem);
            itemWindow.CreateEquipmentWindow(inventory.items, OnSubmit, OnSelectSlot);
            inventory.OnUpdateInventory -= CheckItemExits;
            inventory.OnUpdateInventory += CheckItemExits;
        }

        public void OnDisable()
        {
            if(inventory)
                inventory.OnUpdateInventory -= CheckItemExits;
        }

        public void OnDestroyItem(vItem item, int amount)
        {         
            var slot = itemWindow.slots.Find(s => s.item.Equals(item));
            if (slot != null && (slot.item == null || slot.item.amount == 0))
            {
                
                itemWindow.slots.Remove(slot);
                Destroy(slot.gameObject);
            }
        }

        public void OnSubmit(vItemSlot slot)
        {
            currentSelectedSlot = slot;
            if (!slot.item) return;
            if (!optionWindow.CanOpenOptions(slot.item)) return;
            //optionWindow.transform.position = rect.position;
            optionWindow.gameObject.SetActive(true);
            optionWindow.EnableOptions(slot);
            // currentSelectedSlot = slot;
        }

        public void OnSelectSlot(vItemSlot slot)
        {
            currentSelectedSlot = slot;
        }

        public virtual void DropItem()
        {
            if (amount <= 0) return;
            inventory.OnDropItem(currentSelectedSlot.item, amount);
            if (currentSelectedSlot != null && (currentSelectedSlot.item == null || currentSelectedSlot.item.amount <= 0))
            {
                if (itemWindow.slots.Contains(currentSelectedSlot))
                    itemWindow.slots.Remove(currentSelectedSlot);
                Destroy(currentSelectedSlot.gameObject);
                if (itemWindow.slots.Count > 0)
                    SetSelectable(itemWindow.slots[0].gameObject);
            }
        }

        public void BuyItem()
        {
            if (amount <= 0) return;
            if (transactionManager != null)
            {
                transactionManager.BuyItem(currentSelectedSlot.item, amount);
            }
        }

        public virtual void SellItem()
        {
            if (amount <= 0) return;
            if (transactionManager != null)
            {
                transactionManager.SellItem(currentSelectedSlot.item, amount);
            }
        }

        public virtual void UseItem()
        {

            inventory.OnUseItem(currentSelectedSlot.item);
           
        }

        private void CheckItemExits()
        {
            itemWindow.ReloadItems(inventory.items);
        }

        public virtual void SetOldSelectable()
        {
            try
            {
                if (currentSelectedSlot != null)
                    SetSelectable(currentSelectedSlot.gameObject);
                else if (itemWindow.slots.Count > 0 && itemWindow.slots[0] != null)
                {
                    SetSelectable(itemWindow.slots[0].gameObject);
                }
            }
            catch
            {
                // ignored
            }
        }

        public void SetSelectable(GameObject target)
        {
            try
            {
                var current = EventSystem.current;
                var pointer = new PointerEventData(current);
                ExecuteEvents.Execute(current.currentSelectedGameObject, pointer, ExecuteEvents.pointerExitHandler);
                EventSystem.current.SetSelectedGameObject(target, new BaseEventData(EventSystem.current));
                ExecuteEvents.Execute(target, pointer, ExecuteEvents.selectHandler);
            }
            catch
            {
                // ignored
            }
        }
    }
}

#endif
