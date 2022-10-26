#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using System.Linq;
using Eadon.EadonRPG.Scripts.Editor;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.ClothingSystem;
using Eadon.Rpg.Invector.Utils;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.Editor.Inspectors
{
    [CustomEditor(typeof(SyntyCharacterAppearance))]
    public class SyntyCharacterAppearanceInspector : EadonBaseEditor
    {
        private ClothingManager _clothingManager;
        private SyntyCharacterAppearance _appearance;
        private string[] _itemNamesList;
        private string[] _headList = new string[0];
        private string[] _eyebrowList = new string[0];
        private string[] _hairList = new string[0];
        private string[] _facialHairList = new string[0];
        private string[] _earsList = new string[0];
        private string[] _helmetList = new string[0];
        private string[] _cowlList = new string[0];
        private string[] _faceGuardList = new string[0];
        private string[] _hatList = new string[0];
        private string[] _torsoList = new string[0];
        private string[] _waistList = new string[0];
        private string[] _sleeveRightList = new string[0];
        private string[] _sleeveLeftList = new string[0];
        private string[] _wristRightList = new string[0];
        private string[] _wristLeftList = new string[0];
        private string[] _gloveRightList = new string[0];
        private string[] _gloveLeftList = new string[0];
        private string[] _bootRightList = new string[0];
        private string[] _bootLeftList = new string[0];

        private string[] _frontHeadAccessoryList = new string[0];

        // private string[] _sideHeadAccessoryList = new string[0];
        // private string[] _backHeadAccessoryList = new string[0];
        private string[] _capeList = new string[0];
        private string[] _beltAccessoryList = new string[0];
        private string[] _kneeRightList = new string[0];
        private string[] _kneeLeftList = new string[0];
        private string[] _elbowRightList = new string[0];
        private string[] _elbowLeftList = new string[0];
        private string[] _shoulderRightList = new string[0];
        private string[] _shoulderLeftList = new string[0];

        private bool _initialised;
        private bool _configureCharacter;
        private bool _changesMade;
        private Material _overrideMaterial;

        private void OnEnable()
        {
            editorTitle = "Eadon Synty Character Appearance";
            splashTexture = (Texture2D) Resources.Load("Textures/eadon_rpg_synty_character_appearance", typeof(Texture2D));
            showExpandButton = false;
            _appearance = (SyntyCharacterAppearance) target;
            var character = _appearance.GetComponent<EadonRpgCharacterBase>();
            if (character != null)
            {
                _clothingManager = character.clothingManager;
            }
            if (_clothingManager != null)
            {
                _clothingManager.Init();
                _itemNamesList = _clothingManager.GetAllItemNames();
            }
        }

        protected override void OnBaseInspectorGUI()
        {
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.LabelField("Cannot edit during play mode");
            }
            else
            {
                if (_appearance.headList.Count == 0)
                {
                    _appearance.InitialSetup();
                }

                if (!_initialised)
                {
                    InitialiseData();
                }
                
                _appearance.ApplyCurrentOptions();

                var material =
                    (Material)EditorGUILayout.ObjectField("Override Material", _overrideMaterial, typeof(Material), false);

                if (material != null && _overrideMaterial == null)
                {
                    _overrideMaterial = material;
                    _appearance.overrideMaterial = material;
                    _appearance.ReplaceMaterial();
                }
                else if (material == null && _overrideMaterial != null)
                {
                    _overrideMaterial = null;
                    _appearance.overrideMaterial = null;
                    _appearance.ReplaceMaterial();
                }
                else if (material != null && _overrideMaterial != null && material != _overrideMaterial)
                {
                    _overrideMaterial = material;
                    _appearance.overrideMaterial = material;
                    _appearance.ReplaceMaterial();
                }
                
                _configureCharacter = EditorGUILayout.Foldout(_configureCharacter, "Configure Character");
                if (_configureCharacter)
                {
                    var gender = (SyntyGender) EditorGUILayout.EnumPopup("Gender", _appearance.currentGender);
                    if (gender != _appearance.currentGender)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.Gender, (int) gender);
                        _appearance.InitialSetup();
                        InitialiseData();
                        _changesMade = true;
                    }

                    var face = EditorGUILayout.Popup("Head", _appearance.currentFace, _headList);
                    if (face != _appearance.currentFace)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.Head, face);
                        _changesMade = true;
                    }

                    var ears = EditorGUILayout.Popup("Ears", _appearance.currentEars, _earsList);
                    if (ears != _appearance.currentEars)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.Ears, ears);
                        _changesMade = true;
                    }

                    var hair = EditorGUILayout.Popup("Hair", _appearance.currentHair, _hairList);
                    if (hair != _appearance.currentHair)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.Hair, hair);
                        _changesMade = true;
                    }

                    var facialHair =
                        EditorGUILayout.Popup("Facial Hair", _appearance.currentFacialHair, _facialHairList);
                    if (facialHair != _appearance.currentFacialHair)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.FacialHair, facialHair);
                        _changesMade = true;
                    }

                    var eyebrows = EditorGUILayout.Popup("Eyebrows", _appearance.currentEyebrows, _eyebrowList);
                    if (eyebrows != _appearance.currentEyebrows)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.Eyebrows, eyebrows);
                        _changesMade = true;
                    }

                    var helmet = EditorGUILayout.Popup("Helmet", _appearance.currentHelmet, _helmetList);
                    if (helmet != _appearance.currentHelmet)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.Helmet, helmet);
                        _changesMade = true;
                    }

                    var cowl = EditorGUILayout.Popup("Cowl", _appearance.currentCowl, _cowlList);
                    if (cowl != _appearance.currentCowl)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.Cowl, cowl);
                        _changesMade = true;
                    }

                    var hat = EditorGUILayout.Popup("Hat", _appearance.currentHat, _hatList);
                    if (hat != _appearance.currentHat)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.Hat, hat);
                        _changesMade = true;
                    }

                    var faceGuard = EditorGUILayout.Popup("Face Guard", _appearance.currentFaceGuard, _faceGuardList);
                    if (faceGuard != _appearance.currentFaceGuard)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.FaceGuard, faceGuard);
                        _changesMade = true;
                    }

                    var frontHeadAccessory = EditorGUILayout.Popup("Helmet Acc", _appearance.currentFrontHeadAccessory,
                        _frontHeadAccessoryList);
                    if (frontHeadAccessory != _appearance.currentFrontHeadAccessory)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.FrontHeadAccessory, frontHeadAccessory);
                        _changesMade = true;
                    }

                    var bothShoulder =
                        EditorGUILayout.Popup("Both Shoulders", _appearance.currentBothShoulder, _shoulderLeftList);
                    if (bothShoulder != 0)
                    {
                        // _appearance.leftShoulder = _appearance.bothShoulder;
                        // _appearance.rightShoulder = _appearance.bothShoulder;
                        _appearance.ApplyOptions(SyntyModularOptions.ShoulderLeft, bothShoulder);
                        _appearance.ApplyOptions(SyntyModularOptions.ShoulderRight, bothShoulder);
                        _changesMade = true;
                    }

                    var leftShoulder =
                        EditorGUILayout.Popup("Left Shoulder", _appearance.currentLeftShoulder, _shoulderLeftList);
                    if (leftShoulder != _appearance.currentLeftShoulder)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.ShoulderLeft, leftShoulder);
                        _changesMade = true;
                    }

                    var rightShoulder =
                        EditorGUILayout.Popup("Right Shoulder", _appearance.currentRightShoulder, _shoulderRightList);
                    if (rightShoulder != _appearance.currentRightShoulder)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.ShoulderRight, rightShoulder);
                        _changesMade = true;
                    }

                    var chest = EditorGUILayout.Popup("Chest", _appearance.currentChest, _torsoList);
                    if (chest != _appearance.currentChest)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.Chest, chest);
                        _changesMade = true;
                    }

                    var cape = EditorGUILayout.Popup("Cape", _appearance.currentCape, _capeList);
                    if (cape != _appearance.currentCape)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.Cape, cape);
                        _changesMade = true;
                    }

                    var waist = EditorGUILayout.Popup("Waist", _appearance.currentWaist, _waistList);
                    if (waist != _appearance.currentWaist)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.Waist, waist);
                        _changesMade = true;
                    }

                    var beltAccessory =
                        EditorGUILayout.Popup("Belt Acc", _appearance.currentBeltAccessory, _beltAccessoryList);
                    if (beltAccessory != _appearance.currentBeltAccessory)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.BeltAccessory, beltAccessory);
                        _changesMade = true;
                    }

                    var bothSleeve =
                        EditorGUILayout.Popup("Both Sleeves", _appearance.currentBothSleeve, _sleeveLeftList);
                    if (bothSleeve != _appearance.currentBothSleeve)
                    {
                        // _appearance.leftSleeve = _appearance.bothSleeve;
                        // _appearance.rightSleeve = _appearance.bothSleeve;
                        _appearance.currentBothSleeve = bothSleeve;
                        _appearance.ApplyOptions(SyntyModularOptions.SleeveLeft, bothSleeve);
                        _appearance.ApplyOptions(SyntyModularOptions.SleeveRight, bothSleeve);
                        _changesMade = true;
                    }

                    var leftSleeve =
                        EditorGUILayout.Popup("Left Sleeve", _appearance.currentLeftSleeve, _sleeveLeftList);
                    if (leftSleeve != _appearance.currentLeftSleeve)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.SleeveLeft, leftSleeve);
                        _changesMade = true;
                    }

                    var rightSleeve =
                        EditorGUILayout.Popup("Right Sleeve", _appearance.currentRightSleeve, _sleeveRightList);
                    if (rightSleeve != _appearance.currentRightSleeve)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.SleeveRight, rightSleeve);
                        _changesMade = true;
                    }

                    var bothElbowpads =
                        EditorGUILayout.Popup("Both Elbow Acc", _appearance.currentBothElbowpads, _elbowLeftList);
                    if (bothElbowpads != _appearance.currentBothElbowpads)
                    {
                        // _appearance.leftElbowpad = _appearance.bothElbowpads;
                        // _appearance.rightElbowpad = _appearance.bothElbowpads;
                        _appearance.currentBothElbowpads = bothElbowpads;
                        _appearance.ApplyOptions(SyntyModularOptions.ElbowLeft, bothElbowpads);
                        _appearance.ApplyOptions(SyntyModularOptions.ElbowRight, bothElbowpads);
                        _changesMade = true;
                    }

                    var leftElbowpad =
                        EditorGUILayout.Popup("Left Elbow Acc", _appearance.currentLeftElbowpad, _elbowLeftList);
                    if (leftElbowpad != _appearance.currentLeftElbowpad)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.ElbowLeft, leftElbowpad);
                        _changesMade = true;
                    }

                    var rightElbowpad =
                        EditorGUILayout.Popup("Right Elbow Acc", _appearance.currentRightElbowpad, _elbowRightList);
                    if (rightElbowpad != _appearance.currentRightElbowpad)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.ElbowRight, rightElbowpad);
                        _changesMade = true;
                    }

                    var bothWrists =
                        EditorGUILayout.Popup("Both Wrists", _appearance.currentBothWrists, _wristLeftList);
                    if (bothWrists != _appearance.currentBothWrists)
                    {
                        // _appearance.leftWrist = _appearance.bothWrists;
                        // _appearance.rightWrist = _appearance.bothWrists;
                        _appearance.currentBothWrists = bothWrists;
                        _appearance.ApplyOptions(SyntyModularOptions.WristLeft, bothWrists);
                        _appearance.ApplyOptions(SyntyModularOptions.WristRight, bothWrists);
                        _changesMade = true;
                    }

                    var leftWrist = EditorGUILayout.Popup("Left Wrist", _appearance.currentLeftWrist, _wristLeftList);
                    if (leftWrist != _appearance.currentLeftWrist)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.WristLeft, leftWrist);
                        _changesMade = true;
                    }

                    var rightWrist =
                        EditorGUILayout.Popup("Right Wrist", _appearance.currentRightWrist, _wristRightList);
                    if (rightWrist != _appearance.currentRightWrist)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.WristRight, rightWrist);
                        _changesMade = true;
                    }

                    var bothGloves =
                        EditorGUILayout.Popup("Both GLoves", _appearance.currentBothGloves, _gloveLeftList);
                    if (bothGloves != _appearance.currentBothGloves)
                    {
                        // _appearance.leftGlove = _appearance.bothGloves;
                        // _appearance.rightGlove = _appearance.bothGloves;
                        _appearance.currentBothGloves = bothGloves;
                        _appearance.ApplyOptions(SyntyModularOptions.GloveLeft, bothGloves);
                        _appearance.ApplyOptions(SyntyModularOptions.GloveRight, bothGloves);
                        _changesMade = true;
                    }

                    var leftGlove = EditorGUILayout.Popup("Left GLove", _appearance.currentLeftGlove, _gloveLeftList);
                    if (leftGlove != _appearance.currentLeftGlove)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.GloveLeft, leftGlove);
                        _changesMade = true;
                    }

                    var rightGlove =
                        EditorGUILayout.Popup("Right Glove", _appearance.currentRightGlove, _gloveRightList);
                    if (rightGlove != _appearance.currentRightGlove)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.GloveRight, rightGlove);
                        _changesMade = true;
                    }

                    var bothKneepads =
                        EditorGUILayout.Popup("Both Knee Acc", _appearance.currentBothKneepads, _kneeLeftList);
                    if (bothKneepads != _appearance.currentBothKneepads)
                    {
                        // _appearance.leftKneepad = _appearance.bothKneepads;
                        // _appearance.rightKneepad = _appearance.bothKneepads;
                        _appearance.currentBothKneepads = bothKneepads;
                        _appearance.ApplyOptions(SyntyModularOptions.KneeLeft, bothKneepads);
                        _appearance.ApplyOptions(SyntyModularOptions.KneeRight, bothKneepads);
                        _changesMade = true;
                    }

                    var leftKneepad =
                        EditorGUILayout.Popup("Left Knee Acc", _appearance.currentLeftKneepad, _kneeLeftList);
                    if (leftKneepad != _appearance.currentLeftKneepad)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.KneeLeft, leftKneepad);
                        _changesMade = true;
                    }

                    var rightKneepad =
                        EditorGUILayout.Popup("Right Knee Acc", _appearance.currentRightKneepad, _kneeRightList);
                    if (rightKneepad != _appearance.currentRightKneepad)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.KneeRight, rightKneepad);
                        _changesMade = true;
                    }

                    var bothBoots = EditorGUILayout.Popup("Both Boots", _appearance.currentBothBoots, _bootLeftList);
                    if (bothBoots != _appearance.currentBothBoots)
                    {
                        // _appearance.leftBoot = _appearance.bothBoots;
                        // _appearance.rightBoot = _appearance.bothBoots;
                        _appearance.currentBothBoots = bothBoots;
                        _appearance.ApplyOptions(SyntyModularOptions.BootLeft, bothBoots);
                        _appearance.ApplyOptions(SyntyModularOptions.BootRight, bothBoots);
                        _changesMade = true;
                    }

                    var leftBoot = EditorGUILayout.Popup("Left Boot", _appearance.currentLeftBoot, _bootLeftList);
                    if (leftBoot != _appearance.currentLeftBoot)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.BootLeft, leftBoot);
                        _changesMade = true;
                    }

                    var rightBoot = EditorGUILayout.Popup("Right Boot", _appearance.currentRightBoot, _bootRightList);
                    if (rightBoot != _appearance.currentRightBoot)
                    {
                        _appearance.ApplyOptions(SyntyModularOptions.BootRight, rightBoot);
                        _changesMade = true;
                    }

//                    _appearance.ApplySavedOptions();
                }

                EditorGUILayout.Space();

                if (_changesMade)
                {
                    EditorGUILayout.LabelField("You have unsaved changes", redStyle);
                }
                
                if (GUILayout.Button("Update Starting Equipment"))
                {
                    UpdateStartingEquipment();
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Create Clothing Items"))
                {
                    UpdateClothingManager();
                }
            }


            // var chest = serializedObject.FindProperty("chest");
            //
            // EditorGUILayout.PropertyField(chest, new GUIContent("Chest"), false);
            //
            // serializedObject.ApplyModifiedProperties();
        }

        private void InitialiseData()
        {
            _headList = _appearance.headList.Select(gameObject => gameObject.name).ToArray();
            var ebTmp = new List<string> {"None"};
            ebTmp.AddRange(_appearance.eyebrowList.Select(gameObject => gameObject.name));
            _eyebrowList = ebTmp.ToArray();
            var hTmp = new List<string> {"None"};
            hTmp.AddRange(_appearance.hairList.Select(gameObject => gameObject.name));
            _hairList = hTmp.ToArray();
            var faTmp = new List<string> {"None"};
            faTmp.AddRange(_appearance.facialHairList.Select(gameObject => gameObject.name));
            _facialHairList = faTmp.ToArray();
            var earsTmp = new List<string> {"None"};
            earsTmp.AddRange(_appearance.earsList.Select(gameObject => gameObject.name));
            _earsList = earsTmp.ToArray();
            var helmetTmp = new List<string> {"None"};
            helmetTmp.AddRange(_appearance.helmetList.Select(gameObject => gameObject.name));
            _helmetList = helmetTmp.ToArray();
            var cowlTmp = new List<string> {"None"};
            cowlTmp.AddRange(_appearance.cowlList.Select(gameObject => gameObject.name));
            _cowlList = cowlTmp.ToArray();
            var fgTmp = new List<string> {"None"};
            fgTmp.AddRange(_appearance.faceGuardList.Select(gameObject => gameObject.name));
            _faceGuardList = fgTmp.ToArray();
            var hatTmp = new List<string> {"None"};
            hatTmp.AddRange(_appearance.hatList.Select(gameObject => gameObject.name));
            _hatList = hatTmp.ToArray();
            _torsoList = _appearance.torsoList.Select(gameObject => gameObject.name).ToArray();
            _waistList = _appearance.waistList.Select(gameObject => gameObject.name).ToArray();
            _sleeveRightList = _appearance.sleeveRightList.Select(gameObject => gameObject.name).ToArray();
            _sleeveLeftList = _appearance.sleeveLeftList.Select(gameObject => gameObject.name).ToArray();
            _wristRightList = _appearance.wristRightList.Select(gameObject => gameObject.name).ToArray();
            _wristLeftList = _appearance.wristLeftList.Select(gameObject => gameObject.name).ToArray();
            _gloveRightList = _appearance.gloveRightList.Select(gameObject => gameObject.name).ToArray();
            _gloveLeftList = _appearance.gloveLeftList.Select(gameObject => gameObject.name).ToArray();
            _bootRightList = _appearance.bootRightList.Select(gameObject => gameObject.name).ToArray();
            _bootLeftList = _appearance.bootLeftList.Select(gameObject => gameObject.name).ToArray();
            var fhaTmp = new List<string> {"None"};
            fhaTmp.AddRange(_appearance.frontHeadAccessoryList.Select(gameObject => gameObject.name));
            _frontHeadAccessoryList = fhaTmp.ToArray();
            // _sideHeadAccessoryList = _appearance.sideHeadAccessoryList.Select(gameObject => gameObject.name).ToArray();
            // _backHeadAccessoryList = _appearance.backHeadAccessoryList.Select(gameObject => gameObject.name).ToArray();
            var cTmp = new List<string> {"None"};
            cTmp.AddRange(_appearance.capeList.Select(gameObject => gameObject.name));
            _capeList = cTmp.ToArray();
            var baTmp = new List<string> {"None"};
            baTmp.AddRange(_appearance.beltAccessoryList.Select(gameObject => gameObject.name));
            _beltAccessoryList = baTmp.ToArray();
            var krTmp = new List<string> {"None"};
            krTmp.AddRange(_appearance.kneeRightList.Select(gameObject => gameObject.name));
            _kneeRightList = krTmp.ToArray();
            var klTmp = new List<string> {"None"};
            klTmp.AddRange(_appearance.kneeLeftList.Select(gameObject => gameObject.name));
            _kneeLeftList = klTmp.ToArray();
            var erTmp = new List<string> {"None"};
            erTmp.AddRange(_appearance.elbowRightList.Select(gameObject => gameObject.name));
            _elbowRightList = erTmp.ToArray();
            var elTmp = new List<string> {"None"};
            elTmp.AddRange(_appearance.elbowLeftList.Select(gameObject => gameObject.name));
            _elbowLeftList = elTmp.ToArray();
            var rsTmp = new List<string> {"None"};
            rsTmp.AddRange(_appearance.shoulderRightList.Select(gameObject => gameObject.name));
            _shoulderRightList = rsTmp.ToArray();
            var lsTmp = new List<string> {"None"};
            lsTmp.AddRange(_appearance.shoulderLeftList.Select(gameObject => gameObject.name));
            _shoulderLeftList = lsTmp.ToArray();

            _initialised = true;
        }

        private void UpdateStartingEquipment()
        {
            serializedObject.Update();
            
            serializedObject.FindProperty("gender").enumValueIndex = (int) _appearance.currentGender;
            serializedObject.FindProperty("face").intValue = _appearance.currentFace;
            serializedObject.FindProperty("ears").intValue = _appearance.currentEars;
            serializedObject.FindProperty("hair").intValue = _appearance.currentHair;
            serializedObject.FindProperty("facialHair").intValue = _appearance.currentFacialHair;
            serializedObject.FindProperty("eyebrows").intValue = _appearance.currentEyebrows;
            serializedObject.FindProperty("helmet").intValue = _appearance.currentHelmet;
            serializedObject.FindProperty("cowl").intValue = _appearance.currentCowl;
            serializedObject.FindProperty("hat").intValue = _appearance.currentHat;
            serializedObject.FindProperty("faceGuard").intValue = _appearance.currentFaceGuard;
            serializedObject.FindProperty("frontHeadAccessory").intValue = _appearance.currentFrontHeadAccessory;
            serializedObject.FindProperty("sideHeadAccessory").intValue = _appearance.currentSideHeadAccessory;
            serializedObject.FindProperty("backHeadAccessory").intValue = _appearance.currentBackHeadAccessory;
            serializedObject.FindProperty("bothShoulder").intValue = _appearance.currentBothShoulder;
            serializedObject.FindProperty("leftShoulder").intValue = _appearance.currentLeftShoulder;
            serializedObject.FindProperty("rightShoulder").intValue = _appearance.currentRightShoulder;
            serializedObject.FindProperty("chest").intValue = _appearance.currentChest;
            serializedObject.FindProperty("cape").intValue = _appearance.currentCape;
            serializedObject.FindProperty("waist").intValue = _appearance.currentWaist;
            serializedObject.FindProperty("beltAccessory").intValue = _appearance.currentBeltAccessory;
            serializedObject.FindProperty("bothSleeve").intValue = _appearance.currentBothSleeve;
            serializedObject.FindProperty("leftSleeve").intValue = _appearance.currentLeftSleeve;
            serializedObject.FindProperty("rightSleeve").intValue = _appearance.currentRightSleeve;
            serializedObject.FindProperty("bothElbowpads").intValue = _appearance.currentBothElbowpads;
            serializedObject.FindProperty("leftElbowpad").intValue = _appearance.currentLeftElbowpad;
            serializedObject.FindProperty("rightElbowpad").intValue = _appearance.currentRightElbowpad;
            serializedObject.FindProperty("bothWrists").intValue = _appearance.currentBothWrists;
            serializedObject.FindProperty("leftWrist").intValue = _appearance.currentLeftWrist;
            serializedObject.FindProperty("rightWrist").intValue = _appearance.currentRightWrist;
            serializedObject.FindProperty("bothGloves").intValue = _appearance.currentBothGloves;
            serializedObject.FindProperty("leftGlove").intValue = _appearance.currentLeftGlove;
            serializedObject.FindProperty("rightGlove").intValue = _appearance.currentRightGlove;
            serializedObject.FindProperty("bothKneepads").intValue = _appearance.currentBothKneepads;
            serializedObject.FindProperty("leftKneepad").intValue = _appearance.currentLeftKneepad;
            serializedObject.FindProperty("rightKneepad").intValue = _appearance.currentRightKneepad;
            serializedObject.FindProperty("bothBoots").intValue = _appearance.currentBothBoots;
            serializedObject.FindProperty("leftBoot").intValue = _appearance.currentLeftBoot;
            serializedObject.FindProperty("rightBoot").intValue = _appearance.currentRightBoot;

            // apply modified properties
            serializedObject.ApplyModifiedProperties();
            
            _changesMade = false;
        }

        private void UpdateClothingManager()
        {
            var absolutePath = EditorUtility.OpenFolderPanel("Select Folder To Save Meshes and Prefabs", "", "");
            var localPath = FileUtility.AssetsRelativePath(absolutePath);

            _headList = _appearance.headList.Select(gameObject => gameObject.name).ToArray();
            var ebTmp = new List<string> {"None"};
            ebTmp.AddRange(_appearance.eyebrowList.Select(gameObject => gameObject.name));
            _eyebrowList = ebTmp.ToArray();
            var hTmp = new List<string> {"None"};
            hTmp.AddRange(_appearance.hairList.Select(gameObject => gameObject.name));
            _hairList = hTmp.ToArray();
            var faTmp = new List<string> {"None"};
            faTmp.AddRange(_appearance.facialHairList.Select(gameObject => gameObject.name));
            _facialHairList = faTmp.ToArray();
            var earsTmp = new List<string> {"None"};
            earsTmp.AddRange(_appearance.earsList.Select(gameObject => gameObject.name));
            _earsList = earsTmp.ToArray();


            foreach (var gameObject in _appearance.helmetList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.HelmetClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.cowlList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.CowlClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.faceGuardList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.FaceGuardClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.hatList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.HatClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.torsoList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.ChestClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.waistList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.LegsClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.sleeveRightList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.UpperRightArmClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.sleeveLeftList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.UpperLeftArmClothing);
                    _clothingManager.allItems.Add(newClothing);
                }

                var clothingName = "B#" + gameObject.name;
                if (!_itemNamesList.Contains(clothingName))
                {
                    var newClothing = CreateClothing(clothingName, localPath, EadonClothingItem.ClothingHolderNames.BothUpperArmClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.wristRightList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.LowerRightArmClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.wristLeftList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.LowerLeftArmClothing);
                    _clothingManager.allItems.Add(newClothing);
                }

                var clothingName = "B#" + gameObject.name;
                if (!_itemNamesList.Contains(clothingName))
                {
                    var newClothing = CreateClothing(clothingName, localPath, EadonClothingItem.ClothingHolderNames.BothLowerArmClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.gloveRightList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.RightHandClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.gloveLeftList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing =
                        CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.LeftHandClothing);
                    _clothingManager.allItems.Add(newClothing);
                }

                var clothingName = "B#" + gameObject.name;
                if (!_itemNamesList.Contains(clothingName))
                {
                    var newClothing = CreateClothing(clothingName, localPath, EadonClothingItem.ClothingHolderNames.BothHandsClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.bootRightList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.RightFootClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.bootLeftList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing =
                        CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.LeftFootClothing);
                    _clothingManager.allItems.Add(newClothing);
                }

                var clothingName = "B#" + gameObject.name;
                if (!_itemNamesList.Contains(clothingName))
                {
                    var newClothing = CreateClothing(clothingName, localPath, EadonClothingItem.ClothingHolderNames.BothFeetClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.frontHeadAccessoryList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.HelmetAccessoryClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.capeList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.BackClothing);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.beltAccessoryList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.BeltAttachment);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.kneeRightList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.RightKneeAttachment);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.kneeLeftList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.LeftKneeAttachment);
                    _clothingManager.allItems.Add(newClothing);
                }

                var clothingName = "B#" + gameObject.name;
                if (!_itemNamesList.Contains(clothingName))
                {
                    var newClothing = CreateClothing(clothingName, localPath, EadonClothingItem.ClothingHolderNames.BothKneeAttachment);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.elbowRightList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.RightElbowAttachment);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.elbowLeftList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.LeftElbowAttachment);
                    _clothingManager.allItems.Add(newClothing);
                }

                var clothingName = "B#" + gameObject.name;
                if (!_itemNamesList.Contains(clothingName))
                {
                    var newClothing = CreateClothing(clothingName, localPath, EadonClothingItem.ClothingHolderNames.BothElbowAttachment);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.shoulderRightList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.RightShoulderAttachment);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            foreach (var gameObject in _appearance.shoulderLeftList)
            {
                if (!_itemNamesList.Contains(gameObject.name))
                {
                    var newClothing = CreateClothing(gameObject.name, localPath, EadonClothingItem.ClothingHolderNames.LeftShoulderAttachment);
                    _clothingManager.allItems.Add(newClothing);
                }

                var clothingName = "B#" + gameObject.name;
                if (!_itemNamesList.Contains(clothingName))
                {
                    var newClothing = CreateClothing(clothingName, localPath, EadonClothingItem.ClothingHolderNames.BothShoulderAttachment);
                    _clothingManager.allItems.Add(newClothing);
                }
            }

            EditorUtility.SetDirty(_clothingManager);
            AssetDatabase.SaveAssets();
        }

        private EadonClothingItem CreateClothing(string instanceName, string path, EadonClothingItem.ClothingHolderNames attachmentName)
        {
            var instance = CreateInstance<EadonClothingItem>();
            instance.name = instanceName;
            instance.clothingType = ClothingItemType.ShowAndHide;
            instance.clothingName = instanceName;
            instance.clothingHolderName = attachmentName;
            var assetPath = path + "/" + instanceName + ".asset";
            AssetDatabase.CreateAsset(instance, assetPath);
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
            return instance;
        }
    }
}
#endif
