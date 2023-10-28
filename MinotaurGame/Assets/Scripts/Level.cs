using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    [SerializeField]
    private Tilemap objectTilemap;

    private Door door;

    private int threadCount = 0;


    public int ThreadCount { get { return threadCount; } }


    public void Init()
    {
        TilemapRenderer mapRenderer = objectTilemap.GetComponent<TilemapRenderer>();
        mapRenderer.enabled = false;

        for (int n = objectTilemap.cellBounds.xMin; n < objectTilemap.cellBounds.xMax; n++) // scan from left to right for tiles
        {
            for (int p = objectTilemap.cellBounds.yMin; p < objectTilemap.cellBounds.yMax; p++) // scan from bottom to top for tiles
            {
                Vector3Int localPlace = new Vector3Int(n, p, (int)objectTilemap.transform.position.y); // if you find a tile, record its position on the tile map grid
                Vector3 place = objectTilemap.CellToWorld(localPlace); // convert this tile map grid coords to local space coords
                if (objectTilemap.HasTile(localPlace))
                {
                    Tile tile = objectTilemap.GetTile<Tile>(localPlace);
                    ObjectToPrefab spawnObject = GameManager.main.GetObjectByTile(tile);
                    place.y += 0.5f;
                    GameObject createdObject = Instantiate(spawnObject.Prefab, place, Quaternion.identity, transform);
                    switch (spawnObject.LevelObjectType)
                    {
                        case LevelObjectType.Door:
                            Door doorObject = createdObject.GetComponent<Door>();
                            door = doorObject;
                            break;
                        case LevelObjectType.Player:
                            PlayerCharacter player = createdObject.GetComponent<PlayerCharacter>();
                            Debug.Log("player!");
                            break;
                        case LevelObjectType.WalkingEnemy:
                            MovingEnemy walkingEnemy = createdObject.GetComponent<MovingEnemy>();
                            Debug.Log("walkingEnemy!");
                            break;
                        case LevelObjectType.Axe:
                            Debug.Log("Spawned axe!");
                            break;
                        case LevelObjectType.Thread:
                            Debug.Log("Spawned thread!");
                            threadCount += 1;
                            break;
                    }
                }
            }
        }
    }

    public void SpawnObject()
    {

    }

    public void Kill()
    {
        Destroy(gameObject);
    }


    public void OpenDoor()
    {
        door.Open();
    }

}


public enum LevelObjectType
{
    Door,
    Player,
    Axe,
    Thread,
    WalkingEnemy
}