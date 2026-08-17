using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGen : MonoBehaviour
{
    //Dungeon size and room size
    //Larger dungeon + smaller rooms = more rooms
    [SerializeField]
    private int minRoomWidth = 20, minRoomHeight = 20; //Determines minimum size of rooms
    [SerializeField]
    private int dungeonWidth = 40, dungeonHeight = 40; //Determines size of whole dungeon

    [SerializeField]
    [Range(0, 10)]
    private int offset = 1; //Space between rooms and edges of dungeon in cell count

    [SerializeField]
    private Vector2Int startPos = Vector2Int.zero; //Where generation starts from

    [SerializeField]
    public Tilemap floorTilemap, wallTilemap;

    [SerializeField]
    public TileBase floorTile, wallTile;

    [SerializeField]
    public int enemyCount; //Number of enemies to spawn

    [SerializeField]
    public GameObject enemy;

    int meleeAttacks = PlayerData.meleeAttacks;
    int rangedAttacks = PlayerData.rangedAttacks;

    TextMeshProUGUI parametersTxt;
    void Start()
    {
        int temp;
        dungeonWidth += (int)(PlayerData.totalDistance / 20);
        dungeonHeight += (int)(PlayerData.totalDistance / 20);

        if (meleeAttacks >= rangedAttacks)
        {
            temp = meleeAttacks / 20;

            minRoomWidth += temp;
            minRoomHeight += temp;
        } else
        {
            temp = rangedAttacks / 12;
            if (temp > 10)
            {
                temp = 10;
            }
            minRoomWidth -= temp;
            minRoomHeight -= temp;
        }

        if (PlayerData.timeToCompleteLevel < 120)
        {
            enemyCount = (int)((120 + PlayerData.timeToCompleteLevel) / 10);
        }
        else
        {
            enemyCount = (int)(24 + (PlayerData.timeToCompleteLevel / 100));
        }
        CreateRooms();
        ShowParameterView();
    }

    public void CreateRooms()
    {
        var roomsList = BSPAlgorithm.BSPGeneration(new BoundsInt((Vector3Int)startPos, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight); //List of binary split rooms adhering to dungeon and room parameters
        //foreach (var rooms in roomsList) //Debugging room positions
        //{
            //Debug.Log(rooms);
        //}
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        floor = CreateSimpleRooms(roomsList); 

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList) //Adds center of each room to list
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corrs = ConnectRooms(roomCenters);
        floor.UnionWith(corrs); //Adds corridor vectors to floor list

        AddFloorTiles(floor);
        CreateWalls(floor);
        spawnEnemies(floor);
    }

    public HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corrs = new HashSet<Vector2Int>(); //HashSet list to store corridor positions
        var currRoomCenter = roomCenters[UnityEngine.Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPoint(currRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorr = CreateCorrs(currRoomCenter, closest);
            currRoomCenter = closest;
            corrs.UnionWith(newCorr);
        }
        return corrs;

    }

    private HashSet<Vector2Int> CreateCorrs(Vector2Int currRoomCenter, Vector2Int destinationPoint)
    {
        HashSet<Vector2Int> corr = new HashSet<Vector2Int>();
        var pos = currRoomCenter;
        corr.Add(pos);
        while (pos.y != destinationPoint.y) //Adds vector of next tile, moving towards destination point, by checking whether each dimension of the current position vector is on the same axis as the destination point vector
        {
            if (destinationPoint.y > pos.y) 
            {
                pos += Vector2Int.up;
            } 
            else if (destinationPoint.y < pos.y)
            {
                pos += Vector2Int.down;
            }
            corr.Add(pos);
        }
        while (pos.x != destinationPoint.x)
        {
            if (destinationPoint.x > pos.x)
            {
                pos += Vector2Int.right;
            } 
            else if (destinationPoint.x < pos.x)
            {
                pos += Vector2Int.left;
            }
            corr.Add(pos);
        }
        return corr;
    }

    private Vector2Int FindClosestPoint(Vector2Int currRoomCenter, List<Vector2Int> roomCenters) //Finds closest room center to current room center to join with corridor
    {
        Vector2Int closestPoint = Vector2Int.zero;
        float dist = float.MaxValue;
        foreach (var pos in roomCenters)
        {
            float currDist = Vector2.Distance(pos, currRoomCenter);
            if (currDist < dist) 
            {
                dist = currDist;
                closestPoint = pos;
            }
        }
        return closestPoint;
    }

    public HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList) //Creates rectangular grid over rooms, can be adapted to create irregular shapes.
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)  //Splits room into grid pattern and assigns each vector position to floor list
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row =  offset; row < room.size.y - offset; row++)
                {
                    Vector2Int pos = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(pos);
                }
            }
        }
        //Debug.Log("Returning floor hashset from CreateSimpleRooms");
        return floor;
    }

    public void AddFloorTiles(IEnumerable<Vector2Int> tilePos)
    {
        AddTiles(tilePos, floorTilemap, floorTile);
        //Debug.Log("Attempting to add tiles from top level function");
    }

    public void AddTiles(IEnumerable<Vector2Int> tilePos, Tilemap tilemap, TileBase tile)
    {
        foreach (var pos in tilePos)
        {
            AddSingleTile(tilemap, tile, pos);
        }
        //Debug.Log("Second level of tile placement");
    }


    public void AddSingleTile(Tilemap tilemap, TileBase tile, Vector2Int pos)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)pos);
        try
        {
            tilemap.SetTile(tilePosition, tile);
            //Debug.Log("Third level of tile placement");
        }
        catch 
        {
            //Debug.Log("Couldnt set tile");
        }
    }

    public void PaintWallTile(Vector2Int pos)
    {
        AddSingleTile(wallTilemap, wallTile, pos);
    }

    public void CreateWalls(HashSet<Vector2Int> floorPositions)
    {
        var WallPos = FindWallDirection(floorPositions, Direction2D.directionList);
        foreach (var pos in WallPos)
        {
            PaintWallTile(pos);
        }
    }

    private HashSet<Vector2Int> FindWallDirection(HashSet<Vector2Int> floorPositions, List<Vector2Int> dirList)
    {
        HashSet<Vector2Int> Walls = new HashSet<Vector2Int>();
        foreach (var pos in floorPositions)
        {
            foreach(var dir in dirList)
            {
                var neighbourPos = pos + dir;
                if (floorPositions.Contains(neighbourPos) == false)
                {
                    Walls.Add(neighbourPos); //If neighbouring tile isn't a floor tile, wall tile is placed.
                }
            }
        }
        return Walls;
    }

    public static class Direction2D
    {
        public static List<Vector2Int> directionList = new List<Vector2Int> {
        new Vector2Int(0,1), //Up
        new Vector2Int(1,0), //Right
        new Vector2Int(0,-1), //Down
        new Vector2Int(-1,0) //Left
        };
        
        public static Vector2Int GetRndDireciton() //Generates random direction for random corridor generation
        {
            return directionList[UnityEngine.Random.Range(0, directionList.Count)];
        }

    }

    public void spawnEnemies(IEnumerable<Vector2Int> floor)
    {
        int spawnedEnemiesCount = 0;
        List<Vector2Int> spawnedOnTiles = new List<Vector2Int>();

         foreach (var tile in floor)
         {
             float randValue = UnityEngine.Random.Range(0, 99);
             if (randValue < 10 && spawnedOnTiles.Contains(tile) == false)
             {
                    spawnedOnTiles.Add(tile);
                    spawnSingleEnemy(tile);
                    spawnedEnemiesCount++;                    
             }
             if (spawnedEnemiesCount >= enemyCount)
             {
                    break;
             }
         } 
    }

    private void spawnSingleEnemy(Vector2Int tile)
    {
        GameObject enemy1 = Instantiate(enemy);
        enemy1.transform.position = (Vector3Int)tile;
    }

    void ShowParameterView() //Used for displaying final parameter values when viewing level
    {
        GameObject txt = GameObject.Find("ParameterView");
        parametersTxt = txt.GetComponent<TextMeshProUGUI>();
        parametersTxt.text = "Time Taken: " + PlayerData.timeToCompleteLevel.ToString() + Environment.NewLine + "Melee Attacks: " + PlayerData.meleeAttacks.ToString() + Environment.NewLine + "Ranged Attacks: " + PlayerData.rangedAttacks.ToString() + Environment.NewLine + "Distance Travelled: " + PlayerData.totalDistance.ToString();
    }
}