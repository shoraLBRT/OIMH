#if EADON_RPG_INVECTOR
using System.Collections;
using System.Collections.Generic;
using Eadon.Rpg.Invector.VendorSystem;
using Invector.vCharacterController;
using Invector.vItemManager;
using UnityEngine;

namespace Eadon.Rpg.Invector.StashSystem
{
    public class EadonStashManager : MonoBehaviour
    {
        public static EadonStashManager Instance { get; private set; }
        public EadonStash globalStash;
        
        public vThirdPersonController playerCharacter;
        public EadonStash currentStash;

        public vInventory stashUi;
        public EadonStashItemWindowDisplay playerInventoryDisplay;
        
        protected vItemManager stashItemManager;
        protected bool stashOpen;
        protected vInventory playerInventory;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            if (playerCharacter == null)
            {
                playerCharacter = FindObjectOfType<vThirdPersonController>();
                if (playerCharacter != null)
                {
                    playerInventory = playerCharacter.GetComponentInChildren<vInventory>();
                    if (playerInventory != null)
                    {
                        if (playerInventoryDisplay != null)
                        {
                            playerInventoryDisplay.inventory = playerInventory;
                        }
                    }
                }
            }

            if (stashUi == null)
            {
                stashUi = transform.parent.GetComponentInChildren<vInventory>();
            }
        }

        private void OnEnable()
        {
            if (playerCharacter == null)
            {
                FindPlayer();
            }
            if (playerCharacter != null)
            {
                playerInventory = playerCharacter.GetComponentInChildren<vInventory>();
                if (playerInventory != null)
                {
                    if (playerInventoryDisplay != null)
                    {
                        playerInventoryDisplay.inventory = playerInventory;
                    }
                }
            }
        }
        
        public void SetCurrentStash(EadonStash stash)
        {
            if (stash != null)
            {
                currentStash = stash;
            }
            else
            {
                currentStash = globalStash;
            }
        }

        public void OpenStashGui()
        {
            if (playerCharacter == null)
            {
                FindPlayer();
            }

            if (playerCharacter != null)
            {
                if (stashUi != null && currentStash != null)
                {
                    stashUi.OpenInventory();
                }
            }
        }

        public void StashOpen(bool value)
        {
            playerCharacter.GetComponent<vItemManager>().inventory.openInventory.useInput = !value;
            stashOpen = !stashOpen;
        }
        
        private void FindPlayer()
        {
            var player = FindObjectOfType<vThirdPersonController>();

            if (player)
            {
                playerCharacter = player;
                if (playerCharacter != null)
                {
                    playerInventory = playerCharacter.GetComponentInChildren<vInventory>();
                    if (playerInventory != null)
                    {
                        if (playerInventoryDisplay != null)
                        {
                            playerInventoryDisplay.inventory = playerInventory;
                        }
                    }
                }
            }
        }
    }
}

#endif
