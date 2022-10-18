#if EADON_RPG_INVECTOR
using Invector.vItemManager;
using UnityEngine;

namespace Eadon.Rpg.Invector.StashSystem
{
    public class EadonStash : MonoBehaviour
    {
        public bool isGlobal;
        public string stashName;
        public int stashCapacity = 10;
        [HideInInspector]
        public vInventory inventory;

        private void Start()
        {
            inventory = GetComponentInChildren<vInventory>();
            inventory.OnUpdateInventory += OnInventoryUpdate;
        }

        public void SetStash(GameObject go)
        {
            if (inventory != null)
            {
                if (isGlobal)
                {
                    EadonStashManager.Instance.SetCurrentStash(null);
                }
                else
                {
                    EadonStashManager.Instance.SetCurrentStash(this);
                }
            }
        }

        public void ClearStash(GameObject go)
        {
            EadonStashManager.Instance.currentStash = null;
        }

        public void OpenStashGui()
        {
            EadonStashManager.Instance.OpenStashGui();
        }

        private void OnInventoryUpdate()
        {
            
        }
    }
}
#endif
