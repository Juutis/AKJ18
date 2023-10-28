using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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

    int threadCount = 0;
    private Level currentLevel;
    private int currentLevelThreadCount;

    void Start()
    {
        OpenLevel();
    }

    public void OpenLevel()
    {
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
    }

}

[Serializable]
public class ObjectToPrefab
{
    public Tile Tile;
    public GameObject Prefab;

    public LevelObjectType LevelObjectType;
}
