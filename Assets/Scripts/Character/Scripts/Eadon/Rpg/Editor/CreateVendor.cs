#if EADON_RPG_INVECTOR
using System.IO;
using Eadon.Rpg.Invector.Configuration;
using Eadon.Rpg.Invector.VendorSystem;
using Invector.vCharacterController.AI;
using Invector.vCharacterController.vActions;
using Invector.vItemManager;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;

namespace Eadon.Rpg.Invector.Editor
{
    public class CreateVendor : EditorWindow
    {
        private GUIStyle _redStyle;
        private GUIStyle _bgColor;

        private GameObject _vendorGameObject;
        private GameObject _vendorInventory;
        private GameObject _merchantTrigger;
        private vItemListData _vendorVItemListData;
        private string _vendorName;
        private int _moneyItem;
        private bool _initialised;
        private string _errorMessage;
        
        [MenuItem("Invector/Eadon RPG/Create Vendor")]
        private static void ShowWindow()
        {
            var window = GetWindow<CreateVendor>();
            window.titleContent = new GUIContent("Create Vendor");
            window.titleContent = new GUIContent(WindowTitle());
            window.minSize = MinWindowSize();
            window.maxSize = MinWindowSize();
            window.SetupStyles();
            window.ShowUtility();
        }

        private static Vector2 MinWindowSize()
        {
            return new Vector2(350, 450);
        }

        private static string WindowTitle()
        {
            return "Eadon - Create Vendor";
        }


        private void OnGUI()
        {
            if (!_initialised)
            {
                Initialise();
            }
            
            var splashTexture = (Texture2D) Resources.Load("Textures/eadon_rpg_create_vendor", typeof(Texture2D));
            GUILayoutUtility.GetRect(1f, 3f, GUILayout.ExpandWidth(false));
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(100f));
            GUI.DrawTexture(rect, splashTexture, ScaleMode.ScaleToFit, true, 0f);

            EditorGUILayout.Space();

            _vendorGameObject =
                EditorGUILayout.ObjectField("Vendor Model", _vendorGameObject, typeof(GameObject), true)
                    as GameObject;

            _vendorInventory =
                EditorGUILayout.ObjectField("Vendor Inventory", _vendorInventory, typeof(GameObject), false)
                    as GameObject;
            
            _merchantTrigger =
                EditorGUILayout.ObjectField("Merchant Trigger", _merchantTrigger, typeof(GameObject), false)
                    as GameObject;
            
            _vendorVItemListData =
                EditorGUILayout.ObjectField("Item List", _vendorVItemListData, typeof(vItemListData), false) as
                    vItemListData;

            _vendorName = EditorGUILayout.TextField("Vendor Name", _vendorName);
            
            _moneyItem =
                EditorGUILayout.IntField("Money Item", _moneyItem);

            if (CheckValues())
            {
                if (GUILayout.Button("Create Vendor"))
                {
                    CreateEadonVendor();
                }
            }
            else
            {
                EditorGUILayout.LabelField(_errorMessage, _redStyle);
            }
        }

        private void CreateEadonVendor()
        {
            var vendorGameObject = Instantiate(_vendorGameObject);
            vendorGameObject.name = string.IsNullOrWhiteSpace(_vendorName) ? "Eadon Vendor" : _vendorName;

            var vendor = vendorGameObject.GetComponent<EadonVendor>();
            if (vendor == null)
            {
                vendor = vendorGameObject.AddComponent<EadonVendor>();
            }

            var moneyManager = vendorGameObject.GetComponent<EadonMoneyManager>();
            if (moneyManager == null)
            {
                moneyManager = vendorGameObject.AddComponent<EadonMoneyManager>();
            }
            moneyManager.coinsItemId = _moneyItem;

            var itemManager = vendorGameObject.GetComponent<vItemManager>();
            if (itemManager == null)
            {
                itemManager = vendorGameObject.AddComponent<vItemManager>();
            }
            itemManager.itemListData = _vendorVItemListData;

            var inventoryGameObject = PrefabUtility.InstantiatePrefab(_vendorInventory, vendorGameObject.transform) as GameObject;
            if (inventoryGameObject != null)
            {
                var inventory = inventoryGameObject.GetComponentInChildren<vInventory>();
            }
            
            var merchantTriggerGameObject =
                PrefabUtility.InstantiatePrefab(_merchantTrigger, vendorGameObject.transform) as GameObject;
            if (merchantTriggerGameObject != null)
            {
                var merchantTrigger = merchantTriggerGameObject.GetComponent<vTriggerGenericAction>();
                if (merchantTrigger != null)
                {
                    UnityEventTools.AddPersistentListener(merchantTrigger.OnPressActionInput, vendor.OpenMerchantGui);
                    UnityEventTools.AddPersistentListener(merchantTrigger.OnPlayerEnter, vendor.SetMerchant);
                    UnityEventTools.AddPersistentListener(merchantTrigger.OnPlayerExit, vendor.ClearMerchant);
                }
            }
        }

        private bool CheckValues()
        {
            var result = true;
            var lines = 0;
            _errorMessage = "";

            if (_vendorGameObject == null)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines, "Please select a prefab for the vendor");
            }
            else
            {
                var simpleAi = _vendorGameObject.GetComponent<vSimpleMeleeAI_Controller>();
                bool controlAi = false;
#if INVECTOR_AI_TEMPLATE
                controlAi = _vendorGameObject.GetComponent(typeof(vControlAI)) != null;
#endif
                if (simpleAi == null && !controlAi)
                {
                    result = false;
                    _errorMessage = AppendLine(_errorMessage, ref lines, "Please select a valid Invector AI for the vendor");
                }
            }

            if (_vendorInventory == null)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines, "Please select a prefab for the inventory");
            }
            else
            {
                var inventory = _vendorInventory.GetComponentInChildren<vInventory>();
                if (inventory == null)
                {
                    result = false;
                    _errorMessage = AppendLine(_errorMessage, ref lines, "Please select a valid vInventory prefab");
                }
            }

            if (_merchantTrigger == null)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines, "Please select a prefab for the merchant trigger");
            }
            else
            {
                var trigger = _merchantTrigger.GetComponentInChildren<vTriggerGenericAction>();
                if (trigger == null)
                {
                    result = false;
                    _errorMessage = AppendLine(_errorMessage, ref lines, "Please select a valid merchant trigger prefab");
                }
            }

            if (_vendorVItemListData == null)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines, "Please select a vItemList for the vendor");
            }
            else
            {
                var money = _vendorVItemListData.items.Find(i => i.id == _moneyItem);
                if (money == null || money.type != vItemType.Money)
                {
                    result = false;
                    _errorMessage = AppendLine(_errorMessage, ref lines, "Please select a valid money item");
                }
            }

            return result;
        }

        private void Initialise()
        {
            if (File.Exists(Application.dataPath +
                            "/Cogs & Goggles/Eadon RPG for Invector/Prefabs/Invector/Eadon Vendor Inventory.prefab"))
            {
                _vendorInventory =
                    AssetDatabase.LoadAssetAtPath(
                        "Assets/Cogs & Goggles/Eadon RPG for Invector/Prefabs/Invector/Eadon Vendor Inventory.prefab",
                        typeof(GameObject)) as GameObject;
            }
            
            if (File.Exists(Application.dataPath +
                            "/Cogs & Goggles/Eadon RPG for Invector/Prefabs/Invector/Merchant Trigger.prefab"))
            {
                _merchantTrigger =
                    AssetDatabase.LoadAssetAtPath(
                        "Assets/Cogs & Goggles/Eadon RPG for Invector/Prefabs/Invector/Merchant Trigger.prefab",
                        typeof(GameObject)) as GameObject;
            }
            
            if (File.Exists(Application.dataPath +
                            "/Cogs & Goggles/Eadon RPG for Invector/Data/Invector/Eadon RPG Demo vItemList.asset"))
            {
                _vendorVItemListData =
                    AssetDatabase.LoadAssetAtPath(
                        "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Invector/Eadon RPG Demo vItemList.asset",
                        typeof(vItemListData)) as vItemListData;
            }

            _initialised = true;
        }

        private void SetupStyles()
        {
            _redStyle = new GUIStyle {normal = {textColor = Color.red}};
            var backgroundTexture = (Texture2D) Resources.Load("Textures/eadon_editor_background", typeof(Texture2D));
            _bgColor = new GUIStyle {normal = {background = backgroundTexture}};
        }

        protected string AppendLine(string messages, ref int lines, string line)
        {
            if (lines > 0)
            {
                messages += "\n";
            }

            messages += line;
            lines++;
            return messages;
        }
    }
}
#endif
