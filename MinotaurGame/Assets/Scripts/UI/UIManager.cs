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


    [SerializeField]
    private UIPoppingText poppingTextPrefab;
    [SerializeField]
    private Transform poppingTextContainer;

    public void ShowPoppingText(Vector3 position, string message)
    {
        UIPoppingText poppingText = Instantiate(poppingTextPrefab, poppingTextContainer);
        poppingText.Show(position, message);
    }

    public void OpenCurtains(AfterAnimationCallback afterAnimation)
    {
        MusicPlayer.main.LevelFadeIn();
        afterOpenCallback = afterAnimation;
        animator.SetTrigger("Open");
    }
    public void CloseCurtains(AfterAnimationCallback afterAnimation)
    {
        MusicPlayer.main.LevelFadeOut();
        afterCloseCallback = afterAnimation;
        animator.SetTrigger("Close");
    }


    public void CloseAnimationFinished()
    {
        afterCloseCallback();
    }
    public void OpenAnimationFinished()
    {
        Debug.Log("curtainOpenCalle");
        afterOpenCallback();
    }
}

public delegate void AfterAnimationCallback();