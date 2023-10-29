using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UILevelScoreDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtTime;
    [SerializeField]
    private TextMeshProUGUI txtTotalTime;
    [SerializeField]
    private TextMeshProUGUI txtScore;
    [SerializeField]
    private TextMeshProUGUI txtDeaths;
    [SerializeField]
    private TextMeshProUGUI txtTotalDeaths;
    [SerializeField]
    private TextMeshProUGUI txtBonus;
    [SerializeField]
    private TextMeshProUGUI txtLevelTitle;
    [SerializeField]
    private TextMeshProUGUI totalScore;

    [SerializeField]
    private GameObject container;

    void Start()
    {
        container.SetActive(false);
    }
    public void Show(int levelIndex, LevelScore levelScore)
    {
        Debug.Log("Showing...");
        txtLevelTitle.text = $"Level {levelIndex + 1}";
        txtTime.text = levelScore.LevelTime;
        txtTotalTime.text = levelScore.TotalTime;
        txtBonus.text = $"{levelScore.BonusFromTime}";
        txtScore.text = $"{levelScore.ScoreGained}";
        txtDeaths.text = $"{levelScore.Deaths}";
        txtTotalDeaths.text = $"{levelScore.TotalDeaths}";
        totalScore.text = $"{levelScore.TotalScore}";
        container.SetActive(true);
    }

    public void Hide()
    {
        container.SetActive(false);
    }
}
