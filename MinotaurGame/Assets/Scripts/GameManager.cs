using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager main;

    void Awake()
    {
        main = this;
    }

    [SerializeField]
    private List<ObjectToPrefab> objectsToPrefabs = new();
    public ObjectToPrefab GetObjectByTile(Tile tile)
    {
        return objectsToPrefabs.Where(otp => otp.Tile == tile).FirstOrDefault();
    }

    [SerializeField]
    private List<Level> levels;
    private int currentLevelIndex = 0;

    int ammoCount = 0;

    int threadCount = 0;
    private Level currentLevel;
    private int currentLevelThreadCount;
    int startingLives = 3;
    int lives;
    private int currentScore = 0;
    private int scoreMultiplier = 1;
    private int previousLevelsScore = 0;
    private const int itemScore = 10;
    private const int killScore = 10;
    private double timeBeforeCurrentLevel = 0;
    private double levelTimeMinTarget = 20; // config?
    private double levelTimeMaxTarget = 40;
    private double maxLevelTimeBonus = 500;

    Timer timer;

    private bool playerDead = false;

    private bool waitForInput = false;
    private AfterWaitCallback afterWaitForInput;

    private int deathsThisLevel = 0;
    private int totalDeaths = 0;

    [SerializeField]
    private GameObject LevelSelector;

    private bool levelScoreActive;

    public int LastLevelIndex = 0;
    public int NextLevelIndex = 0;
    private bool endIsHere = false;
    private bool canRestart = false;
    private bool levelFinished = false;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (endIsHere) 
        {
            canRestart = false;
            UIManager.main.HideLevelScore();
            if (Input.anyKeyDown)
            {
                UIManager.main.HideEnd();
                ShowLevelSelector();
                endIsHere = false;
                ResumeTime();
            }
        }
        if (waitForInput)
        {
            canRestart = false;
            if (Input.anyKey)
            {
                waitForInput = false;
                if (afterWaitForInput != null)
                {
                    afterWaitForInput();
                    afterWaitForInput = null;
                }
            }
        }
        if (levelScoreActive)
        {
            canRestart = false;
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ShowLevelSelector();
                levelScoreActive = false;
                UIManager.main.HideLevelScore();
                ResumeTime();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                levelScoreActive = false;
                UIManager.main.HideLevelScore();
                currentLevelIndex--;
                if (afterWaitForInput != null)
                {
                    afterWaitForInput();
                    afterWaitForInput = null;
                }
                ResetLives();
            }
            else if (Input.anyKeyDown)
            {
                if (afterWaitForInput != null)
                {
                    afterWaitForInput();
                    afterWaitForInput = null;
                }
                levelScoreActive = false;
            }
        }
        if (canRestart) {
            if (Input.GetKeyDown(KeyCode.R))
            {
                CancelInvoke();
                canRestart = false;
                FreezeTime();
                UIManager.main.CloseCurtains(delegate
                {
                    ResumeTime();
                    timer = new Timer();
                    UITimer.main.timer = timer;
                    timer.Pause();
                    if (currentLevel != null)
                    {
                        currentLevel.Kill();
                    }
                    
                    LoadLevel(currentLevelIndex);
                });
            }
        }
    }

    private void ShowLevelSelector() {
        if (AllLevelsDone()) {
            UIManager.main.SetTotalTime(GetTotalTime());
        } else {
            UIManager.main.UnsetTotalTime();
        }
        LevelSelector.SetActive(true);
    }

    void Init(bool resetTimer = true)
    {
        waitForInput = true;
        afterWaitForInput = delegate
        {
            ShowLevelSelector();
            UIManager.main.HideStart();
        };
    }

    public void LoadLevel(int levelIndex) {
        currentLevelIndex = levelIndex;
        LevelSelector.SetActive(false);
        FreezeTime();
        Debug.Log("init clled");
        Debug.Log("We are HERE!");
        ResetLives();
        OpenLevel();
        UIManager.main.OpenCurtains(delegate
        {
            ResumeTime();
            timer = new Timer();
            UITimer.main.timer = timer;
            canRestart = true;
        });
        playerDead = false;
        levelFinished = false;
        CancelInvoke();
    }

    private void ResetLives()
    {
        Debug.Log($"Start with {lives}");
        lives = startingLives;
        UILifeDisplay.main.Clear();
        for (int i = lives; i > 0; i -= 1)
        {
            UILifeDisplay.main.AddLife();
        }
    }

    public void ContinueAction()
    {
        playerDead = false;
        if (currentLevel != null)
        {
            currentLevel.Kill();
        }
        LoadLevel(currentLevelIndex);
    }
    public void RestartAction()
    {
        playerDead = false;
        if (currentLevel != null)
        {
            currentLevel.Kill();
        }
        ShowLevelSelector();
    }

    public void PlayerDie()
    {
        if (playerDead || levelFinished) return;
        deathsThisLevel += 1;
        totalDeaths += 1;
        playerDead = true;
        UILifeDisplay.main.RemoveLife();
        PlayerCharacter.main.Die();
        BulletTime.Main.Trigger();
        lives -= 1;
        scoreMultiplier = 1;
        if (lives < 1)
        {
            Debug.Log("Game over");
            GameOver();
        }
        else
        {
            Invoke("RespawnPlayer", 1.0f);
        }
    }

    public void GameOver()
    {
        canRestart = false;
        timer.Pause();
        FreezeTime();
        UIManager.main.CloseCurtains(delegate
        {
            UIGameOver.main.Show();
            UIManager.main.OpenCurtains(delegate {});
        }
        );
    }

    private GameObject playerPrefab;
    private Vector3 playerPosition;
    private Transform playerParent;
    public void SetPlayerInfo(GameObject prefab, Vector3 position, Transform playerParent)
    {
        playerPrefab = prefab;
        playerPosition = position;
        this.playerParent = playerParent;
    }

    public void RespawnPlayer()
    {
        Instantiate(playerPrefab, playerPosition, Quaternion.identity, playerParent);
        playerDead = false;
    }

    public void OpenLevel()
    {
        UIAmmoHUD.main.Clear();
        deathsThisLevel = 0;
        ammoCount = 0;
        threadCount = 0;
        Level levelPrefab = levels[currentLevelIndex];

        currentLevel = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
        currentLevel.Init();
        Debug.Log(currentLevelThreadCount);
        currentLevelThreadCount = currentLevel.ThreadCount;
    }

    public bool SpendProjectile()
    {
        if (ammoCount > 0)
        {
            ammoCount -= 1;
            UIAmmoHUD.main.RemoveAmmo();
            return true;
        }
        return false;
    }

    public void OpenDoor()
    {
        currentLevel.OpenDoor();
    }

    private bool isPaused = false;
    private void ResumeTime()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Resume time");
    }

    private void FreezeTime()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("FREEZE time");
    }

    public void StopTimer() {
        canRestart = false;
        timer.Pause();
    }

    public void SetTimescale(float timeScale)
    {
        if (!isPaused)
        {
            Time.timeScale = timeScale;
        }
    }

    public void LevelFinished() {
        levelFinished = true;
    }

    public void OpenNextLevel()
    {
        canRestart = false;
        LastLevelIndex = currentLevelIndex;
        FreezeTime();
        timer.Pause();
        currentLevelIndex += 1;
        scoreMultiplier = 1;
        NextLevelIndex = currentLevelIndex;

        int levelScore = currentScore - previousLevelsScore;
        double levelTime = timer.GetTime();
        double timeMinTargetInMS = levelTimeMinTarget * 1000;
        double timeMaxTargetInMS = levelTimeMaxTarget * 1000;
        int timeBonus = 0;
        Debug.Log($"Level time is {levelTime} maxTarget is {timeMaxTargetInMS}");

        if (PlayerPrefs.HasKey(LastLevelIndex.ToString())) {
            var levelBestTime = PlayerPrefs.GetInt(LastLevelIndex.ToString());
            if (levelTime < levelBestTime) {
                PlayerPrefs.SetInt(LastLevelIndex.ToString(), (int)levelTime);
            }
        } else {
            PlayerPrefs.SetInt(LastLevelIndex.ToString(), (int)levelTime);
        }

        if (levelTime < timeMaxTargetInMS)
        {
            double timeBonusScale = timeMinTargetInMS / Math.Max(levelTime, timeMinTargetInMS);
            timeBonus = (int)Math.Round(timeBonusScale * maxLevelTimeBonus);
            currentScore += timeBonus;
            Debug.Log($"Time bonus is {timeBonus} for scale {timeBonusScale}");
            UIScore.main.AddScore(timeBonus);
        }

        if (currentLevelIndex >= levels.Count)
        {
            NextLevelIndex = levels.Count - 1;
            Debug.Log("The end!");
            UIManager.main.CloseCurtains(delegate
            {
                currentLevel.Kill();
                UIManager.main.ShowLevelScore(currentLevelIndex - 1, new LevelScore
                {
                    LevelTime = Timer.GetFormattedString(levelTime),
                    TotalTime = Timer.GetFormattedString(levelTime),
                    ScoreGained = levelScore,
                    BonusFromTime = timeBonus,
                    Deaths = deathsThisLevel,
                    TotalDeaths = totalDeaths,
                    TotalScore = currentScore
                });
                levelScoreActive = true;
                afterWaitForInput = delegate
                {
                    UIManager.main.ShowEnd();
                    endIsHere = true;
                };

            });
        }
        else
        {
            UIManager.main.CloseCurtains(delegate
            {
                timer = new Timer();
                UITimer.main.timer = timer;
                timer.Pause();
                if (currentLevel != null)
                {
                    currentLevel.Kill();
                }
                UIManager.main.ShowLevelScore(currentLevelIndex - 1, new LevelScore
                {
                    LevelTime = Timer.GetFormattedString(levelTime),
                    TotalTime = Timer.GetFormattedString(levelTime),
                    ScoreGained = levelScore,
                    BonusFromTime = timeBonus,
                    Deaths = deathsThisLevel,
                    TotalDeaths = totalDeaths,
                    TotalScore = currentScore
                });
                levelScoreActive = true;
                afterWaitForInput = delegate
                {
                    LoadLevel(currentLevelIndex);
                };
            }
            );
        }
    }

    public void PickupItem(Item item, Vector3 pickupPosition)
    {
        if (item.Type == ItemType.Axe)
        {
            ammoCount += 1;
            UIAmmoHUD.main.AddAmmo();
            SoundManager.main.PlaySound(GameSoundType.CollectAxe);
        }
        else if (item.Type == ItemType.Heart)
        {
            lives++;
            UILifeDisplay.main.AddLife();
        }
        else
        {
            if (item.Type == ItemType.Thread)
            {
                threadCount += 1;
                SoundManager.main.PlaySound(GameSoundType.Collect);
                if (threadCount >= currentLevelThreadCount)
                {
                    MusicPlayer.main.FadeOutGameMusic(delegate
                    {
                        SoundManager.main.PlaySound(GameSoundType.OpenDoor);
                        MusicPlayer.main.LevelFadeIn(3f);
                    }, 0.2f);
                    OpenDoor();
                }
            }
            if (item.Type == ItemType.Bonus)
            {
                SoundManager.main.PlaySound(GameSoundType.Collect);
                scoreMultiplier += 1;
            }

            int scoreGained = itemScore * scoreMultiplier;
            UIScore.main.AddScore(scoreGained);
            UIManager.main.ShowPoppingText(pickupPosition, $"+{scoreGained}");
            currentScore += scoreGained;
            Debug.Log($"Score: {currentScore}");
        }
    }

    public void ScoreKill(int count, Vector2 killPosition)
    {
        int scoreGained = itemScore * scoreMultiplier * killComboMultiplier(count);
        UIScore.main.AddScore(scoreGained);
        UIManager.main.ShowPoppingText(killPosition, $"+{scoreGained}");
        currentScore += scoreGained;
        Debug.Log($"Score: {currentScore}");
    }

    private int killComboMultiplier(int count)
    {
        return (int)Mathf.Pow(2, count - 1);
    }

    public bool AllLevelsDone() {
        for (var i = 0; i < levels.Count(); i++) {
            if (!PlayerPrefs.HasKey(i.ToString())) {
                return false;
            }
        }
        return true;
    }

    public TimeSpan GetTotalTime() {
        int total = 0;
        for (var i = 0; i < levels.Count(); i++) {
            var levelTime = PlayerPrefs.GetInt(i.ToString());
            Debug.Log("Level " + i + " = " + levelTime);
            total += levelTime;
        }
        Debug.Log("Total time = " + total);
        return TimeSpan.FromMilliseconds(total);
    }

}

public delegate void AfterWaitCallback();

[Serializable]
public class ObjectToPrefab
{
    public Tile Tile;
    public GameObject Prefab;

    public LevelObjectType LevelObjectType;
}