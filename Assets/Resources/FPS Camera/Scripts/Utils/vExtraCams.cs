// SJM Tech
// www.sjmtech3d.com
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector;
using Invector.vCharacterController;

[vClassHeader(" Extra Camera views", "adds three third cameras which follows the player", iconName = "FPCameraExtraCIcon")]
public class vExtraCams : vMonoBehaviour {

	private Transform target;
	private vThirdPersonController player;
	public float rotationSpeed=4f;
	public float followSpeed=12f;
	public float height=0.6f;
	private float capsuleH;
	
	private Animator animator;
	private Transform headBone;

	void Start () {
		
		player = (vThirdPersonController)FindObjectOfType(typeof(vThirdPersonController));
		player= Object.FindObjectOfType<vThirdPersonController>();
		target=player.transform;
	}

	void FixedUpdate () {
		capsuleH=player.GetComponent<CapsuleCollider>().height;
		float offset=height+capsuleH;
		Vector3 defPosition=target.position+new Vector3(0,offset,0);
		transform.position = Vector3.Lerp(transform.position, defPosition, Time.deltaTime * followSpeed);
		transform.rotation= Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x,target.eulerAngles.y,transform.eulerAngles.z), Time.deltaTime * rotationSpeed);
		
	}
}
