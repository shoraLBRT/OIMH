#if EADON_RPG_INVECTOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eadon.Rpg.Invector.ClothingSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Eadon.Rpg.Invector.ClothingSystem
{
    public class EadonCharacterAppearance : MonoBehaviour
    {
        public ClothingEquipmentHolder[] clothingEquipmentHolders;
        public SkinnedMeshRenderer mainBody;
        public int mainBodyMaterialIndex;
        public readonly Dictionary<string, SkinnedMeshRenderer> submeshes =
            new Dictionary<string, SkinnedMeshRenderer>();
        public string prefabTag;
        public bool removeAllPreviousWhenChangingSet = true;

        [Header("Events")]
        public ClothingEquippedEvent beforeEquippingClothing = new ClothingEquippedEvent();
        public ClothingEquippedEvent afterEquippingClothing = new ClothingEquippedEvent();
        public ClothingEquippedEvent beforeUnequippingClothing = new ClothingEquippedEvent();
        public ClothingEquippedEvent afterUnequippingClothing = new ClothingEquippedEvent();

        private Material _blitter;
        private Material[] _originalBodyMaterials;
        private Texture2D _bodyTexture = null;
        private Texture2D _bodyMaskTexture = null;
        private Texture2D _currentMask;
        private bool _maskValid;
        [HideInInspector]
        public EadonClothingSet currentClothingSet;

        private void Awake()
        {
            SetupSubmeshes();
        }

        protected virtual void Start()
        {
            _maskValid = false;
            if (mainBody != null)
            {
                if (mainBody.sharedMaterials[mainBodyMaterialIndex].HasProperty("_Mask"))
                {
                    _bodyMaskTexture = mainBody.sharedMaterials[mainBodyMaterialIndex].GetTexture("_Mask") as Texture2D;
                    _currentMask = new Texture2D(_bodyMaskTexture.width, _bodyMaskTexture.height);
                    _maskValid = true;
                }

                _originalBodyMaterials = mainBody.sharedMaterials;
            }
        }

        public void WearListOfClothes(IEnumerable<EadonClothingItem> clothingItems)
        {
            foreach (var clothingItem in clothingItems)
            {
                WearClothing(clothingItem);
            }
        }

        public virtual void WearClothing(EadonClothingItem clothingItem)
        {
            var holder = GetClothingHolder(clothingItem.clothingHolderName);
            holder.AddClothing(clothingItem);
            RegenerateMask();
        }

        public ClothingEquipmentHolder GetClothingHolder(EadonClothingItem.ClothingHolderNames holderName)
        {
            return clothingEquipmentHolders.FirstOrDefault(t => t.holderName == holderName);
        }

        public virtual void SetToDefaults()
        {
            foreach (var holder in clothingEquipmentHolders)
            {
                holder.ResetToDefault();
            }
        }

        public virtual void RemoveClothing(EadonClothingItem clothingItem)
        {
            var holder = GetClothingHolder(clothingItem.clothingHolderName);
            holder.RemoveClothing();
            RegenerateMask();
        }
        
        public void SetupSubmeshes()
        {
            submeshes.Clear();
            
            var skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                submeshes.Add(skinnedMeshRenderer.name, skinnedMeshRenderer);
            }
        }

        private void RegenerateMask()
        {
            if (_maskValid)
            {
                if (mainBody != null)
                {
                    _currentMask.SetPixels(_bodyMaskTexture.GetPixels());
                    _currentMask.Apply();
            
                    foreach (var clothingEquipmentHolder in clothingEquipmentHolders)
                    {
                        var clothingGameObject = clothingEquipmentHolder.currentEquippedClothing;
                        if (clothingGameObject != null)
                        {
                            var attachClothing = clothingGameObject.GetComponent<AttachClothing>();
                            if (attachClothing != null)
                            {
                                if (attachClothing.maskTexture != null)
                                {
                                    _currentMask.SetPixels(ApplyMask(_currentMask, attachClothing.maskTexture));
                                    _currentMask.Apply();
                                }
                            }
                        }
                    }

                    mainBody.materials[mainBodyMaterialIndex].SetTexture("_Mask", _currentMask);
                }
            }
        }
        
        private Color[] ApplyMask(Texture2D source, Texture2D mask)
        {
            var sourceWidth = source.width;
            var sourcePixels = source.GetPixels();
            var maskPixels = mask.GetPixels();
            for(int j = 0; j < maskPixels.Length; j++) 
            {
                var sourceColor = maskPixels[j];
                var x = (j % sourceWidth);
                var y = (j / sourceWidth);
                var index = x + y * sourceWidth;
                if(index > 0 && index < maskPixels.Length) 
                {
                    var target = maskPixels[index];
                    if(target == Color.black) 
                    {
                        sourcePixels[index] = Color.black;
                    }
                }
            }

            return sourcePixels;
        }
        
        public virtual void SetDefaultClothing(EadonClothingItem clothingItem)
        {
            var holder = GetClothingHolder(clothingItem.clothingHolderName);
            holder.SetDefaultClothing(clothingItem, true);
            RegenerateMask();
        }

        public string SaveData()
        {
            var result = "";
            foreach (var clothingEquipmentHolder in clothingEquipmentHolders)
            {
                result += $"{clothingEquipmentHolder.holderName.ToString()}:{clothingEquipmentHolder.SaveData()};";
            }
            return result;
        }

        public void LoadData(string data)
        {
            var holders = data.Split(';');

            foreach (var holder in holders)
            {
                if (!string.IsNullOrWhiteSpace(holder))
                {
                    var holderTokens = holder.Split(':');
                    var valid = Enum.TryParse<EadonClothingItem.ClothingHolderNames>(holderTokens[0], out var holderName);
                    if (valid)
                    {
                        var clothingHolder = GetClothingHolder(holderName);
                        clothingHolder.LoadData(holderTokens[1]);
                    }
                }
            }
        }

        public void SwitchBodyMaterial(Material newMaterial, int index)
        {
            Material[] materials = mainBody.sharedMaterials;
            materials[index] = newMaterial;
            mainBody.sharedMaterials = materials;
        }

        public void RestoreBodyMaterial(int index)
        {
            Material[] materials = mainBody.sharedMaterials;
            materials[index] = _originalBodyMaterials[index];
            mainBody.sharedMaterials = materials;
        }
    }
}
#endif
