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
    private int killComboCounter = 0;
    private int scoreMultiplier = 1;
    private const int itemScore = 10;
    private float totalTime = 0f;
    private float levelTime = 0f;

    Timer timer;

    void Start()
    {
        OpenLevel();
        timer = new Timer();
        UITimer.main.timer = timer;
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
        if (currentLevel != null)
        {
            currentLevel.Kill();
        }
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
        currentLevelIndex += 1;
        if (currentLevelIndex >= levels.Count)
        {
            Debug.Log("The end!");
        }
        else
        {
            OpenLevel();
        }
    }

    public void PickupItem(Item item)
    {
        if (item.Type == ItemType.Thread)
        {
            threadCount += 1;
            if (threadCount >= currentLevelThreadCount)
            {
                OpenDoor();
            }
        }
        if (item.Type == ItemType.Axe)
        {
            ammoCount += 1;
            UIAmmoHUD.main.AddAmmo();
        }
        if(item.Type == ItemType.Bonus)
        {
            scoreMultiplier += 1;
        }

        int scoreGained = itemScore * scoreMultiplier;
        UIScore.main.AddScore(scoreGained);
        currentScore += scoreGained;
        Debug.Log($"Score: {currentScore}");
    }

}

[Serializable]
public class ObjectToPrefab
{
    public Tile Tile;
    public GameObject Prefab;

    public LevelObjectType LevelObjectType;
}
