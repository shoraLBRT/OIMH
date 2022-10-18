#if EADON_RPG_INVECTOR
using Invector.vItemManager;
using UnityEngine;
using UnityEngine.UI;

namespace Eadon.Rpg.Invector.StashSystem
{
    public class EadonStashTransactionManager : MonoBehaviour
    {
        public vInventory playerInventory;
        public vInventory stashInventory;

        public vItemManager playerItemManager;
        public vItemManager stashItemManager;

        public vItemWindow inventoryItemWindow;
        public vItemWindow stashItemWindow;
        public Text stashNameField;
        public Text stashCapacityField;
        public GameObject errorDialog;
        public Text errorDialogMessage;
        private EadonStash _currentStash;
        
        private void OnEnable()
        {
            var player = EadonStashManager.Instance.playerCharacter.gameObject;
            _currentStash = EadonStashManager.Instance.currentStash;

            if (stashNameField != null && _currentStash != null)
            {
                if (!string.IsNullOrWhiteSpace(_currentStash.stashName))
                {
                    stashNameField.text = _currentStash.stashName;
                }
            }
            
            if (stashCapacityField != null && _currentStash != null)
            {
                if (_currentStash.stashCapacity > 0)
                {
                    stashCapacityField.text = "Stash Capacity: " + _currentStash.stashCapacity;
                }
                else
                {
                    stashCapacityField.text = "Unlimited Capacity";
                }
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
            if (_currentStash)
            {
                stashInventory = _currentStash.inventory;
                stashItemManager = _currentStash.GetComponent<vItemManager>();
            }
            var merchantWindowDisplay = stashItemWindow.gameObject.GetComponent<EadonStashItemWindowDisplay>();
            if (merchantWindowDisplay != null)
            {
                merchantWindowDisplay.inventory = stashInventory;
            }
            // var playerWindowDisplay = inventoryItemWindow.gameObject.GetComponent<EadonStashItemWindowDisplay>();
            // if (playerWindowDisplay != null)
            // {
            //     playerWindowDisplay.inventory = playerInventory;
            // }

        }

        public void TakeItem(vItem item, int amount)
        {
            if (stashInventory == null || stashItemManager == null)
            {
                return;
            }
            
            if (playerInventory == null || playerItemManager == null)
            {
                return;
            }

            stashItemManager.DestroyItem(item, amount);
            stashInventory.UpdateInventory();

            var itemRef = new ItemReference(item.id) {autoEquip = false, addToEquipArea = false, amount = amount};

            playerItemManager.AddItem(itemRef, true);
            playerInventory.UpdateInventory();

            if (inventoryItemWindow != null)
            {
                inventoryItemWindow.gameObject.SetActive(false);
                inventoryItemWindow.gameObject.SetActive(true);
            }

            if (stashItemWindow != null)
            {
                stashItemWindow.gameObject.SetActive(false);
                stashItemWindow.gameObject.SetActive(true);
            }
        }

        public void StashItem(vItem item, int amount)
        {
            if (stashInventory == null || stashItemManager == null)
                return;
            
            if (playerInventory == null || playerItemManager == null)
                return;

            if (CheckStashCapacity())
            {
                playerItemManager.DestroyItem(item, amount);
                playerInventory.UpdateInventory();

                var itemRef = new ItemReference(item.id) {autoEquip = false, addToEquipArea = false, amount = amount};

                stashItemManager.AddItem(itemRef, true);
                stashInventory.UpdateInventory();

                if (inventoryItemWindow != null)
                {
                    inventoryItemWindow.gameObject.SetActive(false);
                    inventoryItemWindow.gameObject.SetActive(true);
                }

                if (stashItemWindow != null)
                {
                    stashItemWindow.gameObject.SetActive(false);
                    stashItemWindow.gameObject.SetActive(true);
                }
            }
            else
            {
                errorDialog.SetActive(true);
            }
        }

        private bool CheckStashCapacity()
        {
            var itemsInStash = stashItemManager.GetItems().Count;
            return _currentStash.stashCapacity <= 0 || _currentStash.stashCapacity > itemsInStash;
        }
    }
}

#endif
