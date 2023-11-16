using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private bool isOpen = false;
    public bool IsOpen { get { return isOpen; } }

    private bool isAnimating = false;

    public void Open()
    {
        if (!isAnimating)
        {
            isOpen = true;
            isAnimating = true;
            animator.Play("doorOpen");
        }
    }

    public void Disable()
    {
        isOpen = false;
    }

    public void OpenAnimationFinished()
    {
        isOpen = true;
    }
}
