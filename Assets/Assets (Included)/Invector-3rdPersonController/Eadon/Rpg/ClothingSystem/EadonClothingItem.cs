#if EADON_RPG_INVECTOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eadon.Rpg.Invector.ClothingSystem
{
    [Serializable]
    public class ClothingPrefab
    {
        public string prefabTag;
        public GameObject prefab;
    }
	
    [CreateAssetMenu(menuName = "Eadon/Inventory/Items/Clothing")]
    public class EadonClothingItem : ScriptableObject
    {
        public enum ClothingHolderNames
        {
            HeadClothing,                  // 0
            HelmetClothing,                // 1
            CowlClothing,                  // 2
            HatClothing,                   // 3
            FaceGuardClothing,             // 4
            HelmetAccessoryClothing,       // 5
            ChestClothing,                 // 6
            BackClothing,                  // 7
            UpperRightArmClothing,         // 8
            LowerRightArmClothing,         // 9
            BothUpperArmClothing,          // 10
            UpperLeftArmClothing,          // 11
            LowerLeftArmClothing,          // 12
            BothLowerArmClothing,          // 13
            RightHandClothing,             // 14
            LeftHandClothing,              // 15
            BothHandsClothing,             // 16
            LegsClothing,                  // 17
            RightFootClothing,             // 18
            LeftFootClothing,              // 19
            BothFeetClothing,              // 20
            RightShoulderAttachment,       // 21
            LeftShoulderAttachment,        // 22
            BothShoulderAttachment,        // 23
            RightElbowAttachment,          // 24
            LeftElbowAttachment,           // 25
            BothElbowAttachment,           // 26
            RightKneeAttachment,           // 27
            LeftKneeAttachment,            // 28
            BothKneeAttachment,            // 29
            BeltAttachment                 // 30
        };

        [Header("Clothing")]
        public int clothingId;
        public ClothingHolderNames clothingHolderName;
        public ClothingItemType clothingType;
		
        // Spawn And Attach
        [Header("Spawn & Attach Options")]
        public List<ClothingPrefab> clothingPrefabs = new List<ClothingPrefab>();

        public PositionOffset offset;
		
        // Show and Hide
        [Header("Show & Hide Options")]
        public string clothingName;
		
        // Change Texture On Default
        [Header("Change Texture Options")]
        public Material overrideMaterial;
        public int materialIndex;

        [Header("Blocking")]
        public List<ClothingHolderNames> blockedHolders;
        public bool removeDefaultWhenBlocked;
        
        public GameObject GetPrefabForTag(string prefabTag)
        {
            if (string.IsNullOrWhiteSpace(prefabTag))
            {
                return clothingPrefabs.Count > 0 ? clothingPrefabs[0].prefab : null;
            }

            foreach (var clothingPrefab in clothingPrefabs)
            {
                if (clothingPrefab.prefabTag == prefabTag)
                {
                    return clothingPrefab.prefab;
                }
            }

            if (clothingPrefabs.Count > 0)
            {
                return clothingPrefabs[0].prefab;
            }
            return null;
        }
    }
}
#endif
