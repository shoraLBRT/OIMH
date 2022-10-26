#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using System.Linq;
using Eadon.Rpg.Invector.Character;
using Invector.vCharacterController;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.ClothingSystem.Editor
{
    public class BodyPartConfig
    {
        public string name;
        public Transform parent;
        public bool active;
        public bool useDefaultClothing;
        public EadonClothingItem defaultItemEditor;
        public bool hideIfNewClothingEquipped;
        public EadonClothingItem.ClothingHolderNames holderName;
    }
    
    public class EadonClothingSystem : EditorWindow
    {
        private GUIStyle redStyle;
        private bool _selectionValid;
        private Vector2 _scrollPos;
        private bool _syntyModular;
        private GameObject _syntyGameObject;
        private ClothingManager _clothingManager;

        private readonly List<BodyPartConfig> bodyConfig = new List<BodyPartConfig>();
        
        [MenuItem("Invector/Eadon RPG/Add Character Clothing System")]
        private static void ShowWindow()
        {
            var window = GetWindow<EadonClothingSystem>();
            window.titleContent = new GUIContent("Character Clothing System");
            window.minSize = new Vector2(512, 800);
            window.maxSize = new Vector2(512, 800);
            window._clothingManager = Resources.Load("Clothing Manager") as ClothingManager;
            window.SetupStyles();
            window.SetupBodyParts();
            window.Show();
        }

        private void OnGUI()
        {
            var splashTexture = (Texture2D) Resources.Load("Textures/eadon_clothing_system", typeof(Texture2D));
            GUILayout.Box(splashTexture);

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            if (!CheckSelection())
            {
                EditorGUILayout.LabelField("Please select a GameObject with an Invector controller component", redStyle);
            }
            else
            {
                _clothingManager = (ClothingManager) EditorGUILayout.ObjectField("Clothing Manager", _clothingManager, typeof(ClothingManager), false);
                
                EditorGUILayout.Space();
                
                _syntyModular = EditorGUILayout.Toggle("Synty Modular Fantasy Hero", _syntyModular);
                if (_syntyModular)
                {
                    _syntyGameObject = (GameObject) EditorGUILayout.ObjectField("Modular Body", _syntyGameObject, typeof(GameObject), true);
                }
                
                EditorGUILayout.Space();
                
                var labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 150;
                
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(512), GUILayout.Height(600));

                foreach (var bodyPartConfig in bodyConfig)
                {
                    DisplayBodyPartConfig(bodyPartConfig);
                }
                
                EditorGUILayout.EndScrollView();

                EditorGUIUtility.labelWidth = labelWidth;
                
                GUILayout.FlexibleSpace();

                if (!_syntyModular || (_syntyModular && _syntyGameObject != null))
                {
                    if (GUILayout.Button("Create"))
                    {
                        CreateClothingSystem();
                    }
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        private void SetupStyles()
        {
            redStyle = new GUIStyle {normal = {textColor = Color.red}};
        }

        private static bool CheckSelection()
        {
            var currentGameObject = Selection.activeGameObject;
            if (currentGameObject == null)
            {
                return false;
            }
            return currentGameObject.GetComponent(typeof(vThirdPersonController)) != null;
        }

        private void SetupBodyParts()
        {
            var currentGameObject = Selection.activeGameObject;
            var animator = currentGameObject.GetComponentInChildren<Animator>();
            
            bodyConfig.Clear();
            
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.HeadClothing, animator.GetBoneTransform(HumanBodyBones.Head), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.HelmetClothing, animator.GetBoneTransform(HumanBodyBones.Head), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.CowlClothing, animator.GetBoneTransform(HumanBodyBones.Head), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.HatClothing, animator.GetBoneTransform(HumanBodyBones.Head), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.FaceGuardClothing, animator.GetBoneTransform(HumanBodyBones.Head), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.HelmetAccessoryClothing, animator.GetBoneTransform(HumanBodyBones.Head), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.ChestClothing, animator.GetBoneTransform(HumanBodyBones.UpperChest), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.BackClothing, animator.GetBoneTransform(HumanBodyBones.UpperChest), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.UpperRightArmClothing, animator.GetBoneTransform(HumanBodyBones.RightUpperArm), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.LowerRightArmClothing, animator.GetBoneTransform(HumanBodyBones.RightLowerArm), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.BothUpperArmClothing, animator.GetBoneTransform(HumanBodyBones.Hips), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.UpperLeftArmClothing, animator.GetBoneTransform(HumanBodyBones.LeftUpperArm), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.LowerLeftArmClothing, animator.GetBoneTransform(HumanBodyBones.LeftLowerArm), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.BothLowerArmClothing, animator.GetBoneTransform(HumanBodyBones.Hips), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.RightHandClothing, animator.GetBoneTransform(HumanBodyBones.RightHand), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.LeftHandClothing, animator.GetBoneTransform(HumanBodyBones.LeftHand), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.BothHandsClothing, animator.GetBoneTransform(HumanBodyBones.Hips), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.LegsClothing, animator.GetBoneTransform(HumanBodyBones.Hips), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.RightFootClothing, animator.GetBoneTransform(HumanBodyBones.RightLowerLeg), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.LeftFootClothing, animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.BothFeetClothing, animator.GetBoneTransform(HumanBodyBones.Hips), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.RightShoulderAttachment, animator.GetBoneTransform(HumanBodyBones.RightShoulder), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.LeftShoulderAttachment, animator.GetBoneTransform(HumanBodyBones.LeftShoulder), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.BothShoulderAttachment, animator.GetBoneTransform(HumanBodyBones.Hips), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.RightElbowAttachment, animator.GetBoneTransform(HumanBodyBones.RightLowerArm), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.LeftElbowAttachment, animator.GetBoneTransform(HumanBodyBones.LeftLowerArm), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.BothElbowAttachment, animator.GetBoneTransform(HumanBodyBones.Hips), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.RightKneeAttachment, animator.GetBoneTransform(HumanBodyBones.RightLowerLeg), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.LeftKneeAttachment, animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.BothKneeAttachment, animator.GetBoneTransform(HumanBodyBones.Hips), true));
            bodyConfig.Add(CreateBodyPart(EadonClothingItem.ClothingHolderNames.BeltAttachment, animator.GetBoneTransform(HumanBodyBones.Hips), true));
        }

        private BodyPartConfig CreateBodyPart(EadonClothingItem.ClothingHolderNames holderName, Transform bone, bool active)
        {
            var config = new BodyPartConfig {name = holderName.ToString(), parent = bone, active = active, useDefaultClothing = false, holderName = holderName};
            return config;
        }

        private void DisplayBodyPartConfig(BodyPartConfig bodyPartConfig)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(bodyPartConfig.name, EditorStyles.boldLabel);
            bodyPartConfig.active = EditorGUILayout.Toggle("Active", bodyPartConfig.active);
            if (bodyPartConfig.active)
            {
                bodyPartConfig.useDefaultClothing =
                    EditorGUILayout.Toggle("Use Default Clothing", bodyPartConfig.useDefaultClothing);
                if (bodyPartConfig.useDefaultClothing)
                {
                    bodyPartConfig.hideIfNewClothingEquipped = EditorGUILayout.Toggle("Hide If New Clothing Equipped",
                        bodyPartConfig.hideIfNewClothingEquipped);
                    bodyPartConfig.defaultItemEditor =
                        (EadonClothingItem) EditorGUILayout.ObjectField("Clothing Item", bodyPartConfig.defaultItemEditor, typeof(EadonClothingItem), false);
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void CreateClothingSystem()
        {
            var selectedObject = Selection.activeGameObject;
            var character = selectedObject.GetComponent<EadonRpgCharacterBase>();
            character.clothingManager = _clothingManager;
            
            var clothingHolders = new List<ClothingEquipmentHolder>();
            
            foreach (var bodyPartConfig in bodyConfig.Where(bodyPartConfig => bodyPartConfig.active))
            {
                var clothingHolder = new GameObject(bodyPartConfig.name);
                clothingHolder.transform.parent = bodyPartConfig.parent;
                clothingHolder.transform.localPosition = Vector3.zero;
                var clothingHolderComponent = clothingHolder.AddComponent<ClothingEquipmentHolder>();
                clothingHolderComponent.holderName = bodyPartConfig.holderName;
                clothingHolderComponent.hideDefaultWhenAddingNew = bodyPartConfig.hideIfNewClothingEquipped;
                clothingHolderComponent.defaultClothingItem = bodyPartConfig.defaultItemEditor;
                clothingHolders.Add(clothingHolderComponent);
            }

            if (_syntyModular)
            {
                var appearance = selectedObject.AddComponent<SyntyCharacterAppearance>();
                appearance.clothingEquipmentHolders = clothingHolders.ToArray();
                appearance.modularGameObject = _syntyGameObject;
                appearance.originalMaterial = _syntyGameObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
            }
            else
            {
                var appearance = selectedObject.AddComponent<EadonCharacterAppearance>();
                appearance.clothingEquipmentHolders = clothingHolders.ToArray();
            }
            
            var window = GetWindow<EadonClothingSystem>();
            window.Close();
        }
    }
}
#endif
