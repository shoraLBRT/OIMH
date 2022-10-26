#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using System.IO;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.Configuration;
using Eadon.Rpg.Invector.Utils;
using Invector;
using Invector.vCharacterController.AI;
using Invector.vItemManager;
using Invector.vMelee;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;

namespace Eadon.Rpg.Invector.Editor
{
    public class CreateRpgNpcWindow : EditorWindow
    {
        private EadonRpgDefaultValues _defaultValues;
        private EadonRpgCharacterConfig _characterConfig;
        private EadonRpgRace _characterRace;
        private EadonRpgClass _characterClass;
        private EadonRpgAlignment _characterAlignment;
        private vItemListData _itemListData;
        private GameObject _itemCollectionDisplay;
        private Transform _headBone;
        private string _characterName;

        private GameObject _hitDamageParticle;
        //GameObject UIBase;

        private List<string> _bodyMeshNames;
        private SkinnedMeshRenderer[] _bodyMesh;
        private int _meshIndex = -1;
        private GameObject _lastScanned;

        private GUIStyle _redStyle;
        private GUIStyle _bgColor;

        private UnityEditor.Editor _previewEditor;
        private string _errorMessage = "";
        private List<GameObject> _conditionsGo = new List<GameObject>();

        private bool _hasvControlAi = false;
        private bool _hasAiController = false;

        private bool _rpgCharFoldout = true;
        private bool _invectorDataFoldout = true;
        private bool _conditionsFoldout = true;
        private Vector2 _scrollOffset;

        [MenuItem("Invector/Eadon RPG/Create RPG NPC")]
        public static void ShowWindow()
        {
            var window = GetWindow<CreateRpgNpcWindow>(true);
            window.titleContent = new GUIContent(WindowTitle());
            window.minSize = MinWindowSize();
            window.maxSize = MinWindowSize();
            window.SetupStyles();
            window.LoadDefaults();
            //window.ShowUtility();
        }

        private static Vector2 MinWindowSize()
        {
            return new Vector2(350, 450);
        }

        private static string WindowTitle()
        {
            return "Eadon - Create RPG NPC";
        }

        private void OnSelectionChange()
        {
            if (Selection.activeGameObject == null) return;
            DataChanged();
            ScanForLod();
        }

        private void LoadDefaults()
        {
            if (File.Exists(Application.dataPath +
                            "/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Eadon RPG Defaults.asset"))
            {
                _defaultValues =
                    AssetDatabase.LoadAssetAtPath(
                        "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Eadon RPG Defaults.asset",
                        typeof(EadonRpgDefaultValues)) as EadonRpgDefaultValues;
            }

            if (File.Exists(Application.dataPath +
                            "/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Eadon RPG Character Configuration.asset"))
            {
                _characterConfig =
                    AssetDatabase.LoadAssetAtPath(
                        "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Eadon RPG Character Configuration.asset",
                        typeof(EadonRpgCharacterConfig)) as EadonRpgCharacterConfig;
            }

            if (File.Exists(Application.dataPath + "/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Race - Human.asset"))
            {
                _characterRace =
                    AssetDatabase.LoadAssetAtPath(
                        "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Race - Human.asset",
                        typeof(EadonRpgRace)) as EadonRpgRace;
            }


            if (File.Exists(Application.dataPath +
                            "/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Class - Mage.asset"))
            {
                _characterClass =
                    AssetDatabase.LoadAssetAtPath("Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Class - Mage.asset",
                        typeof(EadonRpgClass)) as EadonRpgClass;
            }

            if (File.Exists(Application.dataPath +
                            "/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Alignment - Good.asset"))
            {
                _characterAlignment =
                    AssetDatabase.LoadAssetAtPath(
                        "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Alignment - Good.asset",
                        typeof(EadonRpgAlignment)) as EadonRpgAlignment;
            }

            if (File.Exists(Application.dataPath + "/Cogs & Goggles/Eadon RPG for Invector/Data/Invector/Eadon RPG Demo vItemList.asset"))
            {
                _itemListData =
                    AssetDatabase.LoadAssetAtPath(
                        "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Invector/Eadon RPG Demo vItemList.asset",
                        typeof(vItemListData)) as vItemListData;
            }
        }

        private void OnGUI()
        {
            var splashTexture = (Texture2D) Resources.Load("Textures/eadon_rpg_create_npc", typeof(Texture2D));
            GUILayoutUtility.GetRect(1f, 3f, GUILayout.ExpandWidth(false));
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(100f));
            GUI.DrawTexture(rect, splashTexture, ScaleMode.ScaleToFit, true, 0f);

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            _scrollOffset = EditorGUILayout.BeginScrollView(_scrollOffset);

            _rpgCharFoldout = EditorGUILayout.Foldout(_rpgCharFoldout, "Character");
            if (_rpgCharFoldout)
            {
                EditorGUI.indentLevel++;
                _defaultValues =
                    EditorGUILayout.ObjectField("Default Values", _defaultValues, typeof(EadonRpgDefaultValues), false)
                        as EadonRpgDefaultValues;
                _characterConfig =
                    EditorGUILayout.ObjectField("Character Config", _characterConfig, typeof(EadonRpgCharacterConfig),
                        false) as EadonRpgCharacterConfig;
                _characterRace =
                    EditorGUILayout.ObjectField("Race", _characterRace, typeof(EadonRpgRace), false) as EadonRpgRace;
                _characterClass =
                    EditorGUILayout.ObjectField("Class", _characterClass, typeof(EadonRpgClass),
                        false) as EadonRpgClass;
                _characterAlignment =
                    EditorGUILayout.ObjectField("Alignment", _characterAlignment, typeof(EadonRpgAlignment), false) as
                        EadonRpgAlignment;
                _characterName =
                    EditorGUILayout.TextField("Character Name", _characterName);
                EditorGUI.indentLevel--;
            }

            ScanForLod();
            _headBone =
                EditorGUILayout.ObjectField("Head Bone", _headBone, typeof(Transform), true) as Transform;

            _invectorDataFoldout = EditorGUILayout.Foldout(_invectorDataFoldout, "Invector Data");
            if (_invectorDataFoldout)
            {
                EditorGUI.indentLevel++;
                _itemListData =
                    EditorGUILayout.ObjectField("Item List", _itemListData, typeof(vItemListData), false) as
                        vItemListData;
                _hitDamageParticle =
                    EditorGUILayout.ObjectField("Hit Damage Particles", _hitDamageParticle, typeof(GameObject), false) as
                        GameObject;
                EditorGUI.indentLevel--;
            }

            _conditionsFoldout = EditorGUILayout.Foldout(_conditionsFoldout, "Conditions");
            if (_conditionsFoldout)
            {
                EditorGUI.indentLevel++;
                if (_defaultValues != null)
                {
                    SetupConditionFields();
                }

                EditorGUI.indentLevel--;
            }

            var result = DataChanged();

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            if (!result)
            {
                EditorGUILayout.LabelField(_errorMessage, _redStyle);
            }
            else
            {
                if (GUILayout.Button("Create"))
                {
                    Create();
                }
            }
        }

        private void SetupConditionFields()
        {
            _conditionsGo.Clear();
            foreach (var damageType in _characterConfig.DamageTypes)
            {
                var dtgo =
                    EditorGUILayout.ObjectField(damageType.DamageTypeName, damageType.DamageTypeConditionEffect,
                        typeof(GameObject), false) as GameObject;
                _conditionsGo.Add(dtgo);
            }
        }

        private void ScanForLod()
        {
            if (Selection.activeGameObject != null)
            {
                if (_lastScanned != Selection.activeGameObject)
                {
                    _lastScanned = Selection.activeGameObject;
                    _meshIndex = -1;
                    _bodyMeshNames = new List<string> {""};
                    _bodyMesh = null;
                    _bodyMesh = Selection.activeGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                    if (_bodyMesh != null)
                    {
                        foreach (var t in _bodyMesh)
                        {
                            _bodyMeshNames.Add(t.name);
                        }
                    }
                }
            }
            else
            {
                _meshIndex = -1;
                _bodyMeshNames = new List<string> {""};
            }

            var index = EditorGUILayout.Popup("Body Root", _meshIndex + 1, _bodyMeshNames.ToArray());
            if (index != _meshIndex)
            {
                _meshIndex = index - 1;
                DataChanged();
            }

            var animator = Selection.activeGameObject.GetComponent<Animator>();
            if (animator.avatar.isHuman)
            {
                if (_headBone == null)
                {
                    _headBone = animator.GetBoneTransform(HumanBodyBones.Head);
                }
            }
        }

        private bool DataChanged()
        {
            return ValidateFields();
        }

        private bool ValidateFields()
        {
            var result = true;
            var lines = 0;
            _errorMessage = "";

            if (Selection.activeGameObject == null)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines, "Please select an object in the hierarchy");
            }
            else
            {
                _hasvControlAi = false;
                _hasAiController = false;
#if INVECTOR_AI_TEMPLATE
                var vControlAi = Selection.activeGameObject.GetComponent(typeof(vControlAI)) as vControlAI;
                if (vControlAi != null)
                {
                    _hasvControlAi = true;
                    _hasAiController = false;
                }
#endif
                var vAiController = Selection.activeGameObject.GetComponent<vSimpleMeleeAI_Controller>();
                if (vAiController != null)
                {
                    _hasAiController = true;
                    _hasvControlAi = false;
                }

                if (!_hasvControlAi && !_hasAiController)
                {
                    result = false;
                    _errorMessage = AppendLine(_errorMessage, ref lines,
                        "Please select an Invector character in the hierarchy");
                }
            }

            if (!result) return false;

            if (_defaultValues == null)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines, "Please select an Eadon RPG Defaults object");
            }

            if (_characterConfig == null)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines,
                    "Please select an Eadon RPG Character Configuration object");
            }

            if (_characterRace == null)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines, "Please select an Eadon RPG Race object");
            }

            if (_characterClass == null)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines, "Please select an Eadon RPG Class object");
            }

            if (_characterAlignment == null)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines, "Please select an Eadon RPG Alignment object");
            }

            if (_headBone == null)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines, "Please select the Head Bone");
            }

            if (_itemListData == null)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines, "Please select a vItemListData object");
            }

            if (_meshIndex == -1)
            {
                result = false;
                _errorMessage = AppendLine(_errorMessage, ref lines, "Please select a body root for conditions");
            }

            return result;
        }

        private void Create()
        {
            if (Selection.activeGameObject != null)
            {
                // fail safe
                // add melee manager for when shooter
                if (!Selection.activeGameObject.GetComponent<vMeleeManager>())
                {
                    Selection.activeGameObject.AddComponent<vMeleeManager>();
                }

                // hit damage particle                                
                var hitDamageParticle = Selection.activeGameObject.GetComponent<vHitDamageParticle>();
                if (!hitDamageParticle)
                {
                    hitDamageParticle = Selection.activeGameObject.AddComponent<vHitDamageParticle>();
                }

                if (_hitDamageParticle != null)
                {
                    hitDamageParticle.defaultDamageEffects[0] = _hitDamageParticle;
                }

                // character
                var character = Selection.activeGameObject.GetComponent<EadonRpgCharacterBase>();
                if (!character)
                {
                    if (_hasAiController)
                    {
                        character = Selection.activeGameObject.AddComponent<EadonRpgSimpleNpc>();
                    }
                    else
                    {
#if INVECTOR_AI_TEMPLATE
                        character = Selection.activeGameObject.AddComponent<EadonRpgNpc>();
#endif
                    }
                }

                character.eadonRpgDefaultValues = _defaultValues;
                character.eadonRpgCharacterConfig = _characterConfig;
                character.currentRace = _characterRace;
                character.currentClass = _characterClass;
                character.currentAlignment = _characterAlignment;
                character.characterName = _characterName;

                character.ResetCharacter();

                // melee manager
                var meleeManager = Selection.activeGameObject.GetComponent<vMeleeManager>();
                if (!meleeManager)
                {
                    meleeManager = Selection.activeGameObject.AddComponent<vMeleeManager>();
                    meleeManager.hitProperties = new HitProperties {hitDamageTags = {[0] = "Player"}};

                    UnityEventTools.AddPersistentListener(meleeManager.onDamageHit, character.OnSendHit);
                }

                // link the ai damage to the character
#if INVECTOR_AI_TEMPLATE
                var vControlAi = Selection.activeGameObject.GetComponent(typeof(vControlAI)) as vControlAI;
                if (vControlAi != null)
                {
                    UnityEventTools.AddPersistentListener(vControlAi.onReceiveDamage, character.OnReceiveDamage);
                }
#endif
                var vAiController = Selection.activeGameObject.GetComponent<vSimpleMeleeAI_Controller>();
                if (vAiController != null)
                {
                    UnityEventTools.AddPersistentListener(vAiController.onReceiveDamage, character.OnReceiveDamage);
                }

                // add conditions and update particles to use the LOD 1 mesh
                character.conditions = new EadonConditionDictionary();
                var goConditionsRoot = new GameObject("Conditions");
                goConditionsRoot.transform.SetParent(Selection.activeGameObject.transform);
                goConditionsRoot.transform.position = new Vector3(0f, 0f, 0f);

                for(var index = 0; index < _characterConfig.DamageTypes.Length; index++)
                {
                    var damageType = _characterConfig.DamageTypes[index];

                    if (_conditionsGo[index] != null)
                    {
                        character.conditions.Add(damageType.DamageTypeName, new BaseCondition { type = damageType.DamageTypeName, display = _conditionsGo[index]});
                        SetupConditionGo(_conditionsGo[index], goConditionsRoot);
                    }
                    else
                    {
                        if (damageType.DamageTypeConditionEffect != null)
                        {
                            character.conditions.Add(damageType.DamageTypeName, new BaseCondition { type = damageType.DamageTypeName});
                            SetupConditionGo(damageType.DamageTypeConditionEffect, goConditionsRoot);
                        }
                    }
                }

                // work complete
                Close();
            }
            else
                Debug.Log("Please select the Player to add these components.");
        }

        private void SetupConditionGo(GameObject conditionPrefab, GameObject goConditionsRoot)
        {
            var goCondition = PrefabUtility.InstantiatePrefab(conditionPrefab) as GameObject;
            if (goCondition == null) return;
            goCondition.transform.SetParent(goConditionsRoot.transform);
            goCondition.transform.position = new Vector3(0f, 0f, 0f);

            // update all particles to use the mesh renderer from LOD1
            goCondition.SetActive(true);
            var conditionParticles = goCondition.GetComponentsInChildren<ParticleSystem>();
            foreach (var p in conditionParticles)
            {
                if (!p.shape.enabled) continue;
                if (p.shape.shapeType != ParticleSystemShapeType.SkinnedMeshRenderer) continue;
                var editableShape = p.shape;
                editableShape.skinnedMeshRenderer = _bodyMesh[_meshIndex];
            }

            goCondition.SetActive(false);
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
