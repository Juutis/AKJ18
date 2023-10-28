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
    private const int itemScore = 10;
    private const int killScore = 10;
    private float totalTime = 0f;
    private float levelTime = 0f;

    Timer timer;

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
        PlayerCharacter.main.Die();
        lives -= 1;
        if (lives < 1)
        {
            Debug.Log("Game over");
        }
        else
        {
            RespawnPlayer();
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
