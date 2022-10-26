// SJM Tech
// www.sjmtech3d.com
//
// Unofficial First Person Camera AddOn for Invector Basic/Melee/Shooter Template.
//
// rev. 2.51
//           
// use:
// 1 - drop the inVector Controller prefab on your scene ad add this script on it.
// 2 - remove all ThirdCamera by invector in scene; add a new camera and set it as mainCamera (or add a custom camera in "PC Camera" field.
// 3 - use the ContextMenu to set the camera in the right position.
//
//

using Invector;
using Invector.vCharacterController;
using Invector.vEventSystems;
using UnityEngine;

[RequireComponent(typeof(vHeadTrack))]
[vClassHeader(" First person Camera ", "Use context Menu > Set Camera position to initialize", iconName = "FPCameraIcon")]
public class vFirstPersonCamera : vMonoBehaviour
{
    [Header("Camera Position Settings")]
    [Space(5)]
    [Tooltip("Set the camera used in FPCamera mode. If empty the MainCamera will be used")]
    public Camera fpCamera;
    [Tooltip("Set the FPCamera Near Plane")]
    public float cameraNearClip = 0.01f;
    [Tooltip("Set the FPCamera Y offset from the head bone")]
    public float cameraYOffset = 0.1f;
    [Tooltip("Set the FPCamera Z offset from the head bone")]
    public float cameraZOffset = 0.02f;
    [Space(5)]

    [Tooltip("enable head collision to prevent camera goes into the objects")]
    public bool enableHeadCollider = true;
    [vHideInInspector("enableHeadCollider")]
    [Tooltip("show head collision Gizmos")]
    public bool showGizmos = true;
    [vHideInInspector("enableHeadCollider")]
    [Tooltip("head collision radius")]
    public float colliderRadius = 0.12f;
    [vHideInInspector("enableHeadCollider")]
    [Tooltip("head collision center")]
    public Vector3 colliderCenter = new Vector3(0, 0.1f, 0.04f);
    [Space(5)]

    [Header("Camera Rotation Settings")]
    [Space(5)]
    [Tooltip("the speed of the camera rotation")]
    [Range(0.1f, 3f)]
    public float cameraRotationSpeed = 0.5f;
    [Tooltip("enable the camera extra smoothing")]
    public bool useExtraSmoothing = false;
    [vHideInInspector("useExtraSmoothing")]
    [Tooltip("the smoothing of the camera rotation")]
    public float smoothingValue = 1f;
    [Space(5)]
    [Tooltip("the maximum vertical camera rotation angle")]
    [Range(40f, 80f)]
    public float upAngleLimit = 50f;
    [Tooltip("the minimum vertical camera rotation angle")]
    [Range(40f, 85f)]
    public float downAngleLimit = 85f;
    [Space(5)]
    [Tooltip("the Horizontal clamp angle for head look during actions")]
    [Range(0f, 90f)]
    public float actionHAngleLimit = 90f;
    [Tooltip("the Down Clamp angle for head look during actions")]
    [Range(40f, 85f)]
    public float actionDownAngleLimit = 55f;
    [Space(5)]
    [Tooltip("should be enabled to add Shooter AimCanvas at runtime")]
    public bool usingShooter = false;
    [vHideInInspector("usingShooter")]
    [Tooltip("sway and recoil amount")]
    public float swayRecoilAmount = 1;
    [Tooltip("align melee actions to the camera direction")]
    public bool usingMelee = false;
    [vHideInInspector("usingMelee")]
    [Tooltip("set melee week attack key used")]
    public KeyCode weekAttackKey = KeyCode.Mouse0;
    [vHideInInspector("usingMelee")]
    [Tooltip("set melee strong attack key used")]
    public KeyCode strongAttackKey = KeyCode.Alpha1;

    [Header("Body Rotation Settings")]
    [Space(5)]
    [Tooltip("Set the default Animator Update Mode")]
    public AnimatorUpdateMode animatorUpdateMode = AnimatorUpdateMode.AnimatePhysics;
    [Space(5)]
    [Tooltip("Set the body IK reactivity respect head rotation")]
    [Range(0f, 1f)]
    public float bodyIKWeight = 0.43f;
    [Space(5)]
    [Tooltip("the threshold angle between head and body beyond which the rotation begins")]
    [Range(0f, 70f)]
    public float RotationThld = 55f;
    [Space(5)]



    [Header("Controller Options")]
    [Space(5)]
    [Tooltip("use cinematic camera during DEFAULT actions")]
    public bool cinematicOnActions = false;
    [Tooltip("use cinematic camera by external calls")]
    public bool cinematicOnRequest = true;
    [Tooltip("lock and hide the mouse cursor")]
    public bool lockMouseCursor = true;
    [Tooltip("add Crosshair UI prefab at start")]
    public bool addCrosshair = true;            // Crosshair UI spawning
    [Tooltip("add ExtraCams prefab at start")]
    public bool addExtraCams = false; 			// Extra cams spawning
    [Tooltip("disable camera look")]
    public bool disableMouseInput = false;
    [vHideInInspector("disableMouseInput")]
    public bool resetHeadPosition = false;

    private GameObject extraCamsPrefab;
    private GameObject crosshairPrefab;
    private GameObject crosshairInstance;
    private GameObject extraCamsInstance;

    private Vector2 offsetMouse;
    private Vector2 recoilMouse;
    private bool isAction = false;              //Force action status (Integrations)
    private bool isCinematic;                   //disable mouse user input (Cinematic)
    private bool isUpdateModeNormal = false;
    private bool stateDone = false;

    // Shooter requisite
    private GameObject aimCanvas;
    private GameObject aimInstance;

    // inVector references
    private vThirdPersonInput vInput;
    private vHeadTrack vHeadT;

    // Animator and Bones
    private Animator animator;
    private Transform headBone;
    private GameObject headBoneRef;
    private GameObject headBoneRotCorrection;

    private bool isRight;
    //private bool isRotating;

    // Animator StateInfo
    public vAnimatorStateInfos animatorStateInfos;
    private bool isCustomAction = false;
    private bool isAiming = false;

    //private SphereCollider headCollider;

    // Player Rotation Correction
    private float x = 0;
    private float y = 0;
    private Vector3 originalRotation;
    private Vector3 fpCameraLocapPosition;
    private bool lateUpdateSync;
    private GameObject headCollider;
    private bool headColliderStatus = true;

    // camera transition
    private bool startCameraTransition = false;
    private Vector3 startCameraLocalPosition;
    private Transform cameraTransform;
    private Vector3 endCameraPosition;
    private float startTime;
    private float transitionSpeed;

    void OnDisable()
    {
        if (animatorStateInfos != null)
        {
            animatorStateInfos.RemoveListener();
        }

        if (headBoneRotCorrection != null)
        {
            headBoneRotCorrection.SetActive(false);

            if (headColliderStatus)
            {
                enableHeadCollider = false;
                headCollider.SetActive(false);
            }

            if (extraCamsInstance != null)
                extraCamsInstance.SetActive(false);

            if (crosshairInstance != null)
                crosshairInstance.SetActive(false);

            // if using extraCams change aimCanvas render mode to ScreenSpaceCamera to be centered correctly.
            if (usingShooter && addExtraCams)
            {
                foreach (RectTransform child in aimInstance.transform)
                {
                    child.offsetMax = new Vector2(0, 0);
                }
            }
        }
    }

    void OnEnable()
    {
        if (animatorStateInfos != null)
        {
            animatorStateInfos.RegisterListener();
        }

        if (headBoneRotCorrection != null)
        {
            headBoneRotCorrection.transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
            headBoneRotCorrection.SetActive(true);
            headCollider.SetActive(headColliderStatus);
            enableHeadCollider = true;
            vInput.cc.locomotionType = vThirdPersonMotor.LocomotionType.OnlyStrafe;
            vInput.cc.sprintOnlyFree = false;

            vInput.cc.strafeSpeed.rotateWithCamera = false;

            if (extraCamsInstance != null)
                extraCamsInstance.SetActive(true);

            if (crosshairInstance != null)
                crosshairInstance.SetActive(true);

            // if using extraCams change aimCanvas render mode to ScreenSpaceCamera to be centered correctly.
            if (usingShooter && addExtraCams)
            {
                foreach (RectTransform child in aimInstance.transform)
                {
                    child.offsetMax = new Vector2(0, child.rect.height * fpCamera.rect.y);
                }
            }
        }
    }

    void Awake()
    {
        // auto add aimCanvas default prefab
        if (usingShooter && aimInstance == null)
        {
            aimCanvas = Resources.Load("AimCanvas") as GameObject;
            aimInstance = Instantiate(aimCanvas);
            aimInstance.name = "AimCanvas";
        }

        // add extraCams end resize camera rect
        if (addExtraCams && extraCamsInstance == null)
        {
            extraCamsPrefab = Resources.Load("ExtraCams") as GameObject;
            extraCamsInstance = Instantiate(extraCamsPrefab);
            extraCamsInstance.name = "ExtraCams";
            fpCamera.rect = new Rect(0, -0.25f, 1, 1);
            // if using extraCams change aimCanvas render mode to ScreenSpaceCamera to be centered correctly.
            if (usingShooter)
            {
                foreach (RectTransform child in aimInstance.transform)
                {
                    child.offsetMax = new Vector2(0, child.rect.height * fpCamera.rect.y);
                }
            }
        }

        // add simple crosshair canvas
        if (addCrosshair && crosshairInstance == null)
        {
            crosshairPrefab = Resources.Load("CrosshairUICanvas") as GameObject;
            crosshairInstance = Instantiate(crosshairPrefab, Vector3.zero, Quaternion.identity);
            Canvas crosshairCanvas = crosshairInstance.GetComponent<Canvas>();
            crosshairCanvas.worldCamera = Camera.main;
            crosshairCanvas.planeDistance = cameraNearClip + 0.01f;
            crosshairInstance.name = "CrosshairUI";
        }
    }

    void Start()
    {
        // if there is no custom camera ... use the main camera.
        if (fpCamera == null)
        {
            fpCamera = Camera.main.gameObject.GetComponent<Camera>();
        }

        fpCamera.gameObject.tag = "MainCamera";
        fpCamera.gameObject.layer = 8;

        // set the optimal near clip plane and depth
        fpCamera.GetComponent<Camera>().nearClipPlane = cameraNearClip;
        fpCamera.GetComponent<Camera>().depth = 2;

        // set vTPC reference
        vInput = GetComponent<vThirdPersonInput>();
        vInput.cc.locomotionType = vThirdPersonMotor.LocomotionType.OnlyStrafe;
        vInput.cc.sprintOnlyFree = false;
        vInput.cc.strafeSpeed.rotateWithCamera = false;

        vHeadT = GetComponent<vHeadTrack>();
        vHeadT.strafeBodyWeight = bodyIKWeight;

        // find the head bone
        animator = GetComponent<Animator>();
        headBone = animator.GetBoneTransform(HumanBodyBones.Head);

        // add animation state listener
        animatorStateInfos = new vAnimatorStateInfos(animator);
        animatorStateInfos.RegisterListener();

        animator.updateMode = animatorUpdateMode;

        if (animator.updateMode != AnimatorUpdateMode.AnimatePhysics)
        {
            isUpdateModeNormal = true;
        }
        else
        {
            isUpdateModeNormal = false;
        }

        // create head collision object
        headColliderStatus = enableHeadCollider;

        if (enableHeadCollider)
        {
            headCollider = new GameObject("HeadCollision");
            headCollider.AddComponent<vFPCameraHeadCollider>();
            headCollider.layer = 15;
            headCollider.tag = "Player";
            headCollider.AddComponent<SphereCollider>();
            headCollider.GetComponent<SphereCollider>().radius = colliderRadius;
            headCollider.GetComponent<SphereCollider>().center = colliderCenter;
            headCollider.transform.parent = this.transform;
            headCollider.transform.localRotation = Quaternion.identity;
        }

        // create bones reference
        headBoneRef = new GameObject("HeadRef");
        headBoneRotCorrection = new GameObject("FPCameraRoot");
        headBoneRotCorrection.AddComponent<vFPCameraRoot>();

        // position bones reference
        headBoneRef.transform.position = headBone.transform.position;
        headBoneRotCorrection.transform.position = headBone.transform.position;
        headBoneRotCorrection.transform.rotation = headBone.transform.root.rotation;
        headBoneRef.transform.rotation = headBone.transform.rotation;
        headBoneRef.transform.parent = headBoneRotCorrection.transform;
        //headBoneRotCorrection.transform.parent = this.transform;

        // camera position
        fpCamera.transform.position = headBoneRotCorrection.transform.position + (transform.root.forward * cameraZOffset) + (transform.root.up * cameraYOffset);
        fpCamera.transform.parent = headBoneRotCorrection.transform;
        fpCamera.transform.localRotation = Quaternion.identity;
        fpCameraLocapPosition = fpCamera.transform.localPosition;
        endCameraPosition = fpCamera.transform.localPosition;

        // set initial horizontal and vertical axes
        originalRotation = transform.eulerAngles;
        x = originalRotation.y;
        y = originalRotation.x;

        // find and stop "UnderBody" animator layer to reduce lags during camera free look
        for (int i = 0; i < animator.layerCount; i++)
        {
            if (animator.GetLayerName(i) == "UnderBody")
            {
                animator.SetLayerWeight(i, 0);
            }
        }
    }

    void FixedUpdate()
    {
        lateUpdateSync = true;

        if (!vInput.cc.ragdolled && Time.timeScale != 0)
        {
            if (!isCinematic)
            {
                if (!isAction)
                {
                    CameraLook();
                }
            }
        }
    }

    void LateUpdate()
    {
        if (isUpdateModeNormal)
        {
            lateUpdateSync = true;
        }

        if (lateUpdateSync)
        {
            lateUpdateSync = false;

            if (!vInput.cc.ragdolled && Time.timeScale != 0)
            {
                if (cinematicOnActions)
                {
                    if (vInput.cc.customAction)
                    {
                        isCinematic = true;
                        stateDone = false;
                    }
                    else if (!vInput.cc.customAction && !stateDone)
                    {
                        isCinematic = false;
                        stateDone = true;
                    }
                }

                CameraHeadBonePosition();

                if (isCinematic)
                {
                    y = 0;
                    x = headBoneRotCorrection.transform.eulerAngles.y;
                    headBoneRotCorrection.transform.rotation = headBone.transform.rotation;
                    fpCamera.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    CharacterRotation();
                    FaceToCamera();

                    if (isAction)
                    {
                        CameraLook();
                    }

                    CameraHeadBoneRotation();
                }
            }
            else
            {
                // no user input during ragdoll
                headBoneRotCorrection.transform.position = headBone.transform.position;
                headBoneRotCorrection.transform.rotation = headBone.transform.rotation;
                fpCamera.transform.localRotation = Quaternion.identity;
                fpCamera.transform.localPosition = fpCameraLocapPosition;
            }
        }
    }

    void Update()
    {
        if (animatorStateInfos.HasTag("CustomAction") || animatorStateInfos.HasTag("LockMovement"))
        {
            isCustomAction = true;
        }
        else
        {
            isCustomAction = false;
        }

        if (animatorStateInfos.HasTag("Headtrack"))
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        if (lockMouseCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (animator.updateMode != AnimatorUpdateMode.AnimatePhysics)
        {
            isUpdateModeNormal = true;
        }
        else
        {
            isUpdateModeNormal = false;
        }
    }

    void CameraHeadBonePosition()
    {

        headBoneRotCorrection.transform.position = headBone.transform.position;
        if (enableHeadCollider)
            headCollider.transform.position = headBone.transform.position;


    }

    void CameraHeadBoneRotation()
    {
        headBone.rotation = headBoneRef.transform.rotation;
        if (enableHeadCollider)
            headCollider.transform.rotation = headBone.transform.rotation;
    }

    void CameraLook()
    {
        float headInputYAxis;
        float headInputXAxis;

        if (!disableMouseInput)
        {
            headInputYAxis = vInput.rotateCameraYInput.GetAxis(); // normal vertical mouse input
            headInputXAxis = vInput.rotateCameraXInput.GetAxis(); // normal Horizontal mouse input
        }
        else
        {
            headInputYAxis = 0;
            headInputXAxis = 0;
            if (resetHeadPosition)
            {
                x = 0;
                y = 0;

            }
        }

        y += headInputYAxis * cameraRotationSpeed;

        if (vInput.cc.customAction || isAction || isCustomAction)
        {
            if (headInputXAxis != 0)
            {
                x += headInputXAxis * cameraRotationSpeed;
            }
            else
            {
                x = headBoneRotCorrection.transform.eulerAngles.y;
            }
        }
        else
        {
            x += headInputXAxis * cameraRotationSpeed;
        }

        float minX = transform.eulerAngles.y + (-actionHAngleLimit);
        float maxX = transform.eulerAngles.y + actionHAngleLimit;

        float tempx = Mathf.Clamp(x, minX, maxX);

        if (x > tempx + 100)
        {
            x -= 360;
        }
        else if (x < tempx - 100)
        {
            x += 360;
        }

        if (vInput.cc.customAction || isAction || isCustomAction)
        {
            x = Mathf.Clamp(x, minX, maxX);
            y = Mathf.Clamp(y, -actionDownAngleLimit, upAngleLimit);
        }
        else
        {
            y = Mathf.Clamp(y, -downAngleLimit, upAngleLimit);
        }

        float correctionY = (recoilMouse.x + offsetMouse.x) * swayRecoilAmount;
        float correctionX = (recoilMouse.y + offsetMouse.y) * swayRecoilAmount;

        // Head rotation

        Quaternion defRotation = Quaternion.Euler(-y + correctionY, x + correctionX, 0f);

        if (!useExtraSmoothing)
        {
            //headBoneRotCorrection.transform.rotation = defRotation; // no smoothing
            headBoneRotCorrection.transform.rotation = Quaternion.Lerp(headBoneRotCorrection.transform.rotation, defRotation, cameraRotationSpeed);
        }
        else
        {
            headBoneRotCorrection.transform.rotation = Quaternion.Lerp(headBoneRotCorrection.transform.rotation, defRotation, smoothingValue * 10 * Time.deltaTime);        // smoothing by time
                                                                                                                                                                            //headBoneRotCorrection.transform.rotation = Quaternion.Lerp(headBoneRotCorrection.transform.rotation,defRotation,1/smoothingValue); 					        // smoothing by frame
        }
    }

    void FaceToCamera()
    {
        if (usingMelee && !disableMouseInput)
        {
            if (Input.GetKey(weekAttackKey) || Input.GetKey(strongAttackKey))
            {
                if (!isAiming)
                {
                    //vInput.cc.RotateToDirection(fpCamera.transform.position - transform.position);
                    vInput.cc.RotateToDirection(fpCamera.transform.position - transform.position, vInput.cc.strafeSpeed.rotationSpeed);
                    //RotateToTarget(fpCamera.transform);
                }
            }
        }
    }

    void CharacterRotation()
    {
        Quaternion newRotation = Quaternion.identity;
        // rotate the body only when there is no movement
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            // Get camera forward in the character's rotation space
            Vector3 camRelative = transform.InverseTransformDirection(fpCamera.transform.forward);

            // Get the angle of the camera forward relative to the character forward
            float angle = Mathf.Atan2(camRelative.x, camRelative.z) * Mathf.Rad2Deg;
            float a = 0;

            // check the angle threshold
            if (Mathf.Abs(angle) > Mathf.Abs(RotationThld))
            {
                a = angle - RotationThld;
                if (angle < 0)
                    a = angle + RotationThld;

                // Body Rotation
                if (isAction || isCustomAction || vInput.cc.customAction)
                {
                    return;
                }
                else
                {
                    newRotation = Quaternion.AngleAxis(a, transform.up) * transform.rotation;

                    if (!isUpdateModeNormal)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.fixedDeltaTime * vInput.cc.strafeSpeed.rotationSpeed);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * vInput.cc.strafeSpeed.rotationSpeed);
                    }
                }
            }
            else
            {
                newRotation = transform.rotation;
            }
        }
    }


    // camera sway
    public void SetOffset(Vector2 value)
    {
        offsetMouse = value; // shooter camera sway
    }
    // camera recoil effect
    public void SetRecoil(Vector2 value)
    {
        recoilMouse = value; // shooter camera recoil
    }

    public void SetMeleeRotationMode(bool status)
    {
        usingMelee = status;
    }

    // call this to stop rotating body when threshold is reached.
    //  and when you need the camera rotate with body when there is no mouse inputs. (riding, driving...)
    public void IsAction(bool status)
    {
        isAction = status;
        if (isAction)
        {
            headBoneRotCorrection.transform.parent = this.transform.root;
        }
        else
        {
            headBoneRotCorrection.transform.parent = null;
        }
    }

    // call this to use cinematic camera movement.
    public void IsCinematic(bool state)
    {
        if (cinematicOnRequest) isCinematic = state;
    }

    public void OnDrawGizmosSelected()
    {
        if (fpCamera != null && showGizmos && enableHeadCollider)
        {
            animator = GetComponent<Animator>();
            headBone = animator.GetBoneTransform(HumanBodyBones.Head);
            Gizmos.color = new Color(0, 1, 1, 0.3f);
            Gizmos.DrawSphere(headBone.transform.position + (headBone.transform.forward * colliderCenter.z) + headBone.transform.up * colliderCenter.y + headBone.transform.right * colliderCenter.x, colliderRadius);
        }
    }
    // add smooth transition from third to first camera WIP
    public void CameraTransition(Transform startPos, float tTime)
    {
        //vInput.cc.RotateToTarget(startPos);

        cameraTransform = startPos;
        //cameraTransform.LookAt(headBoneRef.transform);

        startCameraLocalPosition = fpCamera.transform.InverseTransformPoint(cameraTransform.position);

        x = transform.eulerAngles.y;
        y = transform.eulerAngles.x;

        transitionSpeed = tTime;
        startCameraTransition = true;
        startTime = Time.time;


    }

    public void LockCursor(bool status)
    {
        lockMouseCursor = status;
    }
    public void DisablePlayerInputs(bool status)
    {
        disableMouseInput = status;
    }

    // manual camera positioning
    [ContextMenu("FP camera > Set Camera Position")]
    void SetCameraPos()
    {
        animator = GetComponent<Animator>();
        headBone = animator.GetBoneTransform(HumanBodyBones.Head);
        if (fpCamera == null)
        {
            fpCamera = Camera.main.gameObject.GetComponent<Camera>();
        }
        fpCamera.GetComponent<Camera>().nearClipPlane = cameraNearClip;
        fpCamera.GetComponent<Camera>().depth = 2;
        fpCamera.transform.position = headBone.position + (transform.root.forward * cameraZOffset) + (transform.root.up * cameraYOffset);
        fpCamera.transform.parent = headBone.transform.root;
        fpCamera.transform.rotation = transform.rotation;
    }


    void RotateToTarget(Transform target)
    {
        if (target)
        {
            Quaternion rot = Quaternion.LookRotation(target.position - transform.root.position);
            var newPos = new Vector3(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newPos), vInput.cc.strafeSpeed.rotationSpeed * Time.deltaTime);

        }
    }
}
