// SJM Tech
// www.sjmtech3d.com
//
// Simple First/Third Camera Swap
//
// rev. 2.33
//
// assign the ThrdCamera prefab from project tab
//

using Invector;
using Invector.vCamera;
using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(vFirstPersonCamera))]
[vClassHeader(" First person Camera SWAP ", "Assign your Third Camera prefab", iconName = "FPCameraSwapIcon")]
public class vFirstPersonCameraSwap : vMonoBehaviour
{

    [Space(1)]
    [Header("Settings")]

    [Tooltip("Assign thrdCamera prefab from project tab")]
    public GameObject thrdCameraPrefab;
    // [Tooltip("Swap transition duration")]
    // public float swapTime = 0.1f;
    [Tooltip("Assign keyboard button for camera swap camera mode")]
    public KeyCode swapKey = KeyCode.Backspace;
    [Tooltip("Set thrdCamera as default camera on start")]
    public bool defaultIsThirdCamera = false;

    [Space(1)]
    [Header("Third Person Settings")]

    [Tooltip("Set default loomotion type")]
    public vThirdPersonMotor.LocomotionType defaultThirdLocomotion;
    [Tooltip("Force strafe mode in FreeWithStrafe")]
    public bool thrdCameraDefaultStrafe = false;



    public UnityEvent OnSwapThirdPerson;
    public UnityEvent OnSwapFirstPerson;
    private GameObject thrdCameraObject;
    private vFirstPersonCamera fPCamera;
    private bool thrdCameraModeLast;
    private vThirdPersonInput vInput;
    public vHeadTrack headTrack;
    private Camera currentMainCamera;


    //public Component shooterManager;

    void Start()
    {
        vInput = GetComponent<vThirdPersonInput>();
        fPCamera = GetComponent<vFirstPersonCamera>();
        headTrack = GetComponent<vHeadTrack>();
    }
    void OnSwapThrd()
    {
        OnSwapThirdPerson.Invoke();
    }

    void OnSwapFrst()
    {
        OnSwapFirstPerson.Invoke();
    }

    void Update()
    {
        if (thrdCameraModeLast != defaultIsThirdCamera)
        {
            if (defaultIsThirdCamera)
            {

                fPCamera.enabled = false;
                thrdCameraObject = Instantiate(thrdCameraPrefab, transform.position, transform.rotation);
                thrdCameraObject.name = "vThirdCamera";
                thrdCameraObject.GetComponent<vThirdPersonCamera>().Init();


                //thrdCameraObject.GetComponent<vThirdPersonCamera>().SetMainTarget(this.transform);
                //GetComponent<vThirdPersonController>().Init();
                //vInput.tpCamera = FindObjectOfType<vThirdPersonCamera>();

                currentMainCamera = Camera.main;
                vInput.cameraMain = currentMainCamera;
                vInput.cc.rotateTarget = FindObjectOfType<vThirdPersonCamera>().transform;
                headTrack.cameraMain = currentMainCamera;

                //gameObject.SendMessage("CameraSwap", SendMessageOptions.DontRequireReceiver);

                OnSwapThrd();
                vInput.cc.locomotionType = defaultThirdLocomotion;


                if (thrdCameraDefaultStrafe)
                {
                    vInput.cc.isStrafing = true;
                }
                else
                {
                    vInput.cc.isStrafing = false;
                }

                thrdCameraModeLast = defaultIsThirdCamera;

            }
            else
            {
                OnSwapFrst();
                Vector3 thirdCamPos = thrdCameraObject.transform.position;
                //vInput.cc.RotateToDirection(Camera.main.transform.forward,true);

                if (thrdCameraObject != null)
                {
                    Destroy(thrdCameraObject);
                }

                fPCamera.enabled = true;
                //fPCamera.CameraTransition(thrdCameraObject.transform,swapTime);
                thrdCameraModeLast = defaultIsThirdCamera;
            }
        }

        // swap by key
        if (Input.GetKeyDown(swapKey))
        {
            FpcSwap();
        }

    }

    // public methods
    public void FpcSwap()
    {
        defaultIsThirdCamera = !defaultIsThirdCamera;
    }

    public void FpcSetThirdMode(bool value)
    {
        defaultIsThirdCamera = value;
    }
}
