#if EADON_RPG_INVECTOR
using Invector.vItemManager;
using UnityEngine;
using UnityEngine.UI;

namespace Eadon.Rpg.Invector.VendorSystem
{
    public class EadonMerchantTransactionManager : MonoBehaviour
    {
        public vInventory playerInventory;
        public vInventory merchantInventory;

        public vItemManager playerItemManager;
        public vItemManager merchantItemManager;

        public EadonMoneyManager playerMoney;
        public EadonMoneyManager merchantMoney;
        
        public vItemWindow inventoryItemWindow;
        public vItemWindow merchantItemWindow;
        public Text merchantNameField;
        public Text playerMoneyField;
        public Text merchantMoneyField;
        public GameObject errorDialog;
        public Text errorDialogMessage;
        
        private void OnEnable()
        {
            var player = EadonVendorManager.Instance.playerCharacter.gameObject;
            var merchant = EadonVendorManager.Instance.currentMerchant.gameObject;

            if (merchantNameField != null && merchant != null)
            {
                merchantNameField.text = merchant.name;
            }
            
            if (playerInventory == null)
            {
                if (player)
                {
                    playerInventory = player.GetComponentInChildren<vInventory>();
                }
            }
            if (playerItemManager == null)
            {
                if (player)
                {
                    playerItemManager = player.GetComponent<vItemManager>();
                }
            }
            if (playerMoney == null)
            {
                if (player)
                {
                    playerMoney = player.GetComponent<EadonMoneyManager>();
                    playerMoneyField.text = "" + playerMoney.Coins;
                }
            }
            if (merchant)
            {
                merchantInventory = merchant.GetComponentInChildren<vInventory>();
            }
            if (merchant)
            {
                merchantItemManager = merchant.GetComponent<vItemManager>();
            }
            if (merchant)
            {
                merchantMoney = merchant.GetComponent<EadonMoneyManager>();
                merchantMoneyField.text = "" + merchantMoney.Coins;
            }
            var merchantWindowDisplay = merchantItemWindow.gameObject.GetComponent<EadonMerchantItemWindowDisplay>();
            if (merchantWindowDisplay != null)
            {
                merchantWindowDisplay.inventory = merchantInventory;
            }

        }

        public void BuyItem(vItem item, int amount)
        {
            if (merchantInventory == null || merchantItemManager == null)
            {
                return;
            }
            
            if (playerInventory == null || playerItemManager == null)
            {
                return;
            }

            if (playerMoney == null || merchantMoney == null)
            {
                return;
            }

            var buyPrice = 1;
            var buyPriceAttribute = item.GetItemAttribute("BuyPrice");
            if (buyPriceAttribute != null)
            {
                buyPrice = buyPriceAttribute.value * amount;
            }

            if (playerMoney.Coins >= buyPrice)
            {
                merchantItemManager.DestroyItem(item, amount);
                merchantInventory.UpdateInventory();

                var itemRef = new ItemReference(item.id) {autoEquip = false, addToEquipArea = false, amount = amount};

                playerItemManager.AddItem(itemRef, true);
                playerInventory.UpdateInventory();

                if (inventoryItemWindow != null)
                {
                    inventoryItemWindow.gameObject.SetActive(false);
                    inventoryItemWindow.gameObject.SetActive(true);
                }

                if (merchantItemWindow != null)
                {
                    merchantItemWindow.gameObject.SetActive(false);
                    merchantItemWindow.gameObject.SetActive(true);
                }

                playerMoney.RemoveCoins(buyPrice);
                merchantMoney.AddCoins(buyPrice);

                playerMoneyField.text = "" + playerMoney.Coins;
                merchantMoneyField.text = "" + merchantMoney.Coins;
            }
            else
            {
                errorDialogMessage.text = "You don't have enough money";
                errorDialog.SetActive(true);
            }
        }

        public void SellItem(vItem item, int amount)
        {
            if (merchantInventory == null || merchantItemManager == null)
                return;
            
            if (playerInventory == null || playerItemManager == null)
                return;

            
            var sellPrice = 1;
            var sellPriceAttribute = item.GetItemAttribute("SellPrice");
            if (sellPriceAttribute != null)
            {
                sellPrice = sellPriceAttribute.value * amount;
            }

            if (merchantMoney.Coins >= sellPrice)
            {
                playerItemManager.DestroyItem(item, amount);
                playerInventory.UpdateInventory();

                var itemRef = new ItemReference(item.id) {autoEquip = false, addToEquipArea = false, amount = amount};

                merchantItemManager.AddItem(itemRef, true);
                merchantInventory.UpdateInventory();

                if (inventoryItemWindow != null)
                {
                    inventoryItemWindow.gameObject.SetActive(false);
                    inventoryItemWindow.gameObject.SetActive(true);
                }

                if (merchantItemWindow != null)
                {
                    merchantItemWindow.gameObject.SetActive(false);
                    merchantItemWindow.gameObject.SetActive(true);
                }

                playerMoney.AddCoins(sellPrice);
                merchantMoney.RemoveCoins(sellPrice);

                playerMoneyField.text = "" + playerMoney.Coins;
                merchantMoneyField.text = "" + merchantMoney.Coins;
            }
            else
            {
                errorDialogMessage.text = "You don't have enough money";
                errorDialog.SetActive(true);
            }
        }
    }
}

#endif
