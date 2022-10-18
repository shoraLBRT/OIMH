using UnityEngine;
using System.Collections;
using Invector.vCharacterController;
namespace Invector.vShooter
{
    [vClassHeader("Shooter Lock-On")]
    public class vLockOnShooter : vLockOn
    {

        public GameObject canvas;
        public GameObject button;
        protected vShooterMeleeInput shooterMelee;

        protected override void Start()
        {
            base.Start();
            shooterMelee = this.tpInput as vShooterMeleeInput;
        }


        protected override void UpdateLockOn()
        {
            if (shooterMelee == null || shooterMelee.shooterManager == null ||(shooterMelee.shooterManager.useLockOn && shooterMelee.shooterManager.rWeapon != null) || shooterMelee.shooterManager.useLockOnMeleeOnly && shooterMelee.shooterManager.rWeapon == null)
                base.UpdateLockOn();
            else if (isLockingOn && shooterMelee.shooterManager.rWeapon != null)
            {             
                isLockingOn = false;
                LockOn(false);
                StopLockOn();
                aimImage.transform.gameObject.SetActive(false);
            }
        }

        protected override void LockOnInput()
        {
            if (tpInput.tpCamera == null || tpInput.cc == null) return;
            // lock the camera into a target, if there is any around
            if (lockOnInput.GetButtonDown() && !tpInput.cc.customAction)
            {
                isLockingOn = !isLockingOn;
                LockOn(isLockingOn);
            }
            // unlock the camera if the target is null
            else if (isLockingOn && tpInput.tpCamera.lockTarget == null)
            {
                isLockingOn = false;
                LockOn(false);
            }
            // choose to use lock-on with strafe of free movement
            if (strafeWhileLockOn && !tpInput.cc.locomotionType.Equals(vThirdPersonMotor.LocomotionType.OnlyStrafe))
            {
                if (shooterMelee.isAimingByInput || strafeWhileLockOn && isLockingOn && tpInput.tpCamera.lockTarget != null)
                    tpInput.cc.lockInStrafe = true;
                else
                    tpInput.cc.lockInStrafe = false;
            }


        }
       public void VATSon()
        {
            if(shooterMelee.VATS.GetButtonDown())
            {
                canvas.SetActive(true);
                button.SetActive(true);
                shooterMelee.ShowCursor(true);
                shooterMelee.LockCursor(true);
                shooterMelee.SetLockShooterInput(true);
                LockOn(isLockingOn);
                LockOn(true);
                shooterMelee.ChangeCameraStateWithLerp("VATS");
                Time.timeScale = .5f;
                base.UpdateLockOn();
            }

            else if (shooterMelee.VATS.GetButtonDown())
            {
                canvas.SetActive(true);
                shooterMelee.ShowCursor(false);
                shooterMelee.LockCursor(false);
                shooterMelee.SetLockAllInput(false);
                isLockingOn = false;
                LockOn(false);
                shooterMelee.ChangeCameraStateWithLerp("Default");
                Time.timeScale = 1f;
                base.UpdateLockOn();
            }
        }
        public void VATSoff()
        {           
           if (isLockingOn == true)
            {
                canvas.SetActive(false);
                shooterMelee.ShowCursor(false);
                shooterMelee.LockCursor(false);
                shooterMelee.SetLockAllInput(false);
                isLockingOn = false;
                LockOn(false);
                shooterMelee.ChangeCameraStateWithLerp("Default");
                Time.timeScale = 1f;
                base.UpdateLockOn();
            }
        }
    }


}