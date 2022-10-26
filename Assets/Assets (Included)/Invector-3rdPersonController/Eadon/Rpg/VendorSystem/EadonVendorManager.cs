#if EADON_RPG_INVECTOR
using Invector.vCharacterController;
using Invector.vItemManager;
using UnityEngine;

namespace Eadon.Rpg.Invector.VendorSystem
{
    public class EadonVendorManager : MonoBehaviour
    {
        public static EadonVendorManager Instance { get; private set; }

        public vThirdPersonController playerCharacter;
        public GameObject currentMerchant;

        public vInventory merchantUi;
        public EadonMerchantItemWindowDisplay merchantItemWindow;
        
        private EadonMoneyManager _playerMoney;
        private Rigidbody _playerRigidBody;
        private bool _merchantOpen;

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
            }

            if (merchantUi == null)
            {
                merchantUi = transform.parent.GetComponentInChildren<vInventory>();
            }
        }

        private void OnEnable()
        {
            if (playerCharacter)
            {
                _playerMoney = playerCharacter.gameObject.GetComponent<EadonMoneyManager>();
            }
        }
        
        public void SetCurrentMerchant(GameObject merchant, vInventory inventory)
        {
            currentMerchant = merchant;
            merchantItemWindow.inventory = inventory;
        }

        public void OpenMerchantGui()
        {
            if (playerCharacter == null)
            {
                FindPlayer();
            }

            if (playerCharacter != null)
            {
                if (merchantUi != null && currentMerchant != null)
                {
                    merchantUi.OpenInventory();
                }
            }
        }

        public bool CanPayPrice(float price)
        {
            if (_playerMoney)
            {
                return _playerMoney.Coins >= price;
            }
            else
            {
                return false;
            }
        }

        public void MerchantOpen(bool value)
        {
            playerCharacter.GetComponent<vItemManager>().inventory.openInventory.useInput = !value;
            _merchantOpen = !_merchantOpen;
        }
        
        private void FindPlayer()
        {
            var player = FindObjectOfType<vThirdPersonController>();

            if (player)
            {
                playerCharacter = player;
                _playerMoney = playerCharacter.gameObject.GetComponent<EadonMoneyManager>();
            }
        }
    }
}

#endif
