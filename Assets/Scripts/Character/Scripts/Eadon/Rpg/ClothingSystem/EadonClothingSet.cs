#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using UnityEngine;

namespace Eadon.Rpg.Invector.ClothingSystem
{
    [CreateAssetMenu(menuName = "Eadon/Inventory/Items/Clothing Set")]
    public class EadonClothingSet : ScriptableObject
    {
        public int clothingSetId;
        public List<EadonClothingItem> allItems = new List<EadonClothingItem>();
    }
}
#endif
