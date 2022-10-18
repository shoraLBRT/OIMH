using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFog : MonoBehaviour
{
    [SerializeField]
    private float BaseFog;
    [SerializeField]
    private float EndFog;

    private void Update()
    {
        BaseFog = Mathf.MoveTowards(BaseFog, EndFog, Time.deltaTime * 0.5f);
        RenderSettings.fogDensity = BaseFog;
    }
}
