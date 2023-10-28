using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{

    public static UIManager main;
    void Awake()
    {
        main = this;
    }

    [SerializeField]
    private Animator animator;
    private AfterAnimationCallback afterOpenCallback;
    private AfterAnimationCallback afterCloseCallback;


    public void OpenCurtains(AfterAnimationCallback afterAnimation)
    {
        afterOpenCallback = afterAnimation;
        animator.SetTrigger("Open");
    }
    public void CloseCurtains(AfterAnimationCallback afterAnimation)
    {
        afterCloseCallback = afterAnimation;
        animator.SetTrigger("Close");
    }


    public void CloseAnimationFinished()
    {
        afterCloseCallback();
    }
    public void OpenAnimationFinished()
    {
        afterOpenCallback();
    }
}

public delegate void AfterAnimationCallback();