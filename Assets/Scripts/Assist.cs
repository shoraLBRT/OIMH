using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assist : MonoBehaviour
{

    public float Radius = 20f;
    void Update()
    {
        RaycastHit hit;

        if (Physics.SphereCast(this.transform.position, Radius, this.transform.forward, out hit, 1 << LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Guard")))
        {
            GameObject aimTo = hit.transform.gameObject;
            Vector3 direction = aimTo.transform.position - this.transform.position;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), .1f);
        }
    }
}
