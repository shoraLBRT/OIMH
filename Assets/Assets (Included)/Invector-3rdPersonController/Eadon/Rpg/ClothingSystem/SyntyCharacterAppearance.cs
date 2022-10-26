#if EADON_RPG_INVECTOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eadon.Rpg.Invector.ClothingSystem
{
    public enum SyntyModularOptions
    {
        Gender,
        Head,
        Eyebrows,
        FacialHair,
        Helmet,
        Cowl,
        FaceGuard,
        Hat,
        Chest,
        SleeveRight,
        SleeveLeft,
        WristRight,
        WristLeft,
        GloveRight,
        GloveLeft,
        Waist,
        BootRight,
        BootLeft,
        Ears,
        Hair,
        FrontHeadAccessory,

        // SideHeadAccessory,
        // BackHeadAccessory,
        Cape,
        ShoulderRight,
        ShoulderLeft,
        ElbowRight,
        ElbowLeft,
        BeltAccessory,
        KneeRight,
        KneeLeft,
    }

    public enum SyntyGender
    {
        Male,
        Female
    }

    public class SyntyCharacterAppearance : EadonCharacterAppearance
    {
        public GameObject modularGameObject;
        public Material originalMaterial;
        public Material overrideMaterial;
        public readonly List<GameObject> headList = new List<GameObject>();
        public readonly List<GameObject> eyebrowList = new List<GameObject>();
        public readonly List<GameObject> hairList = new List<GameObject>();
        public readonly List<GameObject> facialHairList = new List<GameObject>();
        public readonly List<GameObject> earsList = new List<GameObject>();
        public readonly List<GameObject> helmetList = new List<GameObject>();
        public readonly List<GameObject> cowlList = new List<GameObject>();
        public readonly List<GameObject> faceGuardList = new List<GameObject>();
        public readonly List<GameObject> hatList = new List<GameObject>();
        public readonly List<GameObject> torsoList = new List<GameObject>();
        public readonly List<GameObject> waistList = new List<GameObject>();
        public readonly List<GameObject> sleeveRightList = new List<GameObject>();
        public readonly List<GameObject> sleeveLeftList = new List<GameObject>();
        public readonly List<GameObject> wristRightList = new List<GameObject>();
        public readonly List<GameObject> wristLeftList = new List<GameObject>();
        public readonly List<GameObject> gloveRightList = new List<GameObject>();
        public readonly List<GameObject> gloveLeftList = new List<GameObject>();
        public readonly List<GameObject> bootRightList = new List<GameObject>();
        public readonly List<GameObject> bootLeftList = new List<GameObject>();

        public readonly List<GameObject> frontHeadAccessoryList = new List<GameObject>();

        // public readonly List<GameObject> sideHeadAccessoryList = new List<GameObject>();
        // public readonly List<GameObject> backHeadAccessoryList = new List<GameObject>();
        public readonly List<GameObject> capeList = new List<GameObject>();
        public readonly List<GameObject> beltAccessoryList = new List<GameObject>();
        public readonly List<GameObject> kneeRightList = new List<GameObject>();
        public readonly List<GameObject> kneeLeftList = new List<GameObject>();
        public readonly List<GameObject> elbowRightList = new List<GameObject>();
        public readonly List<GameObject> elbowLeftList = new List<GameObject>();
        public readonly List<GameObject> shoulderRightList = new List<GameObject>();
        public readonly List<GameObject> shoulderLeftList = new List<GameObject>();

        private GameObject _genderObj;
        private GameObject _headObj;
        private GameObject _eyebrowsObj;
        private GameObject _facialHairObj;
        private GameObject _hairObj;
        private GameObject _earObj;
        private GameObject _helmetObj;
        private GameObject _cowlObj;
        private GameObject _faceGuardObj;
        private GameObject _hatObj;
        private GameObject _chestObj;
        private GameObject _waistObj;
        private GameObject _shoulderLeftObj;
        private GameObject _shoulderRightObj;
        private GameObject _sleeveLeftObj;
        private GameObject _sleeveRightObj;
        private GameObject _elbowpadLeftObj;
        private GameObject _elbowpadRightObj;
        private GameObject _wristRightObj;
        private GameObject _wristLeftObj;
        private GameObject _gloveLeftObj;
        private GameObject _gloveRightObj;
        private GameObject _kneepadLeftObj;
        private GameObject _kneepadRightObj;
        private GameObject _bootLeftObj;
        private GameObject _bootRightObj;
        private GameObject _capeObj;
        private GameObject _beltAccessoryObj;

        private GameObject _frontHeadAccessoryObj;
        // private GameObject _backHeadAccessoryObj;
        // private GameObject _sideHeadAccessoryObj;

        private GameObject _maleParts;
        private GameObject _femaleParts;
        private GameObject _allParts;

        public SyntyGender gender;
        public int face;
        public int ears;
        public int hair;
        public int facialHair;
        public int eyebrows;
        public int helmet;
        public int cowl;
        public int hat;
        public int faceGuard;
        public int frontHeadAccessory;
        public int sideHeadAccessory;
        public int backHeadAccessory;
        public int bothShoulder;
        public int leftShoulder;
        public int rightShoulder;
        public int chest;
        public int cape;
        public int waist;
        public int beltAccessory;
        public int bothSleeve;
        public int leftSleeve;
        public int rightSleeve;
        public int bothElbowpads;
        public int leftElbowpad;
        public int rightElbowpad;
        public int bothWrists;
        public int leftWrist;
        public int rightWrist;
        public int bothGloves;
        public int leftGlove;
        public int rightGlove;
        public int bothKneepads;
        public int leftKneepad;
        public int rightKneepad;
        public int bothBoots;
        public int leftBoot;
        public int rightBoot;

        public SyntyGender currentGender;
        public int currentFace;
        public int currentEars;
        public int currentHair;
        public int currentFacialHair;
        public int currentEyebrows;
        public int currentHelmet;
        public int currentCowl;
        public int currentHat;
        public int currentFaceGuard;
        public int currentFrontHeadAccessory;
        public int currentSideHeadAccessory;
        public int currentBackHeadAccessory;
        public int currentBothShoulder;
        public int currentLeftShoulder;
        public int currentRightShoulder;
        public int currentChest;
        public int currentCape;
        public int currentWaist;
        public int currentBeltAccessory;
        public int currentBothSleeve;
        public int currentLeftSleeve;
        public int currentRightSleeve;
        public int currentBothElbowpads;
        public int currentLeftElbowpad;
        public int currentRightElbowpad;
        public int currentBothWrists;
        public int currentLeftWrist;
        public int currentRightWrist;
        public int currentBothGloves;
        public int currentLeftGlove;
        public int currentRightGlove;
        public int currentBothKneepads;
        public int currentLeftKneepad;
        public int currentRightKneepad;
        public int currentBothBoots;
        public int currentLeftBoot;
        public int currentRightBoot;

        private void OnEnable()
        {
            Debug.Log("Chest: " + chest);
        }

        protected override void Start()
        {
            InitialSetup();
            ApplySavedOptions();
        }

        public void InitialSetup()
        {
            ClearCurrentOptionsObjs();
            FindPartRoots();
            DisableAllParts();
            ChangeGender();
            FetchOptions();
            if (Application.isPlaying)
            {
                ApplySavedOptions();
            }
            else
            {
                ApplyCurrentOptions();
            }
        }


        private void DisableAllParts()
        {
            foreach (Transform child in _femaleParts.transform)
            {
                if (child.childCount <= 0)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    foreach (Transform child1 in child)
                    {
                        if (child1.childCount <= 0)
                        {
                            child1.gameObject.SetActive(false);
                        }
                        else
                        {
                            foreach (Transform child2 in child1)
                                if (child2.childCount <= 0)
                                {
                                    child2.gameObject.SetActive(false);
                                }
                                else
                                {
                                }
                        }
                    }
                }
            }

            foreach (Transform child in _maleParts.transform)
            {
                if (child.childCount <= 0)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    foreach (Transform child1 in child)
                    {
                        if (child1.childCount <= 0)
                        {
                            child1.gameObject.SetActive(false);
                        }
                        else
                        {
                            foreach (Transform child2 in child1)
                                if (child2.childCount <= 0)
                                {
                                    child2.gameObject.SetActive(false);
                                }
                                else
                                {
                                }
                        }
                    }
                }
            }

            foreach (Transform child in _allParts.transform)
            {
                if (child.childCount <= 0)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    foreach (Transform child1 in child)
                    {
                        if (child1.childCount <= 0)
                        {
                            child1.gameObject.SetActive(false);
                        }
                        else
                        {
                            foreach (Transform child2 in child1)
                                if (child2.childCount <= 0)
                                {
                                    child2.gameObject.SetActive(false);
                                }
                                else
                                {
                                }
                        }
                    }
                }
            }
        }

        //Method to find the base GameObjects for use in generating list options
        private void FindPartRoots()
        {
            _femaleParts = modularGameObject.transform.Find("Modular_Characters").Find("Female_Parts").gameObject;
            _maleParts = modularGameObject.transform.Find("Modular_Characters").Find("Male_Parts").gameObject;
            _allParts = modularGameObject.transform.Find("Modular_Characters").Find("All_Gender_Parts").gameObject;
        }

        //Method to clear all the OptionOBJs
        private void ClearCurrentOptionsObjs()
        {
            _headObj = null;
            _eyebrowsObj = null;
            _hairObj = null;
            _facialHairObj = null;
            _genderObj = null;
            _helmetObj = null;
            _cowlObj = null;
            _faceGuardObj = null;
            _hatObj = null;
            _chestObj = null;
            _waistObj = null;
            _shoulderLeftObj = null;
            _shoulderRightObj = null;
            _sleeveLeftObj = null;
            _sleeveRightObj = null;
            _elbowpadLeftObj = null;
            _elbowpadRightObj = null;
            _wristRightObj = null;
            _wristLeftObj = null;
            _gloveLeftObj = null;
            _gloveRightObj = null;
            _kneepadLeftObj = null;
            _kneepadRightObj = null;
            _bootLeftObj = null;
            _bootRightObj = null;
            _capeObj = null;
            _beltAccessoryObj = null;
            _frontHeadAccessoryObj = null;
            // _backHeadAccessoryObj = null;
            // _sideHeadAccessoryObj = null;
        }

        //Method to fetch the Options and populate the appropriate lists with them
        private void FetchOptions()
        {
            #region Clear Lists

            headList.Clear();
            eyebrowList.Clear();
            hairList.Clear();
            facialHairList.Clear();
            helmetList.Clear();
            cowlList.Clear();
            faceGuardList.Clear();
            hatList.Clear();
            torsoList.Clear();
            earsList.Clear();
            waistList.Clear();
            sleeveRightList.Clear();
            sleeveLeftList.Clear();
            wristRightList.Clear();
            wristLeftList.Clear();
            gloveLeftList.Clear();
            gloveRightList.Clear();
            bootLeftList.Clear();
            bootRightList.Clear();
            frontHeadAccessoryList.Clear();
            // sideHeadAccessoryList.Clear();
            // backHeadAccessoryList.Clear();
            capeList.Clear();
            beltAccessoryList.Clear();
            elbowRightList.Clear();
            elbowLeftList.Clear();
            kneeRightList.Clear();
            kneeLeftList.Clear();
            shoulderLeftList.Clear();
            shoulderRightList.Clear();

            #endregion

            foreach (Transform hairstyle in _allParts.transform.GetChild(1))
            {
                hairList.Add(hairstyle.gameObject);
            }

            foreach (Transform faceTransform in _genderObj.transform.GetChild(0).GetChild(0))
            {
                headList.Add(faceTransform.gameObject);
            }

            foreach (Transform eyebrow in _genderObj.transform.GetChild(1))
            {
                eyebrowList.Add(eyebrow.gameObject);
            }

            foreach (Transform facialHairTransform in _genderObj.transform.GetChild(2))
            {
                facialHairList.Add(facialHairTransform.gameObject);
            }

            foreach (Transform earsTransform in _allParts.transform.GetChild(12).GetChild(0))
            {
                earsList.Add(earsTransform.gameObject);
            }

            foreach (Transform helmetTransform in _genderObj.transform.GetChild(0).GetChild(1))
            {
                helmetList.Add(helmetTransform.gameObject);
            }

            foreach (Transform hatTransform in _allParts.transform.GetChild(0).GetChild(0))
            {
                hatList.Add(hatTransform.gameObject);
            }

            foreach (Transform faceGuardTransform in _allParts.transform.GetChild(0).GetChild(1))
            {
                faceGuardList.Add(faceGuardTransform.gameObject);
            }

            foreach (Transform cowlTransform in _allParts.transform.GetChild(0).GetChild(2))
            {
                cowlList.Add(cowlTransform.gameObject);
            }

            foreach (Transform torso in _genderObj.transform.GetChild(3))
            {
                torsoList.Add(torso.gameObject);
            }

            foreach (Transform waistTransform in _genderObj.transform.GetChild(10))
            {
                waistList.Add(waistTransform.gameObject);
            }

            foreach (Transform sleeve in _genderObj.transform.GetChild(4))
            {
                sleeveRightList.Add(sleeve.gameObject);
            }

            foreach (Transform sleeve in _genderObj.transform.GetChild(5))
            {
                sleeveLeftList.Add(sleeve.gameObject);
            }

            foreach (Transform wrist in _genderObj.transform.GetChild(6))
            {
                wristRightList.Add(wrist.gameObject);
            }

            foreach (Transform wrist in _genderObj.transform.GetChild(7))
            {
                wristLeftList.Add(wrist.gameObject);
            }

            foreach (Transform glove in _genderObj.transform.GetChild(8))
            {
                gloveRightList.Add(glove.gameObject);
            }

            foreach (Transform glove in _genderObj.transform.GetChild(9))
            {
                gloveLeftList.Add(glove.gameObject);
            }

            foreach (Transform boot in _genderObj.transform.GetChild(11))
            {
                bootRightList.Add(boot.gameObject);
            }

            foreach (Transform boot in _genderObj.transform.GetChild(12))
            {
                bootLeftList.Add(boot.gameObject);
            }

            foreach (Transform headAccessory in _allParts.transform.GetChild(2).GetChild(1))
            {
                frontHeadAccessoryList.Add(headAccessory.gameObject);
            }

            // try
            // {
            //     foreach (Transform headAccessory in _allParts.transform.GetChild(2).GetChild(2))
            //     {
            //         sideHeadAccessoryList.Add(headAccessory.gameObject);
            //     }
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e);
            // }
            //
            // try
            // {
            //     foreach (Transform headAccessory in _allParts.transform.GetChild(2).GetChild(3))
            //     {
            //         backHeadAccessoryList.Add(headAccessory.gameObject);
            //     }
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e);
            // }

            try
            {
                foreach (Transform capeTransform in _allParts.transform.GetChild(4))
                {
                    capeList.Add(capeTransform.gameObject);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            foreach (Transform shoulderPad in _allParts.transform.GetChild(5))
            {
                shoulderRightList.Add(shoulderPad.gameObject);
            }

            foreach (Transform shoulderPad in _allParts.transform.GetChild(6))
            {
                shoulderLeftList.Add(shoulderPad.gameObject);
            }

            foreach (Transform elbowPad in _allParts.transform.GetChild(7))
            {
                elbowRightList.Add(elbowPad.gameObject);
            }

            foreach (Transform elbowPad in _allParts.transform.GetChild(8))
            {
                elbowLeftList.Add(elbowPad.gameObject);
            }

            foreach (Transform beltAccessoryTransform in _allParts.transform.GetChild(9))
            {
                beltAccessoryList.Add(beltAccessoryTransform.gameObject);
            }

            foreach (Transform kneePad in _allParts.transform.GetChild(10))
            {
                kneeRightList.Add(kneePad.gameObject);
            }

            foreach (Transform kneePad in _allParts.transform.GetChild(11))
            {
                kneeLeftList.Add(kneePad.gameObject);
            }
        }

        //Method to apply option changes based on an Enum of Option Type and an ID to select from a list of options
        public void ApplyOptions(SyntyModularOptions type, int id)
        {
            while (true)
            {
                switch (type)
                {
                    case SyntyModularOptions.Gender:
                    {
                        _genderObj = gender == SyntyGender.Male ? _maleParts : _femaleParts;
                        currentGender = (SyntyGender) id;
                        FetchOptions();

                        ChangeFace();
                        ChangeEyebrows();
                        ChangeChest();
                        ChangeSleeveLeft();
                        ChangeSleeveRight();
                        ChangeWristLeft();
                        ChangeWristRight();
                        ChangeGloveLeft();
                        ChangeGloveRight();
                        ChangeWaist();
                        ChangeBootLeft();
                        ChangeBootRight();

                        if (gender == SyntyGender.Male)
                        {
                            ChangeFacialHair();
                        }
                        else
                        {
                            type = SyntyModularOptions.FacialHair;
                            id = 0;
                            continue;
                        }

                        break;
                    }

                    case SyntyModularOptions.Hair:
                    {
                        if (_hairObj != null)
                        {
                            _hairObj.SetActive(false);
                        }

                        currentHair = id;
                        if (id > 0)
                        {
                            _hairObj = hairList[id - 1];
                            _hairObj.SetActive(true);
                        }

                        break;
                    }

                    case SyntyModularOptions.Head:
                    {
                        if (_headObj != null)
                        {
                            _headObj.SetActive(false);
                        }

                        currentFace = id;
                        if (face > headList.Count - 1)
                        {
                            face = headList.Count - 1;
                            id = face;
                        }

                        _headObj = headList[id];
                        _headObj.SetActive(true);
                        break;
                    }

                    case SyntyModularOptions.Eyebrows:
                    {
                        if (_eyebrowsObj != null)
                        {
                            _eyebrowsObj.SetActive(false);
                        }

                        if (eyebrows > eyebrowList.Count - 1)
                        {
                            eyebrows = eyebrowList.Count - 1;
                        }

                        currentEyebrows = id;
                        if (id > 0)
                        {
                            _eyebrowsObj = eyebrowList[id - 1];
                            _eyebrowsObj.SetActive(true);
                        }

                        break;
                    }

                    case SyntyModularOptions.FacialHair:
                    {
                        if (_facialHairObj != null)
                        {
                            _facialHairObj.SetActive(false);
                        }

                        if (facialHairList.Count == 0)
                        {
                            facialHair = 0;
                        }

                        currentFacialHair = id;
                        if (id > 0)
                        {
                            _facialHairObj = facialHairList[id - 1];
                            _facialHairObj.SetActive(true);
                        }

                        break;
                    }

                    case SyntyModularOptions.Ears:
                    {
                        if (_earObj != null)
                        {
                            _earObj.SetActive(false);
                        }

                        currentEars = id;
                        if (id > 0)
                        {
                            _earObj = earsList[id - 1];
                            _earObj.SetActive(true);
                        }

                        break;
                    }

                    case SyntyModularOptions.Helmet:
                    {
                        if (_helmetObj != null)
                        {
                            _helmetObj.SetActive(false);
                        }

                        currentHelmet = id;
                        if (id > 0)
                        {
                            ApplyOptions(SyntyModularOptions.Head, 0);
                            ApplyOptions(SyntyModularOptions.Eyebrows, 0);
                            ApplyOptions(SyntyModularOptions.Hair, 0);
                            ApplyOptions(SyntyModularOptions.FacialHair, 0);

                            _helmetObj = helmetList[id - 1];
                            _helmetObj.SetActive(true);
                        }
                        else
                        {
                            CheckHeadwearStatus();
                        }

                        break;
                    }

                    case SyntyModularOptions.Cowl:
                    {
                        if (_cowlObj != null)
                        {
                            _cowlObj.SetActive(false);
                        }

                        currentCowl = id;
                        if (id != 0)
                        {
                            ApplyOptions(SyntyModularOptions.Hair, 0);

                            _cowlObj = cowlList[id - 1];
                            _cowlObj.SetActive(true);
                        }
                        else
                        {
                            CheckHeadwearStatus();
                        }

                        break;
                    }

                    case SyntyModularOptions.Hat:
                    {
                        if (_hatObj != null)
                        {
                            _hatObj.SetActive(false);
                        }

                        currentHat = id;
                        if (id > 0)
                        {
                            ApplyOptions(SyntyModularOptions.Hair, 0);

                            _hatObj = hatList[id - 1];
                            _hatObj.SetActive(true);
                        }
                        else
                        {
                            CheckHeadwearStatus();
                        }

                        break;
                    }

                    case SyntyModularOptions.FaceGuard:
                    {
                        if (_faceGuardObj != null)
                        {
                            _faceGuardObj.SetActive(false);
                        }

                        currentFaceGuard = id;
                        if (id > 0)
                        {
                            ApplyOptions(SyntyModularOptions.FacialHair, 0);

                            _faceGuardObj = faceGuardList[id - 1];
                            _faceGuardObj.SetActive(true);
                        }
                        else
                        {
                            CheckHeadwearStatus();
                        }

                        break;
                    }

                    case SyntyModularOptions.FrontHeadAccessory:
                    {
                        if (_frontHeadAccessoryObj != null)
                        {
                            _frontHeadAccessoryObj.SetActive(false);
                        }

                        currentFrontHeadAccessory = id;
                        if (id > 0)
                        {
                            _frontHeadAccessoryObj = frontHeadAccessoryList[id - 1];
                            _frontHeadAccessoryObj.SetActive(true);
                        }

                        break;
                    }

                    // case SyntyModularOptions.SideHeadAccessory:
                    // {
                    //     if (_sideHeadAccessoryObj != null)
                    //     {
                    //         _sideHeadAccessoryObj.SetActive(false);
                    //     }
                    //
                    //     if (sideHeadAccessoryList.Count > 0)
                    //     {
                    //         _sideHeadAccessoryObj = sideHeadAccessoryList[id];
                    //         _sideHeadAccessoryObj.SetActive(true);
                    //
                    //         // _currentSideHeadAttachmentIndex = id;
                    //     }
                    //     break;
                    // }
                    //
                    // case SyntyModularOptions.BackHeadAccessory:
                    // {
                    //     if (_backHeadAccessoryObj != null)
                    //     {
                    //         _backHeadAccessoryObj.SetActive(false);
                    //     }
                    //
                    //     if (backHeadAccessoryList.Count > 0)
                    //     {
                    //         _backHeadAccessoryObj = backHeadAccessoryList[id];
                    //         _backHeadAccessoryObj.SetActive(true);
                    //
                    //         // _currentBackHeadAttachmentIndex = id;
                    //     }
                    //     break;
                    // }

                    case SyntyModularOptions.Chest:
                    {
                        if (_chestObj != null)
                        {
                            _chestObj.SetActive(false);
                        }

                        currentChest = id;
                        _chestObj = torsoList[id];
                        _chestObj.SetActive(true);
                        break;
                    }

                    case SyntyModularOptions.Waist:
                    {
                        if (_waistObj != null)
                        {
                            _waistObj.SetActive(false);
                        }

                        currentWaist = id;
                        _waistObj = waistList[id];
                        _waistObj.SetActive(true);
                        break;
                    }

                    case SyntyModularOptions.ShoulderRight:
                    {
                        if (_shoulderRightObj != null)
                        {
                            _shoulderRightObj.SetActive(false);
                        }

                        currentRightShoulder = id;
                        if (id > 0)
                        {
                            _shoulderRightObj = shoulderRightList[id - 1];
                            _shoulderRightObj.SetActive(true);
                        }

                        break;
                    }

                    case SyntyModularOptions.ShoulderLeft:
                    {
                        if (_shoulderLeftObj != null)
                        {
                            _shoulderLeftObj.SetActive(false);
                        }

                        currentLeftShoulder = id;
                        if (id > 0)
                        {
                            _shoulderLeftObj = shoulderLeftList[id - 1];
                            _shoulderLeftObj.SetActive(true);
                        }

                        break;
                    }

                    case SyntyModularOptions.WristLeft:
                    {
                        if (_wristLeftObj != null)
                        {
                            _wristLeftObj.SetActive(false);
                        }

                        currentLeftWrist = id;
                        _wristLeftObj = wristLeftList[id];
                        _wristLeftObj.SetActive(true);
                        break;
                    }

                    case SyntyModularOptions.WristRight:
                    {
                        if (_wristRightObj != null)
                        {
                            _wristRightObj.SetActive(false);
                        }

                        currentRightWrist = id;
                        _wristRightObj = wristRightList[id];
                        _wristRightObj.SetActive(true);
                        break;
                    }

                    case SyntyModularOptions.SleeveRight:
                    {
                        if (_sleeveRightObj != null)
                        {
                            _sleeveRightObj.SetActive(false);
                        }

                        currentRightSleeve = id;
                        _sleeveRightObj = sleeveRightList[id];
                        _sleeveRightObj.SetActive(true);
                        break;
                    }

                    case SyntyModularOptions.SleeveLeft:
                    {
                        if (_sleeveLeftObj != null)
                        {
                            _sleeveLeftObj.SetActive(false);
                        }

                        currentLeftSleeve = id;
                        _sleeveLeftObj = sleeveLeftList[id];
                        _sleeveLeftObj.SetActive(true);
                        break;
                    }

                    case SyntyModularOptions.GloveRight:
                    {
                        if (_gloveRightObj != null)
                        {
                            _gloveRightObj.SetActive(false);
                        }

                        currentRightGlove = id;
                        _gloveRightObj = gloveRightList[id];
                        _gloveRightObj.SetActive(true);
                        break;
                    }

                    case SyntyModularOptions.GloveLeft:
                    {
                        if (_gloveLeftObj != null)
                        {
                            _gloveLeftObj.SetActive(false);
                        }

                        currentLeftGlove = id;
                        _gloveLeftObj = gloveLeftList[id];
                        _gloveLeftObj.SetActive(true);
                        break;
                    }

                    case SyntyModularOptions.ElbowLeft:
                    {
                        if (_elbowpadLeftObj != null)
                        {
                            _elbowpadLeftObj.SetActive(false);
                        }

                        currentLeftElbowpad = id;
                        if (id > 0)
                        {
                            _elbowpadLeftObj = elbowLeftList[id - 1];
                            _elbowpadLeftObj.SetActive(true);
                        }

                        break;
                    }

                    case SyntyModularOptions.ElbowRight:
                    {
                        if (_elbowpadRightObj != null)
                        {
                            _elbowpadRightObj.SetActive(false);
                        }

                        currentRightElbowpad = id;
                        if (id > 0)
                        {
                            _elbowpadRightObj = elbowRightList[id - 1];
                            _elbowpadRightObj.SetActive(true);
                        }

                        break;
                    }

                    case SyntyModularOptions.Cape:
                    {
                        if (_capeObj != null)
                        {
                            _capeObj.SetActive(false);
                        }

                        currentCape = id;
                        if (id > 0)
                        {
                            _capeObj = capeList[id - 1];
                            _capeObj.SetActive(true);
                        }

                        break;
                    }

                    case SyntyModularOptions.BeltAccessory:
                    {
                        if (_beltAccessoryObj != null)
                        {
                            _beltAccessoryObj.SetActive(false);
                        }

                        currentBeltAccessory = id;
                        if (id > 0)
                        {
                            _beltAccessoryObj = beltAccessoryList[id - 1];
                            _beltAccessoryObj.SetActive(true);
                        }

                        break;
                    }

                    case SyntyModularOptions.KneeLeft:
                    {
                        if (_kneepadLeftObj != null)
                        {
                            _kneepadLeftObj.SetActive(false);
                        }

                        currentLeftKneepad = id;
                        if (id > 0)
                        {
                            _kneepadLeftObj = kneeLeftList[id - 1];
                            _kneepadLeftObj.SetActive(true);
                        }

                        break;
                    }

                    case SyntyModularOptions.KneeRight:
                    {
                        if (_kneepadRightObj != null)
                        {
                            _kneepadRightObj.SetActive(false);
                        }

                        currentRightKneepad = id;
                        if (id > 0)
                        {
                            _kneepadRightObj = kneeRightList[id - 1];
                            _kneepadRightObj.SetActive(true);
                        }

                        break;
                    }

                    case SyntyModularOptions.BootLeft:
                    {
                        if (_bootLeftObj != null)
                        {
                            _bootLeftObj.SetActive(false);
                        }

                        currentLeftBoot = id;
                        _bootLeftObj = bootLeftList[id];
                        _bootLeftObj.SetActive(true);
                        break;
                    }

                    case SyntyModularOptions.BootRight:
                    {
                        if (_bootRightObj != null)
                        {
                            _bootRightObj.SetActive(false);
                        }

                        currentRightBoot = id;
                        _bootRightObj = bootRightList[id];
                        _bootRightObj.SetActive(true);
                        break;
                    }
                }

                break;
            }
        }

        private void ChangeGender()
        {
            ApplyOptions(SyntyModularOptions.Gender, (int) currentGender);
        }

        private void ChangeFace()
        {
            ApplyOptions(SyntyModularOptions.Head, currentFace);
        }

        private void ChangeEars()
        {
            ApplyOptions(SyntyModularOptions.Ears, currentEars);
        }

        private void ChangeHair()
        {
            ApplyOptions(SyntyModularOptions.Hair, currentHair);
        }

        private void ChangeFacialHair()
        {
            ApplyOptions(SyntyModularOptions.FacialHair, currentFacialHair);
        }

        private void ChangeEyebrows()
        {
            ApplyOptions(SyntyModularOptions.Eyebrows, currentEyebrows);
        }

        private void ChangeHelmet()
        {
            ApplyOptions(SyntyModularOptions.Helmet, currentHelmet);
        }

        private void ChangeCowl()
        {
            ApplyOptions(SyntyModularOptions.Cowl, currentCowl);
        }

        private void ChangeHat()
        {
            ApplyOptions(SyntyModularOptions.Hat, currentHat);
        }

        private void ChangeFaceGuard()
        {
            ApplyOptions(SyntyModularOptions.FaceGuard, currentFaceGuard);
        }

        private void ChangeFrontHeadAccessory()
        {
            ApplyOptions(SyntyModularOptions.FrontHeadAccessory, currentFrontHeadAccessory);
        }

        // private void ChangeSideHeadAccessory()
        // {
        //     ApplyOptions(SyntyModularOptions.SideHeadAccessory, sideHeadAccessory);
        // }
        //
        // private void ChangeBackHeadAccessory()
        // {
        //     ApplyOptions(SyntyModularOptions.BackHeadAccessory, backHeadAccessory);
        // }

        private void ChangeChest()
        {
            ApplyOptions(SyntyModularOptions.Chest, currentChest);
        }

        private void ChangeWaist()
        {
            ApplyOptions(SyntyModularOptions.Waist, currentWaist);
        }

        private void ChangeCape()
        {
            ApplyOptions(SyntyModularOptions.Cape, currentCape);
        }

        private void ChangeBeltAccessory()
        {
            ApplyOptions(SyntyModularOptions.BeltAccessory, currentBeltAccessory);
        }

        private void ChangeShoulderLeft()
        {
            ApplyOptions(SyntyModularOptions.ShoulderLeft, currentLeftShoulder);
        }

        private void ChangeShoulderRight()
        {
            ApplyOptions(SyntyModularOptions.ShoulderRight, currentRightShoulder);
        }

        private void ChangeShoulderBoth()
        {
            currentLeftShoulder = currentBothShoulder;
            currentRightShoulder = currentBothShoulder;
            ApplyOptions(SyntyModularOptions.ShoulderRight, currentRightShoulder);
            ApplyOptions(SyntyModularOptions.ShoulderLeft, currentLeftShoulder);
        }

        private void ChangeWristRight()
        {
            ApplyOptions(SyntyModularOptions.WristRight, currentRightWrist);
        }

        private void ChangeWristLeft()
        {
            ApplyOptions(SyntyModularOptions.WristLeft, currentLeftWrist);
        }

        private void ChangeWristBoth()
        {
            currentLeftWrist = currentBothWrists;
            currentRightWrist = currentBothWrists;
            ApplyOptions(SyntyModularOptions.WristLeft, currentLeftWrist);
            ApplyOptions(SyntyModularOptions.WristRight, currentRightWrist);
        }

        private void ChangeSleeveLeft()
        {
            ApplyOptions(SyntyModularOptions.SleeveLeft, currentLeftSleeve);
        }

        private void ChangeSleeveRight()
        {
            ApplyOptions(SyntyModularOptions.SleeveRight, currentRightSleeve);
        }

        private void ChangeSleeveBoth()
        {
            currentLeftSleeve = currentBothSleeve;
            currentRightSleeve = currentBothSleeve;
            ApplyOptions(SyntyModularOptions.SleeveRight, currentRightSleeve);
            ApplyOptions(SyntyModularOptions.SleeveLeft, currentLeftSleeve);
        }

        private void ChangeElbowpadLeft()
        {
            ApplyOptions(SyntyModularOptions.ElbowLeft, currentLeftElbowpad);
        }

        private void ChangeElbowpadRight()
        {
            ApplyOptions(SyntyModularOptions.ElbowRight, currentRightElbowpad);
        }

        private void ChangeElbowpadBoth()
        {
            currentLeftElbowpad = currentBothElbowpads;
            currentRightElbowpad = currentBothElbowpads;
            ApplyOptions(SyntyModularOptions.ElbowLeft, currentLeftElbowpad);
            ApplyOptions(SyntyModularOptions.ElbowRight, currentRightElbowpad);
        }

        private void ChangeGloveLeft()
        {
            ApplyOptions(SyntyModularOptions.GloveLeft, currentLeftGlove);
        }

        private void ChangeGloveRight()
        {
            ApplyOptions(SyntyModularOptions.GloveRight, currentRightGlove);
        }

        private void ChangeGloveBoth()
        {
            currentLeftGlove = currentBothGloves;
            currentRightGlove = currentBothGloves;
            ApplyOptions(SyntyModularOptions.GloveRight, currentRightGlove);
            ApplyOptions(SyntyModularOptions.GloveLeft, currentLeftGlove);
        }

        private void ChangeKneepadLeft()
        {
            ApplyOptions(SyntyModularOptions.KneeLeft, currentLeftKneepad);
        }

        private void ChangeKneepadRight()
        {
            ApplyOptions(SyntyModularOptions.KneeRight, currentRightKneepad);
        }

        private void ChangeKneepadBoth()
        {
            currentLeftKneepad = bothKneepads;
            currentRightKneepad = bothKneepads;
            ApplyOptions(SyntyModularOptions.KneeRight, currentRightKneepad);
            ApplyOptions(SyntyModularOptions.KneeLeft, currentLeftKneepad);
        }

        private void ChangeBootLeft()
        {
            ApplyOptions(SyntyModularOptions.BootLeft, currentLeftBoot);
        }

        private void ChangeBootRight()
        {
            ApplyOptions(SyntyModularOptions.BootRight, currentRightBoot);
        }

        private void ChangeBootBoth()
        {
            currentLeftBoot = currentBothBoots;
            currentRightBoot = currentBothBoots;
            ApplyOptions(SyntyModularOptions.BootLeft, currentLeftBoot);
            ApplyOptions(SyntyModularOptions.BootRight, currentRightBoot);
        }

        private void CheckHeadwearStatus()
        {
            if (helmet == 0)
            {
                ChangeFace();
                ChangeEyebrows();
            }

            if (helmet == 0 && cowl == 0 && hat == 0)
            {
                ChangeHair();
            }
        }

        private int GetIndexByName(SyntyModularOptions syntyModularOption, string itemName)
        {
            int retVal = -1;

            switch (syntyModularOption)
            {
                case SyntyModularOptions.Gender:
                    break;
                case SyntyModularOptions.Head:
                    retVal = FindInList(headList, itemName);
                    break;
                case SyntyModularOptions.Eyebrows:
                    retVal = FindInList(eyebrowList, itemName);
                    break;
                case SyntyModularOptions.FacialHair:
                    retVal = FindInList(facialHairList, itemName);
                    break;
                case SyntyModularOptions.Helmet:
                    retVal = FindInList(helmetList, itemName);
                    break;
                case SyntyModularOptions.Cowl:
                    retVal = FindInList(cowlList, itemName);
                    break;
                case SyntyModularOptions.FaceGuard:
                    retVal = FindInList(faceGuardList, itemName);
                    break;
                case SyntyModularOptions.Hat:
                    retVal = FindInList(hatList, itemName);
                    break;
                case SyntyModularOptions.Chest:
                    retVal = FindInList(torsoList, itemName);
                    break;
                case SyntyModularOptions.SleeveRight:
                    retVal = FindInList(sleeveRightList, itemName);
                    break;
                case SyntyModularOptions.SleeveLeft:
                    retVal = FindInList(sleeveLeftList, itemName);
                    break;
                case SyntyModularOptions.WristRight:
                    retVal = FindInList(wristRightList, itemName);
                    break;
                case SyntyModularOptions.WristLeft:
                    retVal = FindInList(wristLeftList, itemName);
                    break;
                case SyntyModularOptions.GloveRight:
                    retVal = FindInList(gloveRightList, itemName);
                    break;
                case SyntyModularOptions.GloveLeft:
                    retVal = FindInList(gloveLeftList, itemName);
                    break;
                case SyntyModularOptions.Waist:
                    retVal = FindInList(waistList, itemName);
                    break;
                case SyntyModularOptions.BootRight:
                    retVal = FindInList(bootRightList, itemName);
                    break;
                case SyntyModularOptions.BootLeft:
                    retVal = FindInList(bootLeftList, itemName);
                    break;
                case SyntyModularOptions.Ears:
                    retVal = FindInList(earsList, itemName);
                    break;
                case SyntyModularOptions.Hair:
                    retVal = FindInList(hairList, itemName);
                    break;
                case SyntyModularOptions.FrontHeadAccessory:
                    retVal = FindInList(frontHeadAccessoryList, itemName);
                    break;
                // case SyntyModularOptions.SideHeadAccessory:
                //     retVal = FindInList(sideHeadAccessoryList, itemName);
                //     break;
                // case SyntyModularOptions.BackHeadAccessory:
                //     retVal = FindInList(backHeadAccessoryList, itemName);
                //     break;
                case SyntyModularOptions.Cape:
                    retVal = FindInList(capeList, itemName);
                    break;
                case SyntyModularOptions.ShoulderRight:
                    retVal = FindInList(shoulderRightList, itemName);
                    break;
                case SyntyModularOptions.ShoulderLeft:
                    retVal = FindInList(shoulderLeftList, itemName);
                    break;
                case SyntyModularOptions.ElbowRight:
                    retVal = FindInList(elbowRightList, itemName);
                    break;
                case SyntyModularOptions.ElbowLeft:
                    retVal = FindInList(elbowLeftList, itemName);
                    break;
                case SyntyModularOptions.BeltAccessory:
                    retVal = FindInList(beltAccessoryList, itemName);
                    break;
                case SyntyModularOptions.KneeRight:
                    retVal = FindInList(kneeRightList, itemName);
                    break;
                case SyntyModularOptions.KneeLeft:
                    retVal = FindInList(kneeLeftList, itemName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(syntyModularOption), syntyModularOption, null);
            }

            return retVal;
        }

        private static int FindInList(IReadOnlyList<GameObject> list, string itemName)
        {
            var name = itemName;
            if (name.StartsWith("B#"))
            {
                name = name.Substring(2);
            }
            var retVal = -1;

            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].name == name)
                {
                    retVal = i;
                }
            }

            retVal++;

            return retVal;
        }

        public void ApplySavedOptions()
        {
            currentGender = gender;
            currentFace = face;
            currentEars = ears;
            currentHair = hair;
            currentFacialHair = facialHair;
            currentEyebrows = eyebrows;
            currentHelmet = helmet;
            currentCowl = cowl;
            currentHat = hat;
            currentFaceGuard = faceGuard;
            currentFrontHeadAccessory = frontHeadAccessory;
            currentSideHeadAccessory = sideHeadAccessory;
            currentBackHeadAccessory = backHeadAccessory;
            currentBothShoulder = bothShoulder;
            currentLeftShoulder = leftShoulder;
            currentRightShoulder = rightShoulder;
            currentChest = chest;
            currentCape = cape;
            currentWaist = waist;
            currentBeltAccessory = beltAccessory;
            currentBothSleeve = bothSleeve;
            currentLeftSleeve = leftSleeve;
            currentRightSleeve = rightSleeve;
            currentBothElbowpads = bothElbowpads;
            currentLeftElbowpad = leftElbowpad;
            currentRightElbowpad = rightElbowpad;
            currentBothWrists = bothWrists;
            currentLeftWrist = leftWrist;
            currentRightWrist = rightWrist;
            currentBothGloves = bothGloves;
            currentLeftGlove = leftGlove;
            currentRightGlove = rightGlove;
            currentBothKneepads = bothKneepads;
            currentLeftKneepad = leftKneepad;
            currentRightKneepad = rightKneepad;
            currentBothBoots = bothBoots;
            currentLeftBoot = leftBoot;
            currentRightBoot = rightBoot;
            ApplyCurrentOptions();
        }

        public void ApplyCurrentOptions()
        {
            ApplyOptions(SyntyModularOptions.Gender, (int) gender);
            ApplyOptions(SyntyModularOptions.Hair, currentHair);
            ApplyOptions(SyntyModularOptions.Head, currentFace);
            ApplyOptions(SyntyModularOptions.Eyebrows, currentEyebrows);
            ApplyOptions(SyntyModularOptions.FacialHair, currentFacialHair);
            ApplyOptions(SyntyModularOptions.Ears, currentEars);
            ApplyOptions(SyntyModularOptions.Chest, currentChest);
            ApplyOptions(SyntyModularOptions.Waist, currentWaist);
            ApplyOptions(SyntyModularOptions.Helmet, currentHelmet);
            ApplyOptions(SyntyModularOptions.Cowl, currentCowl);
            ApplyOptions(SyntyModularOptions.Hat, currentHat);
            ApplyOptions(SyntyModularOptions.FaceGuard, currentFaceGuard);
            ApplyOptions(SyntyModularOptions.FrontHeadAccessory, currentFrontHeadAccessory);
            // ApplyOptions(SyntyModularOptions.SideHeadAccessory, sideHeadAccessory);
            // ApplyOptions(SyntyModularOptions.BackHeadAccessory, backHeadAccessory);
            ApplyOptions(SyntyModularOptions.ShoulderLeft, currentLeftShoulder);
            ApplyOptions(SyntyModularOptions.ShoulderRight, currentRightShoulder);
            ApplyOptions(SyntyModularOptions.SleeveLeft, currentLeftSleeve);
            ApplyOptions(SyntyModularOptions.SleeveRight, currentRightSleeve);
            ApplyOptions(SyntyModularOptions.WristLeft, currentLeftWrist);
            ApplyOptions(SyntyModularOptions.WristRight, currentRightWrist);
            ApplyOptions(SyntyModularOptions.GloveLeft, currentLeftGlove);
            ApplyOptions(SyntyModularOptions.GloveRight, currentRightGlove);
            ApplyOptions(SyntyModularOptions.BootLeft, currentLeftBoot);
            ApplyOptions(SyntyModularOptions.BootRight, currentRightBoot);
            ApplyOptions(SyntyModularOptions.ElbowLeft, currentLeftElbowpad);
            ApplyOptions(SyntyModularOptions.ElbowRight, currentRightElbowpad);
            ApplyOptions(SyntyModularOptions.Cape, currentCape);
            ApplyOptions(SyntyModularOptions.BeltAccessory, currentBeltAccessory);
            ApplyOptions(SyntyModularOptions.KneeRight, currentRightKneepad);
            ApplyOptions(SyntyModularOptions.KneeLeft, currentLeftKneepad);
            ApplyOptions(SyntyModularOptions.BootLeft, currentLeftBoot);
            ApplyOptions(SyntyModularOptions.BootRight, currentRightBoot);
        }

        public void SaveOptions()
        {
            gender = currentGender;
            hair = currentHair;
            face = currentFace;
            eyebrows = currentEyebrows;
            facialHair = currentFacialHair;
            ears = currentEars;
            chest = currentChest;
            waist = currentWaist;
            helmet = currentHelmet;
            cowl = currentCowl;
            hat = currentHat;
            faceGuard = currentFaceGuard;
            frontHeadAccessory = currentFrontHeadAccessory;
            // ApplyOptions(SyntyModularOptions.SideHeadAccessory, sideHeadAccessory);
            // ApplyOptions(SyntyModularOptions.BackHeadAccessory, backHeadAccessory);
            leftShoulder = currentLeftShoulder;
            rightShoulder = currentRightShoulder;
            leftSleeve = currentLeftSleeve;
            rightSleeve = currentRightSleeve;
            leftWrist = currentLeftWrist;
            rightWrist = currentRightWrist;
            leftGlove = currentLeftGlove;
            rightGlove = currentRightGlove;
            leftBoot = currentLeftBoot;
            rightBoot = currentRightBoot;
            leftElbowpad = currentLeftElbowpad;
            rightElbowpad = currentRightElbowpad;
            cape = currentCape;
            beltAccessory = currentBeltAccessory;
            rightKneepad = currentRightKneepad;
            leftKneepad = currentLeftKneepad;
            leftBoot = currentLeftBoot;
            rightBoot = currentRightBoot;
        }

        public override void WearClothing(EadonClothingItem clothingItem)
        {
            int index = 0;

            switch (clothingItem.clothingHolderName)
            {
                case EadonClothingItem.ClothingHolderNames.HelmetClothing:
                    index = FindInList(helmetList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.Helmet, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.CowlClothing:
                    index = FindInList(cowlList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.Cowl, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.HatClothing:
                    index = FindInList(hatList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.Hat, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.FaceGuardClothing:
                    index = FindInList(faceGuardList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.FaceGuard, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.HelmetAccessoryClothing:
                    index = FindInList(frontHeadAccessoryList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.FrontHeadAccessory, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.ChestClothing:
                    index = FindInList(torsoList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.Chest, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.BackClothing:
                    index = FindInList(capeList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.Cape, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.UpperRightArmClothing:
                    index = FindInList(sleeveRightList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.SleeveRight, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.LowerRightArmClothing:
                    index = FindInList(wristRightList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.WristRight, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.BothUpperArmClothing:
                    index = FindInList(sleeveLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.SleeveLeft, index);
                        ApplyOptions(SyntyModularOptions.SleeveRight, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.UpperLeftArmClothing:
                    index = FindInList(sleeveLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.SleeveLeft, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.LowerLeftArmClothing:
                    index = FindInList(wristLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.WristLeft, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.BothLowerArmClothing:
                    index = FindInList(wristLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.WristLeft, index);
                        ApplyOptions(SyntyModularOptions.WristRight, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.RightHandClothing:
                    index = FindInList(gloveRightList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.GloveRight, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.LeftHandClothing:
                    index = FindInList(gloveLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.GloveLeft, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.BothHandsClothing:
                    index = FindInList(gloveLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.GloveLeft, index);
                        ApplyOptions(SyntyModularOptions.GloveRight, index);
                    }

                    break;
                case EadonClothingItem.ClothingHolderNames.LegsClothing:
                    index = FindInList(waistList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.Waist, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.RightFootClothing:
                    index = FindInList(bootRightList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.BootRight, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.LeftFootClothing:
                    index = FindInList(bootLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.BootLeft, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.BothFeetClothing:
                    index = FindInList(bootLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.BootLeft, index);
                        ApplyOptions(SyntyModularOptions.BootRight, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.RightShoulderAttachment:
                    index = FindInList(shoulderRightList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.ShoulderRight, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.LeftShoulderAttachment:
                    index = FindInList(shoulderLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.ShoulderLeft, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.BothShoulderAttachment:
                    index = FindInList(shoulderLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.ShoulderLeft, index);
                        ApplyOptions(SyntyModularOptions.ShoulderRight, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.RightElbowAttachment:
                    index = FindInList(elbowRightList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.ElbowRight, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.LeftElbowAttachment:
                    index = FindInList(elbowLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.ElbowLeft, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.BothElbowAttachment:
                    index = FindInList(elbowLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.ElbowLeft, index);
                        ApplyOptions(SyntyModularOptions.ElbowRight, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.RightKneeAttachment:
                    index = FindInList(kneeRightList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.KneeRight, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.LeftKneeAttachment:
                    index = FindInList(kneeLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.KneeLeft, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.BothKneeAttachment:
                    index = FindInList(kneeLeftList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.KneeLeft, index);
                        ApplyOptions(SyntyModularOptions.KneeRight, index);
                    }
                    break;
                case EadonClothingItem.ClothingHolderNames.BeltAttachment:
                    index = FindInList(beltAccessoryList, clothingItem.clothingName);
                    if (index != 0)
                    {
                        ApplyOptions(SyntyModularOptions.BeltAccessory, index);
                    }
                    break;
            }
        }

        public override void RemoveClothing(EadonClothingItem clothingItem)
        {
            switch (clothingItem.clothingHolderName)
            {
                case EadonClothingItem.ClothingHolderNames.HelmetClothing:
                    ApplyOptions(SyntyModularOptions.Helmet, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.CowlClothing:
                    ApplyOptions(SyntyModularOptions.Cowl, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.HatClothing:
                    ApplyOptions(SyntyModularOptions.Hat, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.FaceGuardClothing:
                    ApplyOptions(SyntyModularOptions.FaceGuard, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.HelmetAccessoryClothing:
                    ApplyOptions(SyntyModularOptions.FrontHeadAccessory, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.ChestClothing:
                    ApplyOptions(SyntyModularOptions.Chest, chest);
                    break;
                case EadonClothingItem.ClothingHolderNames.BackClothing:
                    ApplyOptions(SyntyModularOptions.Cape, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.UpperRightArmClothing:
                    ApplyOptions(SyntyModularOptions.SleeveRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.LowerRightArmClothing:
                    ApplyOptions(SyntyModularOptions.WristRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.BothUpperArmClothing:
                    ApplyOptions(SyntyModularOptions.SleeveLeft, 0);
                    ApplyOptions(SyntyModularOptions.SleeveRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.UpperLeftArmClothing:
                    ApplyOptions(SyntyModularOptions.SleeveLeft, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.LowerLeftArmClothing:
                    ApplyOptions(SyntyModularOptions.WristLeft, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.BothLowerArmClothing:
                    ApplyOptions(SyntyModularOptions.WristRight, 0);
                    ApplyOptions(SyntyModularOptions.WristRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.RightHandClothing:
                    ApplyOptions(SyntyModularOptions.GloveRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.LeftHandClothing:
                    ApplyOptions(SyntyModularOptions.GloveLeft, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.BothHandsClothing:
                    ApplyOptions(SyntyModularOptions.GloveLeft, 0);
                    ApplyOptions(SyntyModularOptions.GloveRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.LegsClothing:
                    ApplyOptions(SyntyModularOptions.Waist, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.RightFootClothing:
                    ApplyOptions(SyntyModularOptions.BootRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.LeftFootClothing:
                    ApplyOptions(SyntyModularOptions.BootLeft, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.BothFeetClothing:
                    ApplyOptions(SyntyModularOptions.BootLeft, 0);
                    ApplyOptions(SyntyModularOptions.BootRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.RightShoulderAttachment:
                    ApplyOptions(SyntyModularOptions.ShoulderRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.LeftShoulderAttachment:
                    ApplyOptions(SyntyModularOptions.ShoulderLeft, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.BothShoulderAttachment:
                    ApplyOptions(SyntyModularOptions.ShoulderLeft, 0);
                    ApplyOptions(SyntyModularOptions.ShoulderRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.RightElbowAttachment:
                    ApplyOptions(SyntyModularOptions.ElbowRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.LeftElbowAttachment:
                    ApplyOptions(SyntyModularOptions.ElbowLeft, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.BothElbowAttachment:
                    ApplyOptions(SyntyModularOptions.ElbowLeft, 0);
                    ApplyOptions(SyntyModularOptions.ElbowRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.RightKneeAttachment:
                    ApplyOptions(SyntyModularOptions.KneeRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.LeftKneeAttachment:
                    ApplyOptions(SyntyModularOptions.KneeLeft, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.BothKneeAttachment:
                    ApplyOptions(SyntyModularOptions.KneeLeft, 0);
                    ApplyOptions(SyntyModularOptions.KneeRight, 0);
                    break;
                case EadonClothingItem.ClothingHolderNames.BeltAttachment:
                    ApplyOptions(SyntyModularOptions.BeltAccessory, 0);
                    break;
            }
        }

        public override void SetToDefaults()
        {
            foreach (var holder in clothingEquipmentHolders)
            {
                holder.ResetToDefault();
            }
        }

        public void ReplaceMaterial()
        {
            foreach (var gameObject in headList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in eyebrowList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in hairList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in facialHairList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in earsList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in helmetList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in cowlList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in faceGuardList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in hatList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in torsoList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in waistList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in sleeveRightList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in sleeveLeftList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in wristRightList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in wristLeftList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in gloveRightList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in gloveLeftList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in bootRightList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in bootLeftList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in frontHeadAccessoryList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in capeList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in beltAccessoryList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in kneeRightList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in kneeLeftList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in elbowRightList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in elbowLeftList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in shoulderRightList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }

            foreach (var gameObject in shoulderLeftList)
            {
                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.material = overrideMaterial == null ? originalMaterial : overrideMaterial;
                }
            }
        }
    }
}
#endif
