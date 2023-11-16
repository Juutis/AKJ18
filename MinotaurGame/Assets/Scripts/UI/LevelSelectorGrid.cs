using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSelectorGrid : MonoBehaviour
{
    [SerializeField]
    private LevelSelectorLevel prefab;

    private static int levelCount = 17;
    public LevelSelectorLevel[] Levels;

    // Start is called before the first frame update
    void Start()
    {
        Levels = new LevelSelectorLevel[levelCount];
        for (var i = 0; i < levelCount; i++) {
            var tile = Instantiate(prefab, transform);
            tile.Init(i, levelCount, this);
            Levels[i] = tile;
        }
    }
}
