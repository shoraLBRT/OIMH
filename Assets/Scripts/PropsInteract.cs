using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsInteract : MonoBehaviour
{
    private PropsAnimation _propsAnimation;

    private void Update()
    {
        if(_propsAnimation != null)
        {
            CloseOpenProps();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        _propsAnimation = other.GetComponent<PropsAnimation>();
        if(_propsAnimation != null)
        {   
            _propsAnimation.ViewCanvas(true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
      if (_propsAnimation != null)
      {
        _propsAnimation.ViewCanvas(false);
        _propsAnimation = null;
       } 
     
}
    private void CloseOpenProps()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_propsAnimation.isOpen)
            {
              _propsAnimation.CloseOpenAnimation(true);
            }
            else if (_propsAnimation.isOpen)
            {
              _propsAnimation.CloseOpenAnimation(false);
            }
        }
    }
}
