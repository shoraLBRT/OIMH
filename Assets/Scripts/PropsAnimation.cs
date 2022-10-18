using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PropsAnimation : MonoBehaviour
{
    [SerializeField] private GameObject interactBtn; 
    private Animator _animator;
    private bool _isOpen;

    public bool isOpen { get => _isOpen; }


    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void CloseOpenAnimation(bool Open)
    {
        if (Open)
        {
            _animator.SetBool("Open", true);
            _isOpen = true;
        }
        else if (!Open)
        {
            _animator.SetBool("Open", false);
            _isOpen = false;
        }
    }

   public void ViewCanvas(bool isView)
    {
        if (isView)
        {
          interactBtn.SetActive(true);
        }
        else if (!isView)
        {
            interactBtn.SetActive(false);
        }
    }
}
