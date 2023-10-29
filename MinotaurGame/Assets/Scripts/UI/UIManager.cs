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
    private GameObject startGameScreen;
    [SerializeField]
    private GameObject endGameScreen;


    [SerializeField]
    private UIPoppingText poppingTextPrefab;
    [SerializeField]
    private Transform poppingTextContainer;

    [SerializeField]
    private UILevelScoreDisplay scoreDisplay;

    public void HideStart()
    {
        startGameScreen.SetActive(false);
    }

    public void ShowEnd()
    {
        endGameScreen.SetActive(true);
    }

    public void ShowLevelScore(int levelIndex, LevelScore levelScore)
    {
        scoreDisplay.Show(levelIndex, levelScore);
    }

    public void ShowPoppingText(Vector3 position, string message)
    {
        UIPoppingText poppingText = Instantiate(poppingTextPrefab, poppingTextContainer);
        poppingText.Show(position, message);
    }

    public void OpenCurtains(AfterAnimationCallback afterAnimation, bool hideScore = true)
    {
        SoundManager.main.PlaySound(GameSoundType.CurtainsOpen);
        if (hideScore)
        {
            scoreDisplay.Hide();
        }
        //MusicPlayer.main.LevelFadeIn();
        afterOpenCallback = afterAnimation;
        animator.SetTrigger("Open");
        HideStart();
    }
    public void CloseCurtains(AfterAnimationCallback afterAnimation)
    {
        SoundManager.main.PlaySound(GameSoundType.CurtainsClose);
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

[System.Serializable]
public class LevelScore
{
    public int ScoreGained;
    public string LevelTime;
    public string TotalTime;
    public int BonusFromTime;
    public int Deaths;
    public int TotalDeaths;
    public int TotalScore;
}

public delegate void AfterAnimationCallback();