#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using System.Linq;
using Invector.vItemManager;
using UnityEngine;

namespace Eadon.Rpg.Invector.VendorSystem
{
    public class EadonMoneyManager : MonoBehaviour
    {
        public int coinsItemId;
        
        private int _coins;

        public int Coins => GetCoins();

        private int GetCoins()
        {
            var coins = 0;
            
            var inventory = gameObject.GetComponentsInChildren<vInventory>()
                .First(i => i.gameObject.name == "Inventory");
            if (!inventory) return coins;
            {
                var moneyList = inventory.items.FindAll(i => i.type == vItemType.Money);
                if (moneyList.Count <= 0) return coins;
                coins = moneyList.Sum(item => item.amount);
            }

            return coins;
        }

        public void AddCoins(int amount)
        {
            var itemManager = gameObject.GetComponent<vItemManager>();
            if (!itemManager) return;
            var itemRef = new ItemReference(coinsItemId);
            itemRef.autoEquip = false;
            itemRef.addToEquipArea = false;
            itemRef.amount = amount;
            itemManager.AddItem(itemRef);
        }

        public void RemoveCoins(int amount)
        {
            var itemManager = gameObject.GetComponent<vItemManager>();
            if (!itemManager) return;
            var items = itemManager.items.FindAll(i => i.id == coinsItemId);

            var leftoverAmount = amount;
            for (var i = items.Count() - 1; i >= 0; i--)
            {
                if (items[i].amount <= leftoverAmount)
                {
                    leftoverAmount -= items[i].amount;
                    itemManager.DestroyItem(items[i]);
                }
                else
                {
                    items[i].amount -= leftoverAmount;
                }
            }
        }
    }
}

#endif
