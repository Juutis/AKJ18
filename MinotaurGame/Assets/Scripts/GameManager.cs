using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;
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
    int lives = 5;
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

    void Start()
    {
        UIManager.main.OpenCurtains(delegate
        {
            OpenLevel();
            timer = new Timer();
            UITimer.main.timer = timer;
        });
    }

    public void PlayerDie()
    {
        if (playerDead) return;
        playerDead = true;
        PlayerCharacter.main.Die();
        BulletTime.Main.Trigger();
        lives -= 1;
        scoreMultiplier = 1;
        if (lives < 1)
        {
            Debug.Log("Game over");
        }
        else
        {
            Invoke("RespawnPlayer", 1.0f);
        }
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

    public void OpenNextLevel()
    {
        Time.timeScale = 0f;
        timer.Pause();
        currentLevelIndex += 1;
        scoreMultiplier = 1;

        int levelScore = currentScore - previousLevelsScore;
        double currentTime = timer.GetTime();
        double levelTime = currentTime - timeBeforeCurrentLevel;
        double timeMinTargetInMS = levelTimeMinTarget * 1000;
        double timeMaxTargetInMS = levelTimeMaxTarget * 1000;
        int timeBonus = 0;
        Debug.Log($"Level time is {levelTime} maxTarget is {timeMaxTargetInMS}");

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
            Debug.Log("The end!");
        }
        else
        {
            UIManager.main.CloseCurtains(delegate
            {
                if (currentLevel != null)
                {
                    currentLevel.Kill();
                }
                OpenLevel();
                UIManager.main.OpenCurtains(delegate
                {
                    timer.Unpause();
                    Time.timeScale = 1f;

                    previousLevelsScore = currentScore;
                    timeBeforeCurrentLevel = currentTime;
                });
            }
            );
        }
    }

    public void PickupItem(Item item)
    {
        if (item.Type == ItemType.Axe)
        {
            ammoCount += 1;
            UIAmmoHUD.main.AddAmmo();
        }
        else
        {
            if (item.Type == ItemType.Thread)
            {
                threadCount += 1;
                if (threadCount >= currentLevelThreadCount)
                {
                    OpenDoor();
                }
            }
            if (item.Type == ItemType.Bonus)
            {
                scoreMultiplier += 1;
            }

            int scoreGained = itemScore * scoreMultiplier;
            UIScore.main.AddScore(scoreGained);
            currentScore += scoreGained;
            Debug.Log($"Score: {currentScore}");
        }
    }

    public void ScoreKill(int count)
    {
        int scoreGained = itemScore * scoreMultiplier * killComboMultiplier(count);
        UIScore.main.AddScore(scoreGained);
        currentScore += scoreGained;
        Debug.Log($"Score: {currentScore}");
    }

    private int killComboMultiplier(int count)
    {
        return (int)Mathf.Pow(2, count - 1);
    }

}

[Serializable]
public class ObjectToPrefab
{
    public Tile Tile;
    public GameObject Prefab;

    public LevelObjectType LevelObjectType;
}
