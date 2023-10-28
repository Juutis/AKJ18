using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;

public class UIScore : MonoBehaviour
{
    public static UIScore main;
    void Awake()
    {
        main = this;
    }

    [SerializeField]
    private TextMeshProUGUI txtScore;

    private float scoreAnimTimer = 0f;
    private float scoreAnimDuration = 0.3f;
    private bool isAnimatingScore = false;

    int score = 0;

    int targetScore;
    int startingScore;

    void Start()
    {
        UpdateScoreDisplay();
    }

    public void UpdateScoreDisplay()
    {
        txtScore.text = $"{score}";
    }

    public void AddScore(int scoreChange)
    {
        startingScore = score;
        targetScore = score + scoreChange;
        isAnimatingScore = true;
        scoreAnimTimer = 0f;
    }

    void Update()
    {
        if (isAnimatingScore)
        {
            scoreAnimTimer += Time.deltaTime;
            score = (int)Mathf.Lerp(startingScore, targetScore, scoreAnimTimer / scoreAnimDuration);
            if (scoreAnimTimer >= scoreAnimDuration)
            {
                isAnimatingScore = false;
                score = targetScore;
            }
            UpdateScoreDisplay();
        }
    }
}