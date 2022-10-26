#if EADON_RPG_INVECTOR
using System;
using Invector.vItemManager;
using UnityEngine;

namespace Eadon.Rpg.Invector.VendorSystem
{
    public class EadonVendor : MonoBehaviour
    {
        private vInventory inventory;

        private void Start()
        {
            inventory = GetComponentInChildren<vInventory>();
            inventory.OnUpdateInventory += OnInventoryUpdate;
        }

        public void SetMerchant(GameObject go)
        {
            EadonVendorManager.Instance.SetCurrentMerchant(gameObject, inventory);
        }

        public void ClearMerchant(GameObject go)
        {
            EadonVendorManager.Instance.currentMerchant = null;
        }

        public void OpenMerchantGui()
        {
            EadonVendorManager.Instance.OpenMerchantGui();
        }

        private void OnInventoryUpdate()
        {
            
        }
    }
}

#endif
