#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Eadon.Rpg.Invector.ClothingSystem
{
    [CreateAssetMenu(menuName ="Eadon/Managers/Clothing Manager")]
    public class ClothingManager : ScriptableObject
    {
        public List<EadonClothingItem> allItems = new List<EadonClothingItem>();
        public List<EadonClothingSet> allSets = new List<EadonClothingSet>();

        private Dictionary<int, EadonClothingItem> _itemDict = new Dictionary<int, EadonClothingItem>();
        private Dictionary<int, EadonClothingSet> _itemSetDict = new Dictionary<int, EadonClothingSet>();

        public void Init()
        {
            _itemDict = new Dictionary<int, EadonClothingItem>();
			
            foreach (var t in allItems)
            {
                if (!_itemDict.ContainsKey(t.clothingId))
                {
                    _itemDict.Add(t.clothingId, t);
                }
                else
                {
                    Debug.LogWarning($"ClothingManager: item '{t.name}' already present. Check for duplicates");
                }
            }
            
            _itemSetDict = new Dictionary<int, EadonClothingSet>();
			
            foreach (var t in allSets)
            {
                if (!_itemSetDict.ContainsKey(t.clothingSetId))
                {
                    _itemSetDict.Add(t.clothingSetId, t);
                }
                else
                {
                    Debug.LogWarning($"ClothingManager: item set '{t.name}' already present. Check for duplicates");
                }
            }
        }

        public EadonClothingItem GetItemInstance(int clothingId)
        {
            var defaultItem = GetItem(clothingId);
            if (defaultItem == null) return null;
            var newItem = Instantiate(defaultItem);
            newItem.name = defaultItem.name;

            return newItem;
        }

        private EadonClothingItem GetItem(int clothingId)
        {
            _itemDict.TryGetValue(clothingId, out var retVal);
            return retVal;
        }

        public string[] GetAllItemNames()
        {
            var names = new List<string>();
            foreach (var key in _itemDict.Keys)
            {
                names.Add(_itemDict[key].name);
            }

            return names.ToArray();
        }

        public EadonClothingSet GetItemSetInstance(int clothingSetId)
        {
            var defaultItem = GetItemSet(clothingSetId);
            if (defaultItem == null) return null;
            var newItem = Instantiate(defaultItem);
            newItem.name = defaultItem.name;

            return newItem;
        }

        private EadonClothingSet GetItemSet(int clothingSetId)
        {
            _itemSetDict.TryGetValue(clothingSetId, out var retVal);
            return retVal;
        }

        public string[] GetAllItemSetNames()
        {
            var names = new List<string>();
            foreach (var key in _itemSetDict.Keys)
            {
                names.Add(_itemSetDict[key].name);
            }

            return names.ToArray();
        }
    }
}
#endif
