#if EADON_RPG_INVECTOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CogsAndGoggles.Library.Utilities.FullSerializer;
using Eadon.RPG;
using Eadon.Rpg.Invector.Configuration;
using Eadon.Rpg.Invector.Magic;
using Eadon.Rpg.Invector.Magic.Editor;
using Invector.vItemManager;
using Invector.vMelee;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Eadon.Rpg.Invector.Editor
{
    public class EadonSpellbookWindow : EditorWindow
    {
        private const string NoFilter = "No Filter";

        private EadonSpellbookDefaults _spellbookDefaults;

        private vItemListData _currentData;

        private vItemListData _spellItemData;
        private vItemListData _oldSpellItemData;

        private ScriptableObject _currentDataScriptableObject;
        private EadonRpgDefaultValues _defaultValues;
        private EadonRpgDefaultValues _oldDefaultValues;
        private EadonRpgCharacterConfig _characterConfig;
        private EadonRpgCharacterConfig _oldCharacterConfig;
        private SpellbookAnimator _targetAnimator;
        private RuntimeAnimatorController _animator;
        private RuntimeAnimatorController _oldAnimator;

        private AnimationClip _spawnClipDefault;
        private AnimationClip _oldSpawnClipDefault;
        private AnimationClip _spellCastDefault;
        private AnimationClip _oldSpellCastDefault;
        private AnimationClip _chargeInitDefault;
        private AnimationClip _oldChargeInitDefault;
        private AnimationClip _chargeHoldDefault;
        private AnimationClip _oldChargeHoldDefault;
        private AnimationClip _chargeReleaseDefault;
        private AnimationClip _oldChargeReleaseDefault;
        private GameObject _handParticleDefault;
        private GameObject _oldHandParticleDefault;

        private bool _useDamageFilter;
        private List<string> _damageFilterChoices;
        private List<string> _damageTypeChoices;
        private int _damageFilterIndex;
        private int _oldDamageFilterIndex;

        private List<EadonSpellListItem> _spellsInItemData = new List<EadonSpellListItem>();
        private List<EadonSpellListItem> _currentSpellsInItemData = new List<EadonSpellListItem>();
        private List<EadonSpellBookSpell> _activeSpellBook = new List<EadonSpellBookSpell>();
        private List<EadonSpellBookSpell> _spellBook = new List<EadonSpellBookSpell>();

        public EadonSpellBookSpell currentSpell;

        private string _messages = "";

        private GUIStyle _redStyle;

        private Vector2 _leftScrollView = Vector2.zero;
        private Vector2 _spellbookScrollView = Vector2.zero;
        private Vector2 _itemListScrollView = Vector2.zero;
        private Vector2 _spellScrollView = Vector2.zero;

        [MenuItem("Invector/Eadon RPG/Spellbook")]
        public static void ShowWindow()
        {
            var window = GetWindow<EadonSpellbookWindow>();
            window.titleContent = new GUIContent(WindowTitle());
            window.minSize = MinWindowSize();
            window.SetupStyles();
            window.Show();
            window.LoadDefaults();
            window.LoadDefaultValues();
        }

        private void SetupStyles()
        {
            _redStyle = new GUIStyle {normal = {textColor = Color.red}};
            var backgroundTexture = (Texture2D) Resources.Load("Textures/eadon_editor_background", typeof(Texture2D));
            new GUIStyle {normal = {background = backgroundTexture}};
        }

        private static Vector2 MinWindowSize()
        {
            return new Vector2(1024, 700);
        }

        private void OnGUI()
        {
            var splashTexture = (Texture2D) Resources.Load("Textures/eadon_rpg_spellbook", typeof(Texture2D));
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(128));
            GUI.DrawTexture(rect, splashTexture, ScaleMode.ScaleToFit, true, 0f);

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(350)); // left column

            _leftScrollView = EditorGUILayout.BeginScrollView(_leftScrollView);

            EditorGUILayout.LabelField("Animator", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            _animator =
                EditorGUILayout.ObjectField("Animator", _oldAnimator, typeof(RuntimeAnimatorController), false) as
                    RuntimeAnimatorController;
            if (_animator != _oldAnimator)
            {
                _oldAnimator = _animator;
                _targetAnimator = new SpellbookAnimator
                {
                    controller = _animator as AnimatorController,
                    magicLayerFullBodyIndex = 0,
                    magicLayerUpperBodyIndex = 0,
                    allLayerNames = new List<string>()
                };
                if (_targetAnimator.controller != null)
                {
                    for (var layer = 0; layer < _targetAnimator.controller.layers.Length; layer++)
                    {
                        _targetAnimator.allLayerNames.Add(_targetAnimator.controller.layers[layer].name);
                        if (_targetAnimator.controller.layers[layer].name == "FullBody")
                        {
                            _targetAnimator.magicLayerFullBodyIndex = layer;
                        }
                        else if (_targetAnimator.controller.layers[layer].name == "UpperBody")
                        {
                            _targetAnimator.magicLayerUpperBodyIndex = layer;
                        }
                    }
                }
            }

            if (_animator != null)
            {
                EditorGUILayout.LabelField("Full Body", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                var oldFullBodyIndex = _targetAnimator.magicLayerFullBodyIndex;
                _targetAnimator.magicLayerFullBodyIndex = EditorGUILayout.Popup("Full Body",
                    _targetAnimator.magicLayerFullBodyIndex, _targetAnimator.allLayerNames.ToArray());
                if (_targetAnimator.magicLayerFullBodyIndex != oldFullBodyIndex)
                {
                    if (_targetAnimator.controller != null && !_targetAnimator.controller
                        .layers[_targetAnimator.magicLayerFullBodyIndex].iKPass)
                    {
                        EditorGUILayout.LabelField("You need to set IK on this layer", _redStyle);
                    }
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Upper Body", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                var oldUpperBodyIndex = _targetAnimator.magicLayerUpperBodyIndex;
                _targetAnimator.magicLayerUpperBodyIndex = EditorGUILayout.Popup("Upper Body",
                    _targetAnimator.magicLayerUpperBodyIndex, _targetAnimator.allLayerNames.ToArray());
                if (_targetAnimator.magicLayerUpperBodyIndex != oldUpperBodyIndex)
                {
                    if (_targetAnimator.controller != null &&
                        !_targetAnimator.controller.layers[_targetAnimator.magicLayerUpperBodyIndex].iKPass)
                    {
                        EditorGUILayout.LabelField("You need to set IK on this layer", _redStyle);
                    }
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                _targetAnimator.dontSetAnimationClips =
                    EditorGUILayout.Toggle("Do NOT Set Clips", _targetAnimator.dontSetAnimationClips);

                EditorGUI.indentLevel--;

                if (GUILayout.Button("Apply To Controller"))
                {
                    ValidateController(_targetAnimator);
                }
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Defaults", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            _spawnClipDefault =
                EditorGUILayout.ObjectField("Default Spawn Clip", _oldSpawnClipDefault, typeof(AnimationClip), false) as
                    AnimationClip;
            if (_spawnClipDefault != null && _oldSpawnClipDefault != _spawnClipDefault)
            {
                _oldSpawnClipDefault = _spawnClipDefault;
                _spellbookDefaults.defaultSpawnClip = AssetDatabase.GetAssetPath(_spawnClipDefault);
                SaveDefaults();
            }

            _spellCastDefault =
                EditorGUILayout.ObjectField("Default Spell Cast", _oldSpellCastDefault, typeof(AnimationClip), false) as
                    AnimationClip;
            if (_spellCastDefault != null && _oldSpellCastDefault != _spellCastDefault)
            {
                _oldSpellCastDefault = _spellCastDefault;
                _spellbookDefaults.defaultSpellCast = AssetDatabase.GetAssetPath(_spellCastDefault);
                SaveDefaults();
            }

            _chargeInitDefault =
                EditorGUILayout.ObjectField("Default Charge Init", _oldChargeInitDefault, typeof(AnimationClip), false)
                    as AnimationClip;
            if (_chargeInitDefault != null && _oldChargeInitDefault != _chargeInitDefault)
            {
                _oldChargeInitDefault = _chargeInitDefault;
                _spellbookDefaults.defaultChargeInit = AssetDatabase.GetAssetPath(_chargeInitDefault);
                SaveDefaults();
            }

            _chargeHoldDefault =
                EditorGUILayout.ObjectField("Default Charge Hold", _oldChargeHoldDefault, typeof(AnimationClip), false)
                    as AnimationClip;
            if (_chargeHoldDefault != null && _oldChargeHoldDefault != _chargeHoldDefault)
            {
                _oldChargeHoldDefault = _chargeHoldDefault;
                _spellbookDefaults.defaultChargeHold = AssetDatabase.GetAssetPath(_chargeHoldDefault);
                SaveDefaults();
            }

            _chargeReleaseDefault =
                EditorGUILayout.ObjectField("Default Charge Release", _oldChargeReleaseDefault, typeof(AnimationClip),
                    false) as AnimationClip;
            if (_handParticleDefault != null && _oldChargeReleaseDefault != _chargeReleaseDefault)
            {
                _oldChargeReleaseDefault = _chargeReleaseDefault;
                _spellbookDefaults.defaultChargeRelease = AssetDatabase.GetAssetPath(_chargeReleaseDefault);
                SaveDefaults();
            }

            _handParticleDefault =
                EditorGUILayout.ObjectField("Default Hand Particles", _oldHandParticleDefault, typeof(GameObject),
                    false) as GameObject;
            if (_handParticleDefault != null && _oldHandParticleDefault != _handParticleDefault)
            {
                _oldHandParticleDefault = _handParticleDefault;
                _spellbookDefaults.defaultHandParticle = AssetDatabase.GetAssetPath(_handParticleDefault);
                SaveDefaults();
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Available Spells", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            _spellItemData =
                EditorGUILayout.ObjectField("Item Data", _oldSpellItemData, typeof(vItemListData), false) as
                    vItemListData;
            if (_spellItemData != null && _oldSpellItemData != _spellItemData)
            {
                _oldSpellItemData = _spellItemData;
                _spellbookDefaults.defaultItemList = AssetDatabase.GetAssetPath(_spellItemData);
                SaveDefaults();
                if (_spellItemData != _currentData)
                {
                    _currentData = _spellItemData;

                    LoadSpellsFromItemList();
                }

                if (GUILayout.Button("Edit Spells In Item List"))
                {
                    vItemListWindow.CreateWindow(_spellItemData);
                }
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("RPG Default Values", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            _defaultValues =
                EditorGUILayout.ObjectField("Default Values", _oldDefaultValues, typeof(EadonRpgDefaultValues), false)
                    as EadonRpgDefaultValues;
            if (_defaultValues != null && _oldDefaultValues != _defaultValues)
            {
                _spellbookDefaults.defaultRpgDefaults = AssetDatabase.GetAssetPath(_defaultValues);
                SaveDefaults();
                _oldDefaultValues = _defaultValues;
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("RPG Character Config", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            _characterConfig =
                EditorGUILayout.ObjectField("Character Config", _oldCharacterConfig, typeof(EadonRpgCharacterConfig),
                    false) as EadonRpgCharacterConfig;
            if (_characterConfig != null && _oldCharacterConfig != _characterConfig)
            {
                _oldCharacterConfig = _characterConfig;
                _spellbookDefaults.defaultCharacterConfig = AssetDatabase.GetAssetPath(_characterConfig);
                SaveDefaults();
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save Spellbook"))
            {
                SaveSpellBook();
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); // right main column

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            _useDamageFilter = EditorGUILayout.Toggle("Filter By Damage Type", _useDamageFilter);
            if (_useDamageFilter)
            {
                GUILayout.FlexibleSpace();

                _damageFilterIndex =
                    EditorGUILayout.Popup("Damage Type", _oldDamageFilterIndex, _damageFilterChoices.ToArray());
                if (_damageFilterIndex != _oldDamageFilterIndex)
                {
                    _oldDamageFilterIndex = _damageFilterIndex;
                    ApplyDamageTypeFilter();
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(200));

            EditorGUILayout.LabelField("Spells In Spellbook", EditorStyles.boldLabel);

            _spellbookScrollView = EditorGUILayout.BeginScrollView(_spellbookScrollView);

            // display list
            foreach (var spell in _spellBook)
            {
                if (GUILayout.Button(new GUIContent(spell.ToString(), spell.Icon), GUILayout.Height(64),
                    GUILayout.Width(190)))
                {
                    currentSpell = spell;
                }
            }

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField("Spells In vItemListData", EditorStyles.boldLabel);

            _itemListScrollView = EditorGUILayout.BeginScrollView(_itemListScrollView);

            // display list
            foreach (var item in _currentSpellsInItemData)
            {
                if (GUILayout.Button(new GUIContent(item.ToString(), item.icon), GUILayout.Height(64),
                    GUILayout.Width(190)))
                {
                }
            }

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            //Spell Content
            if (currentSpell != null)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                Rect iconRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(64),
                    GUILayout.Width(64));
                GUI.DrawTexture(iconRect, currentSpell.Icon, ScaleMode.ScaleToFit, true, 0f);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                currentSpell.SpellName = EditorGUILayout.TextField("Spell Name", currentSpell.SpellName);
                currentSpell.MagicID = EditorGUILayout.IntField("Magic ID", currentSpell.MagicID);
                currentSpell.ManaCost = EditorGUILayout.IntField("Mana Cost", currentSpell.ManaCost);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Icon");
                var icon = (Texture2D) EditorGUILayout.ObjectField(currentSpell.Icon, typeof(Texture2D),
                    false);
                if (icon != currentSpell.Icon)
                {
                    currentSpell.Icon = icon;
                    currentSpell.IconAssetPath = AssetDatabase.GetAssetPath(icon);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();

                EditorGUILayout.EndHorizontal();

                var spellDamageIndex = _damageTypeChoices.IndexOf(currentSpell.DamageTypeName);
                spellDamageIndex =
                    EditorGUILayout.Popup("Base Damage Name", spellDamageIndex, _damageTypeChoices.ToArray());
                if (spellDamageIndex != -1)
                {
                    currentSpell.DamageTypeName = _damageTypeChoices[spellDamageIndex];
                    currentSpell.DamageType = _characterConfig.GetDamageType(currentSpell.DamageTypeName);
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                currentSpell.UpperBodyOnly = EditorGUILayout.Toggle("Upper Body Only", currentSpell.UpperBodyOnly);

                EditorGUILayout.EndHorizontal();

                _spellScrollView = EditorGUILayout.BeginScrollView(_spellScrollView);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Animator", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                var sectionClip = (AnimationClip) EditorGUILayout.ObjectField("Spell Cast Clip",
                    currentSpell.sectionClip, typeof(AnimationClip), false);
                if (sectionClip != currentSpell.sectionClip)
                {
                    currentSpell.sectionClip = sectionClip;
                    currentSpell.sectionClipAssetPath = AssetDatabase.GetAssetPath(sectionClip);
                }

                EditorGUILayout.BeginHorizontal();
                currentSpell.mirror = EditorGUILayout.Toggle("Mirror", currentSpell.mirror);
                currentSpell.speed = EditorGUILayout.FloatField("Speed", currentSpell.speed);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                currentSpell.footIk = EditorGUILayout.Toggle("Foot IK", currentSpell.footIk);
                currentSpell.cycleOffset = EditorGUILayout.FloatField("Cycle Offset", currentSpell.cycleOffset);
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Hand Particles", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                var leftHandParticleEffect = (GameObject) EditorGUILayout.ObjectField("Left Hand",
                    currentSpell.leftHandParticleEffect, typeof(GameObject), false);
                if (leftHandParticleEffect != currentSpell.leftHandParticleEffect)
                {
                    currentSpell.leftHandParticleEffect = leftHandParticleEffect;
                    currentSpell.leftHandParticleEffectAssetPath = AssetDatabase.GetAssetPath(leftHandParticleEffect);
                }

                var rightHandParticleEffect = (GameObject) EditorGUILayout.ObjectField("Right Hand",
                    currentSpell.rightHandParticleEffect, typeof(GameObject), false);
                if (rightHandParticleEffect != currentSpell.rightHandParticleEffect)
                {
                    currentSpell.rightHandParticleEffect = rightHandParticleEffect;
                    currentSpell.rightHandParticleEffectAssetPath = AssetDatabase.GetAssetPath(rightHandParticleEffect);
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Spawn Prefabs", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                if (GUILayout.Button("Add Spawn Prefab"))
                {
                    var newSpawner = new EadonSpellBookSpawnPrefab
                    {
                        spawnStartTime = .5f,
                        spawnEndTime = .5f,
                        numberToSpawn = 1
                    };
                    currentSpell.spawnOverTime.Add(newSpawner);
                }

                foreach (var spawnPrefab in currentSpell.spawnOverTime.ToList())
                {
                    DisplaySpawnPrefab(spawnPrefab, currentSpell);
                }

                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndScrollView();

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            if (GUILayout.Button("Create New Spell"))
            {
                Debug.Log("Creating new spell");
                NewSpell();
            }

            if (GUILayout.Button("Add To vItemListData"))
            {
                Debug.Log("Add spell to vItemData");
                AddSpellToItemData();
            }

            if (GUILayout.Button("Delete Spell"))
            {
                Debug.Log("Delete spell");
                DeleteSpell();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            EditorGUILayout.LabelField(_messages, _redStyle);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            _currentData = _spellItemData;
        }

        private void DisplaySpawnPrefab(EadonSpellBookSpawnPrefab spawnPrefab, EadonSpellBookSpell entry)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var prefab =
                (GameObject) EditorGUILayout.ObjectField("Prefab", spawnPrefab.prefab, typeof(GameObject), false);
            if (prefab != spawnPrefab.prefab)
            {
                spawnPrefab.prefab = prefab;
                spawnPrefab.prefabAssetPath = AssetDatabase.GetAssetPath(prefab);
            }

            EditorGUILayout.BeginHorizontal();
            spawnPrefab.spawnStartTime = EditorGUILayout.FloatField("Spawn Start", spawnPrefab.spawnStartTime);
            spawnPrefab.spawnEndTime = EditorGUILayout.FloatField("End", spawnPrefab.spawnEndTime);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            spawnPrefab.numberToSpawn = EditorGUILayout.IntField("Number To Spawn", spawnPrefab.numberToSpawn);
            spawnPrefab.destructionTimeOut =
                EditorGUILayout.FloatField("Destruction Timeout", spawnPrefab.destructionTimeOut);
            EditorGUILayout.EndHorizontal();

            var spawnAudioClip =
                (AudioClip) EditorGUILayout.ObjectField("Audio Clip", spawnPrefab.spawnAudioClip, typeof(AudioClip),
                    false);
            if (spawnAudioClip != spawnPrefab.spawnAudioClip)
            {
                spawnPrefab.spawnAudioClip = spawnAudioClip;
                spawnPrefab.spawnAudioClipAssetPath = AssetDatabase.GetAssetPath(spawnAudioClip);
            }

            var audioSource =
                (AudioSource) EditorGUILayout.ObjectField("Audio Source", spawnPrefab.audioSource, typeof(AudioSource),
                    false);
            if (audioSource != spawnPrefab.audioSource)
            {
                spawnPrefab.audioSource = audioSource;
                spawnPrefab.audioSourceAssetPath = AssetDatabase.GetAssetPath(audioSource);
            }

            EditorGUILayout.LabelField("Advanced Spawning", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            spawnPrefab.offset = EditorGUILayout.Vector3Field("Offset", spawnPrefab.offset);
            spawnPrefab.angle = EditorGUILayout.Vector3Field("Angle", spawnPrefab.angle);

            EditorGUILayout.BeginHorizontal();
            spawnPrefab.keepParent = EditorGUILayout.Toggle("Keep Parent", spawnPrefab.keepParent);
            spawnPrefab.useRootTransform = EditorGUILayout.Toggle("Use Root Transform", spawnPrefab.useRootTransform);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;

            if (GUILayout.Button("Delete"))
            {
                entry.spawnOverTime.Remove(spawnPrefab);
            }

            EditorGUILayout.EndVertical();
        }

        private void LoadDefaultValues()
        {
            _spellsInItemData = new List<EadonSpellListItem>();
            _currentSpellsInItemData = new List<EadonSpellListItem>();

            if (File.Exists(Application.dataPath + _spellbookDefaults.defaultSpawnClip.Substring(6)))
            {
                _spawnClipDefault = _oldSpawnClipDefault =
                    AssetDatabase.LoadAssetAtPath(_spellbookDefaults.defaultSpawnClip, typeof(AnimationClip)) as
                        AnimationClip;
            }

            if (File.Exists(Application.dataPath + _spellbookDefaults.defaultSpellCast.Substring(6)))
            {
                _spellCastDefault = _oldSpellCastDefault =
                    AssetDatabase.LoadAssetAtPath(_spellbookDefaults.defaultSpellCast, typeof(AnimationClip)) as
                        AnimationClip;
            }

            if (File.Exists(Application.dataPath + _spellbookDefaults.defaultChargeInit.Substring(6)))
            {
                _chargeInitDefault = _oldChargeInitDefault =
                    AssetDatabase.LoadAssetAtPath(_spellbookDefaults.defaultChargeInit, typeof(AnimationClip)) as
                        AnimationClip;
            }

            if (File.Exists(Application.dataPath + _spellbookDefaults.defaultChargeHold.Substring(6)))
            {
                _chargeHoldDefault = _oldChargeHoldDefault =
                    AssetDatabase.LoadAssetAtPath(_spellbookDefaults.defaultChargeHold, typeof(AnimationClip)) as
                        AnimationClip;
            }

            if (File.Exists(Application.dataPath + _spellbookDefaults.defaultChargeRelease.Substring(6)))
            {
                _chargeReleaseDefault = _oldChargeReleaseDefault =
                    AssetDatabase.LoadAssetAtPath(_spellbookDefaults.defaultChargeRelease, typeof(AnimationClip)) as
                        AnimationClip;
            }

            if (File.Exists(Application.dataPath + _spellbookDefaults.defaultHandParticle.Substring(6)))
            {
                _handParticleDefault = _oldHandParticleDefault =
                    AssetDatabase.LoadAssetAtPath(_spellbookDefaults.defaultHandParticle, typeof(GameObject)) as
                        GameObject;
            }

            if (File.Exists(Application.dataPath + _spellbookDefaults.defaultItemList.Substring(6)))
            {
                _spellItemData = _oldSpellItemData =
                    AssetDatabase.LoadAssetAtPath(_spellbookDefaults.defaultItemList, typeof(vItemListData)) as
                        vItemListData;
                _currentData = _spellItemData;
            }

            if (File.Exists(Application.dataPath + _spellbookDefaults.defaultRpgDefaults.Substring(6)))
            {
                _defaultValues = _oldDefaultValues =
                    AssetDatabase.LoadAssetAtPath(_spellbookDefaults.defaultRpgDefaults, typeof(EadonRpgDefaultValues))
                        as EadonRpgDefaultValues;
            }

            if (File.Exists(Application.dataPath + _spellbookDefaults.defaultCharacterConfig.Substring(6)))
            {
                _characterConfig = _oldCharacterConfig =
                    AssetDatabase.LoadAssetAtPath(_spellbookDefaults.defaultCharacterConfig,
                        typeof(EadonRpgCharacterConfig)) as EadonRpgCharacterConfig;
                _damageFilterChoices = new List<string> {NoFilter};
                if (_characterConfig != null)
                {
                    _damageFilterChoices.AddRange(_characterConfig.GetDamageTypeNames());
                }

                _damageTypeChoices = new List<string>();
                if (_characterConfig != null)
                {
                    _damageTypeChoices.AddRange(_characterConfig.GetDamageTypeNames());
                }
            }

            LoadSpellbook();

            LoadSpellsFromItemList();
        }

        private void LoadSpellsFromItemList()
        {
            if (_currentData == null) return;

            _spellsInItemData.Clear();
            _currentSpellsInItemData.Clear();

            foreach (var item in _currentData.items)
            {
                if (item != null && item.type == vItemType.Spell)
                {
                    var spell = new EadonSpellListItem
                    {
                        spellName = item.name,
                        magicId = item.attributes.Find(ai => ai.name.ToString() == "MagicID").value,
                        manaCost = item.attributes.Find(ai => ai.name.ToString() == "ManaCost").value,
                        icon = item.iconTexture,
                    };
                    if (item.attributes.Find(ai => ai.name.ToString() == "DamageType") != null)
                    {
                        spell.damageType = _characterConfig
                            .DamageTypes[item.attributes.Find(ai => ai.name.ToString() == "DamageType").value]
                            .DamageTypeName;
                    }
                    _spellsInItemData.Add(spell);
                    _currentSpellsInItemData.Add(spell);
                }
            }
        }

        private void LoadSpellbook()
        {
            if (File.Exists(Application.dataPath +
                            "/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Eadon Spellbook.asset"))
            {
                var textAsset =
                    AssetDatabase.LoadAssetAtPath(
                        "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Eadon Spellbook.asset",
                        typeof(TextAsset)) as TextAsset;
                if (textAsset == null) return;
                var content = textAsset.text;
                _spellBook =
                    (List<EadonSpellBookSpell>) StringSerialization.Deserialize(typeof(List<EadonSpellBookSpell>),
                        content);
                foreach (var spell in _spellBook)
                {
                    if (spell.IconAssetPath != "")
                    {
                        spell.Icon = AssetDatabase.LoadAssetAtPath(spell.IconAssetPath, typeof(Texture2D)) as Texture2D;
                    }

                    if (spell.DamageTypeName != "")
                    {
                        spell.DamageType = _characterConfig.GetDamageType(spell.DamageTypeName);
                    }

                    if (spell.sectionClipAssetPath != "")
                    {
                        spell.sectionClip =
                            AssetDatabase.LoadAssetAtPath(spell.sectionClipAssetPath,
                                typeof(AnimationClip)) as AnimationClip;
                    }

                    if (spell.leftHandParticleEffectAssetPath != "")
                    {
                        spell.leftHandParticleEffect =
                            AssetDatabase.LoadAssetAtPath(spell.leftHandParticleEffectAssetPath,
                                typeof(GameObject)) as GameObject;
                    }

                    if (spell.rightHandParticleEffectAssetPath != "")
                    {
                        spell.rightHandParticleEffect =
                            AssetDatabase.LoadAssetAtPath(spell.rightHandParticleEffectAssetPath,
                                typeof(GameObject)) as GameObject;
                    }

                    foreach (var spawnPrefab in spell.spawnOverTime)
                    {
                        if (spawnPrefab.prefabAssetPath != "")
                        {
                            spawnPrefab.prefab =
                                AssetDatabase.LoadAssetAtPath(spawnPrefab.prefabAssetPath, typeof(GameObject)) as
                                    GameObject;
                        }

                        if (spawnPrefab.spawnAudioClipAssetPath != "")
                        {
                            spawnPrefab.spawnAudioClip =
                                AssetDatabase.LoadAssetAtPath(spawnPrefab.spawnAudioClipAssetPath,
                                    typeof(AudioClip)) as AudioClip;
                        }

                        if (spawnPrefab.audioSourceAssetPath != "")
                        {
                            spawnPrefab.audioSource =
                                AssetDatabase.LoadAssetAtPath(spawnPrefab.audioSourceAssetPath,
                                    typeof(AudioSource)) as AudioSource;
                        }
                    }
                }
            }
            else
            {
                SaveSpellBook();
            }
        }

        private void DeleteSpell()
        {
            _spellBook.Remove(currentSpell);
        }

        private void AddSpellToItemData()
        {
            if (_currentData == null || currentSpell == null) return;
            var newSpell = ScriptableObject.CreateInstance<vItem>();
            newSpell.id = _currentData.items.Count;
            newSpell.name = currentSpell.SpellName;
            var magicIdAttribute = new vItemAttribute(vItemAttributes.MagicID, currentSpell.MagicID);
            newSpell.attributes.Add(magicIdAttribute);
            var manaCostAttribute = new vItemAttribute(vItemAttributes.ManaCost, currentSpell.ManaCost);
            newSpell.attributes.Add(manaCostAttribute);
            var damageTypeAttribute = new vItemAttribute(vItemAttributes.DamageType,
                _characterConfig.GetDamageTypeIndex(currentSpell.DamageTypeName));
            newSpell.attributes.Add(damageTypeAttribute);
            newSpell.type = vItemType.Spell;
            if (currentSpell.Icon != null)
            {
                newSpell.icon = Sprite.Create(currentSpell.Icon,
                    new Rect(0, 0, currentSpell.Icon.width, currentSpell.Icon.height), new Vector2(0.5f, 0.5f));
            }
            newSpell.stackable = true;
            newSpell.maxStack = 1;
            newSpell.destroyAfterUse = false;
            newSpell.canBeDestroyed = false;
            newSpell.canBeDroped = false;

            if (newSpell.name.Contains("(Clone)"))
            {
                newSpell.name = newSpell.name.Replace("(Clone)", string.Empty);
            }

            var serializedObject = new SerializedObject(_currentData);

            if (newSpell && !_currentData.items.Find(it => ToClearUpper(it.name).Equals(ToClearUpper(newSpell.name))))
            {
                AssetDatabase.AddObjectToAsset(newSpell, AssetDatabase.GetAssetPath(_currentData));
                newSpell.hideFlags = HideFlags.HideInHierarchy;

                if (_currentData.items.Exists(it => it.id.Equals(newSpell.id)))
                    newSpell.id = GetUniqueId(_currentData.items);
                _currentData.items.Add(newSpell);
                OrderById(ref _currentData.items);
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_currentData);
                AssetDatabase.SaveAssets();
            }

            SaveSpellBook();

            Debug.Log("Spell added... refreshing list");
            LoadSpellsFromItemList();
        }

        private static string ToClearUpper(string target)
        {
            return target.Replace(" ", string.Empty).ToUpper();
        }

        private static int GetUniqueId(List<vItem> items, int value = 0)
        {
            var result = value;

            for (int i = 0; i < items.Count + 1; i++)
            {
                var item = items.Find(t => t.id == i);
                if (item == null)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        private static void OrderById(ref List<vItem> items)
        {
            if (items != null && items.Count > 0)
            {
                items = items.OrderBy(i => i.id).ToList();
            }
        }

        private void SaveSpellBook()
        {
            var json = StringSerialization.Serialize(typeof(List<EadonSpellBookSpell>), _spellBook);
            var textAsset = new TextAsset(json);
            AssetDatabase.CreateAsset(textAsset,
                "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Eadon Spellbook.asset");
        }

        private void NewSpell()
        {
            currentSpell = new EadonSpellBookSpell {SpellName = "New Spell"};
            if (_characterConfig != null && _characterConfig.DamageTypes.Length > 0)
            {
                currentSpell.DamageType = _characterConfig.DamageTypes[0];
                currentSpell.DamageTypeName = _characterConfig.DamageTypes[0].DamageTypeName;
            }

            if (_spellbookDefaults != null && !string.IsNullOrEmpty(_spellbookDefaults.defaultSpellCast))
            {
                currentSpell.sectionClip =
                    AssetDatabase.LoadAssetAtPath(_spellbookDefaults.defaultSpellCast, typeof(AnimationClip)) as
                        AnimationClip;
                currentSpell.sectionClipAssetPath = _spellbookDefaults.defaultSpellCast;
            }

            currentSpell.spawnOverTime = new List<EadonSpellBookSpawnPrefab>();
            currentSpell.speed = 1;
            currentSpell.leftHandParticleEffect = _handParticleDefault;
            currentSpell.leftHandParticleEffectAssetPath =
                AssetDatabase.GetAssetPath(_handParticleDefault);
            currentSpell.rightHandParticleEffect = _handParticleDefault;
            currentSpell.rightHandParticleEffectAssetPath =
                AssetDatabase.GetAssetPath(_handParticleDefault);

            _spellBook.Add(currentSpell);

            ValidateCurrentSpell();
        }

        private void ApplyDamageTypeFilter()
        {
            Debug.Log("Filter set to " + _damageFilterChoices[_damageFilterIndex]);
            _currentSpellsInItemData = GetSpellsForList();

            _activeSpellBook = GetSpellbookSpellsForList();
        }

        private List<EadonSpellListItem> GetSpellsForList()
        {
            if (_useDamageFilter && _damageFilterChoices[_damageFilterIndex] != NoFilter)
            {
                var damageType = _damageFilterChoices[_damageFilterIndex];
                return _spellsInItemData.FindAll(s => s.damageType == damageType);
            }
            else
            {
                return _spellsInItemData;
            }
        }

        private List<EadonSpellBookSpell> GetSpellbookSpellsForList()
        {
            if (_useDamageFilter && _damageFilterChoices[_damageFilterIndex] != NoFilter)
            {
                return _spellBook.FindAll(s => s.DamageTypeName == _damageFilterChoices[_damageFilterIndex]);
            }

            return _spellBook;
        }

        private static string WindowTitle()
        {
            return "Eadon - Spellbook";
        }

        private void LoadDefaults()
        {
            if (File.Exists(Application.dataPath +
                            "/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Eadon Spellbook Defaults.asset"))
            {
                var textAsset =
                    AssetDatabase.LoadAssetAtPath(
                        "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Eadon Spellbook Defaults.asset",
                        typeof(TextAsset)) as TextAsset;
                if (textAsset != null)
                {
                    var content = textAsset.text;
                    _spellbookDefaults =
                        (EadonSpellbookDefaults) StringSerialization.Deserialize(typeof(EadonSpellbookDefaults),
                            content);
                }
            }
            else
            {
                _spellbookDefaults = new EadonSpellbookDefaults();
                SaveDefaults();
            }
        }

        private void SaveDefaults()
        {
            var json = StringSerialization.Serialize(typeof(EadonSpellbookDefaults), _spellbookDefaults);
            var textAsset = new TextAsset(json);
            AssetDatabase.CreateAsset(textAsset,
                "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Eadon Spellbook Defaults.asset");
        }


        private void ValidateCurrentSpell()
        {
            var duplicateMagicId = false;
            _messages = "";

            var magicId = currentSpell.MagicID;
            var count = _spellBook.FindAll(s => s.MagicID == magicId).Count;
            if (count > 1)
            {
                duplicateMagicId = true;
                _messages += "Duplicate MagicID ";
            }

            currentSpell.valid = !duplicateMagicId;
        }

        /// <summary>
        /// Validate the current animator controller
        /// </summary>
        private void ValidateController(SpellbookAnimator anim)
        {
            // magic attack triggers and ID
            if (anim.controller.parameters.Count(a => a.name == "MagicAttack") == 0)
            {
                anim.controller.AddParameter("MagicAttack", AnimatorControllerParameterType.Trigger);
            }

            if (anim.controller.parameters.Count(a => a.name == "MagicCharge") == 0)
            {
                anim.controller.AddParameter("MagicCharge", AnimatorControllerParameterType.Trigger);
            }

            if (anim.controller.parameters.Count(a => a.name == "MagicID") == 0)
            {
                anim.controller.AddParameter("MagicID", AnimatorControllerParameterType.Int);
            }


            // check layers have IK
            anim.controller.layers[anim.magicLayerFullBodyIndex].iKPass = true;
            anim.controller.layers[anim.magicLayerUpperBodyIndex].iKPass = true;


            // magic container state machine on full body    
            var magicContainerFullBody = anim.controller.layers[anim.magicLayerFullBodyIndex].stateMachine;
            if (!anim.dedicatedFullBodyLayer)
            {
                magicContainerFullBody =
                    FindStateMachine("Magic", anim.controller.layers[anim.magicLayerFullBodyIndex].stateMachine);
                if (magicContainerFullBody == null)
                {
                    magicContainerFullBody = anim.controller.layers[anim.magicLayerFullBodyIndex].stateMachine
                        .AddStateMachine("Magic");
                }
            }

            TidyStateMachineChildren(magicContainerFullBody);


            // magic container state machine on upper body
            var magicContainerUpperBody =
                FindStateMachine("Magic", anim.controller.layers[anim.magicLayerUpperBodyIndex].stateMachine);
            if (magicContainerUpperBody == null)
            {
                magicContainerUpperBody = anim.controller.layers[anim.magicLayerUpperBodyIndex].stateMachine
                    .AddStateMachine("Magic");
            }

            TidyStateMachineChildren(magicContainerUpperBody);

            // find destination magic null state on full body
            var attacksContainer =
                FindStateMachine("Attacks", anim.controller.layers[anim.magicLayerFullBodyIndex].stateMachine);
            AnimatorStateMachine nullFullBody;
            if (attacksContainer != null) // found attack state machine
            {
                nullFullBody = ValidateNullStateMachine(attacksContainer); // ensure null exists
            }
            else // not found, find/create null on the root
            {
                nullFullBody = ValidateNullStateMachine(anim.controller.layers[anim.magicLayerFullBodyIndex]
                    .stateMachine); // ensure null exists                
            }

            // find destination magic null state on upper body
            var nullUpperBody = ValidateNullStateMachine(anim.controller.layers[anim.magicLayerUpperBodyIndex].stateMachine);

            // check all magic spells are inside the state
            foreach (var vi in _currentData.items)
            {
                // only interested in the spells
                if (vi != null && vi.type == vItemType.Spell)
                {
                    // magic ID is not optional
                    var vAttribMagicId = vi.attributes.Find(ai => ai.name.ToString() == "MagicID");
                    if (vAttribMagicId == null) continue;
                    // find the spell options
                    //SpellBookListEntry SpellDetail = SpellBook.Find(s => s.MagicID == vAttribMagicID.value);
                    var spell = _spellBook.Find(s => s.MagicID == vAttribMagicId.value);
                    SetupSpell(anim, magicContainerFullBody, magicContainerUpperBody, nullFullBody, nullUpperBody, vi,
                        vAttribMagicId, spell);
                }
            }
        }

        private void SetupSpell(SpellbookAnimator anim, AnimatorStateMachine magicContainerFullBody,
            AnimatorStateMachine magicContainerUpperBody, AnimatorStateMachine nullFullBody, AnimatorStateMachine nullUpperBody,
            vItem vi, vItemAttribute vAttribMagicId, EadonSpellBookSpell spell)
        {
            if (spell == null) return;

            // allow movement whilst casting?
            var parent = (spell.UpperBodyOnly ? magicContainerUpperBody : magicContainerFullBody);
            var destination = (spell.UpperBodyOnly ? nullUpperBody : nullFullBody);

            // validate the spell in the animator
            // check this spell hasn't just changed between move and fixed layers
            var parentInverted = (spell.UpperBodyOnly ? magicContainerFullBody : magicContainerUpperBody);
            var removeMe = FindState(vi.name + " (" + vAttribMagicId.value + ")", parentInverted);
            if (removeMe != null)
            {
                parentInverted.RemoveState(removeMe);
            }

            // create if state doesn't exist
            SetupAnimatorState(
                parent,
                anim.controller
                    .layers[(spell.UpperBodyOnly ? anim.magicLayerUpperBodyIndex : anim.magicLayerFullBodyIndex)]
                    .stateMachine,
                vi.name + " (" + vAttribMagicId.value + ")", spell.sectionClip, anim.dontSetAnimationClips,
                spell.mirror, spell.speed, spell.footIk, spell.cycleOffset,
                true, null, new[]
                {
                    new AnimatorCondition()
                        {parameter = "MagicAttack", mode = AnimatorConditionMode.If, threshold = 0},
                    new AnimatorCondition()
                    {
                        parameter = "MagicID", mode = AnimatorConditionMode.Equals,
                        threshold = vAttribMagicId.value
                    }
                },
                null, destination, spell, /*meleeAttack,*/ false, true
            );
        }

        /// <summary>
        /// Check the state exists and has the correct transitions, motion and conditions.
        /// </summary>
        /// <param name="parent">Parent state machine to check for the state.</param>
        /// <param name="rootState"></param>
        /// <param name="stateName">Name of the state to check.</param>
        /// <param name="theClip">Motion clip to apply.</param>
        /// <param name="dontApplyClip">Set don't apply when applying to a non human creature (but u have to manually set the animations).</param>
        /// <param name="mirror"></param>
        /// <param name="speed"></param>
        /// <param name="footIk"></param>
        /// <param name="cycleOffset"></param>
        /// <param name="sourceAnyState">Entry transition is from any state.</param>
        /// <param name="sourceState">Source state for the spell transition, null if none.</param>
        /// <param name="spellConditions">Conditions to enter, null if none.</param>
        /// <param name="destinationState">Destination state for the exit transition, null if none.</param>
        /// <param name="destinationStateMachine">Destination state machine for the exit transition, null if none.</param>
        /// <param name="spell"></param>
        /// <param name="spellHasExitTime"></param>
        /// <param name="exitHasExitTime"></param>
        /// <returns>Count of the number of changes applied.</returns>
        private static void SetupAnimatorState(
            AnimatorStateMachine parent, AnimatorStateMachine rootState,
            string stateName, AnimationClip theClip,
            bool dontApplyClip, bool mirror, float speed, bool footIk, float cycleOffset,
            bool sourceAnyState, AnimatorState sourceState,
            AnimatorCondition[] spellConditions,
            AnimatorState destinationState, AnimatorStateMachine destinationStateMachine,
            EadonSpellBookSpell spell, /*vMeleeAttackControl meleeAttackBehavior,*/
            bool spellHasExitTime, bool exitHasExitTime)
        {
            // create the state if it doesn't exist
            var stateChangesMade = 0;
            var theState = FindState(stateName, parent);
            if (theState == null)
            {
                // state not found
                theState = parent.AddState(stateName);
                stateChangesMade += 1;
            }


            // apply the clip if required
            if (theState.motion != theClip)
            {
                // motion clip is different
                if (!dontApplyClip) // update the animation
                {
                    // update the clip
                    theState.motion = theClip;
                    theState.mirror = mirror;
                    theState.speed = speed;
                    theState.iKOnFeet = footIk;
                    theState.cycleOffset = cycleOffset;
                    stateChangesMade += 1;
                }
            }

            // remove existing behaviors
            if (theState.behaviours.Count(b => b.GetType() == typeof(vMeleeAttackControl)) == 0)
            {
                theState.behaviours = new StateMachineBehaviour[0];
            }

            // check spell options
            if (spell != null)
            {
                AddSpellBookAttack(theState, spell);
            }

            // check the source transition
            AnimatorStateTransition spellTransition = null;
            if (sourceAnyState)
            {
                // check the transition
                if (rootState.anyStateTransitions.Count(t => t.destinationState == theState) == 0)
                {
                    // not found add it
                    rootState.AddAnyStateTransition(theState);
                    stateChangesMade += 1;
                }

                // cache reference
                spellTransition = rootState.anyStateTransitions.FirstOrDefault(t => t.destinationState == theState);
            }
            else if (sourceState != null)
            {
                // check the transition
                if (sourceState.transitions.Count(t => t.destinationState == theState) == 0)
                {
                    // not found add it
                    sourceState.AddTransition(theState);
                    stateChangesMade += 1;
                }

                // cache reference
                spellTransition = sourceState.transitions.FirstOrDefault(t => t.destinationState == theState);
            }

            // check spell transition parameters
            if (spellTransition != null)
            {
                // check conditions
                if (spellConditions != null)
                {
                    // do the two arrays match
                    if (!spellTransition.conditions.SequenceEqual(spellConditions))
                    {
                        // overwrite the conditions
                        spellTransition.conditions = spellConditions;
                        stateChangesMade += 1;
                    }
                }

                // set exit time on the transition
                spellTransition.hasExitTime = spellHasExitTime;
            }

            // check destination transition
            AnimatorStateTransition exitTransition = null;
            if (destinationState != null)
            {
                // check the transition
                if (theState.transitions.Count(t => t.destinationState == destinationState) == 0)
                {
                    // not found add it
                    exitTransition = theState.AddTransition(destinationState);
                    stateChangesMade += 1;
                }
            }
            else if (destinationStateMachine != null)
            {
                // check the transition
                if (theState.transitions.Count(t => t.destinationStateMachine == destinationStateMachine) == 0)
                {
                    // not found add it
                    exitTransition = theState.AddTransition(destinationStateMachine);
                    stateChangesMade += 1;
                }
            }

            // check the exit transitions conditions
            if (exitTransition != null)
            {
                exitTransition.hasExitTime = exitHasExitTime;
            }

            // report changes applied
        }

        /// <summary>
        /// Find or Create null state machine.
        /// </summary>
        /// <param name="parent">Container for the null state machine.</param>
        /// <returns>Null state machine found or created.</returns>
        private static AnimatorStateMachine ValidateNullStateMachine(AnimatorStateMachine parent)
        {
            var nullStateMachine = FindStateMachine("Null", parent);
            if (nullStateMachine != null) return nullStateMachine;
            nullStateMachine = parent.AddStateMachine("Null");
            parent.entryTransitions = new AnimatorTransition[0];
            parent.AddEntryTransition(nullStateMachine);
            var nullSub = nullStateMachine.AddState("Null");
            nullStateMachine.AddEntryTransition(nullSub);

            return nullStateMachine;
        }

        /// <summary>
        /// Find a state child on a state machine.
        /// </summary>
        /// <param name="stateName">Name of the state child to find.</param>
        /// <param name="parent">Parent state machine to search for the state child.</param>
        /// <returns>State or null if not found.</returns>
        private static AnimatorState FindState(string stateName, AnimatorStateMachine parent)
        {
            return parent.states.FirstOrDefault(s => s.state.name == stateName).state;
        }

        /// <summary>
        /// Find a state machine child on a state machine.
        /// </summary>
        /// <param name="stateMachineName">Name of the state machine child to find.</param>
        /// <param name="parent">Parent state machine to search for the state machine child.</param>
        /// <returns>State machine or null if not found.</returns>
        private static AnimatorStateMachine FindStateMachine(string stateMachineName, AnimatorStateMachine parent)
        {
            return parent.stateMachines.FirstOrDefault(s => s.stateMachine.name == stateMachineName).stateMachine;
        }

        /// <summary>
        /// Tidy the states on a state machine.
        /// </summary>
        /// <param name="parent">Parent state machine to tidy.</param>
        private static void TidyStateMachineChildren(AnimatorStateMachine parent)
        {
            // define entry and exit positions
            var leftPosition = new Vector3(-250, 0, 0);
            var rightPosition = new Vector3(1000, 0, 0);

            parent.entryPosition = leftPosition;
            leftPosition.y += 100;
            parent.anyStatePosition = leftPosition;
            parent.exitPosition = rightPosition;
            rightPosition.y += 100;
            parent.parentStateMachinePosition = rightPosition;
        }

        /// <summary>
        /// Add the spell book attack script if not found and sync the options.
        /// </summary>
        /// <param name="theState">State to check.</param>
        /// <param name="spell">Options to sync.</param>
        private static void AddSpellBookAttack(AnimatorState theState, EadonSpellBookSpell spell)
        {
            // check behavior exists
            if (theState.behaviours.Count(b => b.GetType() == typeof(EadonSpellAttack)) == 0)
            {
                theState.AddStateMachineBehaviour(typeof(EadonSpellAttack));
            }

            // sync the options
            var sb = (EadonSpellAttack) theState.behaviours.FirstOrDefault(b =>
                b.GetType() == typeof(EadonSpellAttack));
            if (sb != null)
            {
                sb.leftHandParticleEffect = spell.leftHandParticleEffect;
                sb.rightHandParticleEffect = spell.rightHandParticleEffect;
                sb.chargeState = spell.chargeState;
                sb.spawnOverTime = spell.GetRuntimeSpawners();
            }
        }
    }

    [Serializable]
    public class EadonSpellbookDefaults
    {
        public string defaultSpawnClip = "Assets/Cogs & Goggles/Eadon RPG for Invector/Animations/Default Spawn.anim";
        public string defaultSpellCast = "Assets/Cogs & Goggles/Eadon RPG for Invector/Animations/Default Cast.anim";
        public string defaultChargeInit = "Assets/Cogs & Goggles/Eadon RPG for Invector/Animations/Default Charge.anim";
        public string defaultChargeHold = "Assets/Cogs & Goggles/Eadon RPG for Invector/Animations/Default Charge.anim";

        public string defaultChargeRelease =
            "Assets/Cogs & Goggles/Eadon RPG for Invector/Animations/Default Release.anim";

        public string defaultHandParticle =
            "Assets/Cogs & Goggles/Eadon RPG for Invector/Prefabs/Particles/FireHands.prefab";

        public string defaultItemList =
            "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Invector/Eadon RPG Demo vItemList.asset";

        public string defaultRpgDefaults =
            "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Eadon RPG Defaults.asset";

        public string defaultCharacterConfig =
            "Assets/Cogs & Goggles/Eadon RPG for Invector/Data/Rpg/Eadon RPG Character Configuration.asset";
    }

    [Serializable]
    public class EadonSpellListItem
    {
        public string spellName;
        public int magicId;
        public int manaCost;
        public Texture2D icon;
        public string damageType;

        public override string ToString()
        {
            return $"{spellName}\nMagic ID:{magicId}\nMana Cost:{manaCost}\nDamage Type:{damageType}";
        }
    }

    /// <summary>
    /// List entry with settings regarding the animators to update.
    /// </summary>
    [Serializable]
    public class SpellbookAnimator
    {
#if UNITY_EDITOR
        /// <summary>Animation controller to update. </summary>       
        public AnimatorController controller;
#endif

        /// <summary>Apply the spell book animations to the controller.</summary>
        public bool dontSetAnimationClips;

        /// <summary>Index of the layer to add the magic to aka full body.</summary>
        public int magicLayerFullBodyIndex;

        /// <summary>Index of the layer to add the magic to aka full body.</summary>
        public int magicLayerUpperBodyIndex;

        /// <summary>List of the current animator layer names.</summary>
        public List<string> allLayerNames;

        public bool dedicatedFullBodyLayer;
    }
}

#endif
