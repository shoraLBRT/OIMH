#if EADON_RPG_INVECTOR
using System;
using Eadon.Rpg.Invector.Character;
using UnityEngine;

namespace Eadon.Rpg.Invector.ClothingSystem
{
    public class ClothingEquipmentHolder : MonoBehaviour
    {
        public EadonClothingItem.ClothingHolderNames holderName;
        public bool hideDefaultWhenAddingNew;
        public GameObject currentEquippedClothing;
        public GameObject defaultEquippedClothing;
        public EadonClothingItem defaultClothingItem;
        private EadonClothingItem _currentClothingItem;
        private Material _originalMaterial;

        private EadonCharacterAppearance _appearance;
        private Transform _mainTransform;
        
        private void Start()
        {
            _appearance = gameObject.GetComponentInParent<EadonCharacterAppearance>();
            _mainTransform = _appearance.transform;
            if (defaultClothingItem != null)
            {
                AttachDefault();
            }
        }

        private void AttachDefault()
        {
            if (defaultClothingItem == null)
            {
                return;
            }
            
            _appearance = gameObject.GetComponentInParent<EadonCharacterAppearance>();
            if (_appearance == null)
            {
                return;
            }
            
            if (defaultEquippedClothing != null)
            {
                Destroy(defaultEquippedClothing);
                defaultEquippedClothing = null;
            }
            
            _appearance.beforeEquippingClothing.Invoke(defaultClothingItem.clothingId);

            // var clothingPrefab = _appearance.sex == AppearanceSex.Female
            //     ? defaultClothingItem.femaleClothingPrefab
            //     : defaultClothingItem.maleClothingPrefab;
            var clothingPrefab = defaultClothingItem.GetPrefabForTag(_appearance.prefabTag);
            
            switch (defaultClothingItem.clothingType)
            {
                case ClothingItemType.SpawnAndAttach:
                    if (clothingPrefab != null)
                    {
                        defaultEquippedClothing = Instantiate(clothingPrefab, transform);
                        if (defaultClothingItem.offset != null)
                        {
                            currentEquippedClothing.transform.localPosition = defaultClothingItem.offset.pos;
                            currentEquippedClothing.transform.localRotation = Quaternion.Euler(defaultClothingItem.offset.rot);
                        }

                        _originalMaterial = defaultEquippedClothing.GetComponent<SkinnedMeshRenderer>().material;
                    }
                    break;
                case ClothingItemType.SpawnAndAttachOnMainObject:
                    if (clothingPrefab != null)
                    {
                        defaultEquippedClothing = Instantiate(clothingPrefab, transform);
                        _originalMaterial = defaultEquippedClothing.GetComponent<SkinnedMeshRenderer>().material;
                    }
                    break;
                case ClothingItemType.ShowAndHide:
                    if (_appearance.submeshes.ContainsKey(defaultClothingItem.clothingName))
                    {
                        defaultEquippedClothing = _appearance.submeshes[defaultClothingItem.clothingName].gameObject;
                        defaultEquippedClothing.SetActive(true);
                    }

                    _originalMaterial = defaultEquippedClothing.GetComponent<SkinnedMeshRenderer>().material;
                    break;
                case ClothingItemType.ChangeTextureOnDefault:
                    _appearance.SwitchBodyMaterial(defaultClothingItem.overrideMaterial, defaultClothingItem.materialIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _appearance.afterEquippingClothing.Invoke(defaultClothingItem.clothingId);
        }

        public void AddClothing(EadonClothingItem newClothing)
        {
            _currentClothingItem = newClothing;
            
            // var clothingPrefab = _appearance.sex == AppearanceSex.Female
            //     ? newClothing.femaleClothingPrefab
            //     : newClothing.maleClothingPrefab;
            var clothingPrefab = _currentClothingItem.GetPrefabForTag(_appearance.prefabTag);

            if (currentEquippedClothing != null)
            {
                RemoveClothing();
            }

            _appearance.beforeEquippingClothing.Invoke(newClothing.clothingId);

            switch (newClothing.clothingType)
            {
                case ClothingItemType.SpawnAndAttach:
                    if (clothingPrefab != null)
                    {
                        currentEquippedClothing = Instantiate(clothingPrefab, transform);
                        if (newClothing.offset != null)
                        {
                            currentEquippedClothing.transform.localPosition = newClothing.offset.pos;
                            currentEquippedClothing.transform.localRotation = Quaternion.Euler(newClothing.offset.rot);
                        }
                        SetDefaultClothingVisibility(false);
                    }
                    break;
                case ClothingItemType.ShowAndHide:
                    if (_appearance.submeshes.ContainsKey(newClothing.clothingName))
                    {
                        var newClothingGameObject = _appearance.submeshes[newClothing.clothingName];
                        newClothingGameObject.gameObject.SetActive(true);
                    }
                    SetDefaultClothingVisibility(false);
                    break;
                case ClothingItemType.ChangeTextureOnDefault:
                    currentEquippedClothing.GetComponent<SkinnedMeshRenderer>().material = newClothing.overrideMaterial;
                    break;
                case ClothingItemType.SpawnAndAttachOnMainObject:
                    if (clothingPrefab != null)
                    {
                        SetDefaultClothingVisibility(false);
                        currentEquippedClothing = Instantiate(clothingPrefab, _mainTransform);
                        if (newClothing.offset != null)
                        {
                            currentEquippedClothing.transform.localPosition = newClothing.offset.pos;
                            currentEquippedClothing.transform.localRotation = Quaternion.Euler(newClothing.offset.rot);
                        }
                    }
                    break;
                case ClothingItemType.ChangeMaterialOnMainBody:
                    if (newClothing.overrideMaterial != null)
                    {
                        _appearance.SwitchBodyMaterial(newClothing.overrideMaterial, newClothing.materialIndex);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _appearance.afterEquippingClothing.Invoke(_currentClothingItem.clothingId);
        }

        public void SetDefaultClothingVisibility(bool status)
        {
            if (hideDefaultWhenAddingNew && defaultClothingItem != null)
            {
                if (defaultEquippedClothing != null)
                {
                    defaultEquippedClothing.SetActive(status);
                }
            }
        }

        public void RemoveClothing()
        {
            _appearance.beforeUnequippingClothing.Invoke(_currentClothingItem.clothingId);

            switch (_currentClothingItem.clothingType)
            {
                case ClothingItemType.SpawnAndAttach:
                    if (currentEquippedClothing != null)
                    {
                        Destroy(currentEquippedClothing);
                    }
                    break;
                case ClothingItemType.ShowAndHide:
                    if (_appearance.submeshes.ContainsKey(_currentClothingItem.clothingName))
                    {
                        var newClothingGameObject = _appearance.submeshes[_currentClothingItem.clothingName];
                        newClothingGameObject.gameObject.SetActive(false);
                    }
                    break;
                case ClothingItemType.ChangeTextureOnDefault:
                    currentEquippedClothing.GetComponent<SkinnedMeshRenderer>().material = _originalMaterial;
                    break;
                case ClothingItemType.SpawnAndAttachOnMainObject:
                    if (currentEquippedClothing != null)
                    {
                        Debug.Log("Destroying: " + currentEquippedClothing.name);
                        Destroy(currentEquippedClothing);
                    }
                    break;
                case ClothingItemType.ChangeMaterialOnMainBody:
                    _appearance.RestoreBodyMaterial(_currentClothingItem.materialIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            currentEquippedClothing = null;
            SetDefaultClothingVisibility(true);
            _appearance.afterUnequippingClothing.Invoke(_currentClothingItem.clothingId);
        }

        public void ResetToDefault()
        {
            if (currentEquippedClothing != null)
            {
                Destroy(currentEquippedClothing);
            }
            SetDefaultClothingVisibility(true);
        }
        
        public void SetDefaultClothing(EadonClothingItem newClothing, bool attach)
        {
            if (defaultEquippedClothing != null)
            {
                Destroy(defaultEquippedClothing);
                defaultEquippedClothing = null;
            }

            defaultClothingItem = newClothing;

            if (attach)
            {
                AttachDefault();
            }
        }

        public string SaveData()
        {
            var defaultId = defaultClothingItem != null ? defaultClothingItem.clothingId : -1;
            var currentId = _currentClothingItem != null ? _currentClothingItem.clothingId : -1;
            return $"{defaultId},{currentId}";
        }

        public void LoadData(string data)
        {
            var character = gameObject.GetComponentInParent<EadonRpgCharacterBase>();
            if (character != null)
            {
                var clothingManager = character.clothingManager;
                if (clothingManager != null)
                {
                    clothingManager.Init();
                    var ids = data.Split(',');

                    var defaultId = int.Parse(ids[0]);
                    var currentId = int.Parse(ids[1]);
            
                    if (defaultId != -1)
                    {
                        defaultClothingItem = clothingManager.GetItemInstance(defaultId);
                        AttachDefault();
                    }
            
                    if (currentId != -1)
                    {
                        _currentClothingItem = clothingManager.GetItemInstance(currentId);
                        AddClothing(_currentClothingItem);
                    }
                }
            }
        }

        public EadonClothingItem GetCurrentClothingItem()
        {
            return _currentClothingItem;
        }
    }
}
#endif
