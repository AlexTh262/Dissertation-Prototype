using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using RangeAttribute = UnityEngine.RangeAttribute;
using System.Collections.Generic;
using TMPro;

public class TileMapGen : MonoBehaviour
{
    public int width;
    public int height;

    public string seed; //Used to generate random caves, calculated using current system time ticks
    public bool useRandSeed; //Option for future implementation of storing seeds and being able to re-generate the levels they create

    [Range(0, 100)]
    public int fillPercent = 45;

    int meleeAttacks = PlayerData.meleeAttacks;
    int rangedAttacks = PlayerData.rangedAttacks;
    int iterations = 8;

    int[,] level; //Array of 1s and 0s which corresponds to wall and floor tiles respectively

    [SerializeField] TileBase ground;
    [SerializeField] Tilemap currTilemap; //Tilemap for floor
    [SerializeField] TileBase wall;
    [SerializeField] Tilemap WallMap; //Separate tilemap for walls
    [SerializeField] GameObject enemy;
    [SerializeField] public int enemyCount = 10;

    public List<Vector3Int> spawnableTiles = new List<Vector3Int>();
    public int spawnedEnemyCount = 0;

    TextMeshProUGUI parametersTxt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerData.totalDistance > 1000) //Ensuring cave will never be more than 100x100
        {
            width = 100;
            height = 100;
        } else
        {
            width = (int)(PlayerData.totalDistance / 10);
            height = (int)(PlayerData.totalDistance / 10);
        }

        if (PlayerData.timeToCompleteLevel < 120)
        {
            enemyCount = (int)((120 + PlayerData.timeToCompleteLevel) / 10);
        } else
        {
            enemyCount = (int)(24 + (PlayerData.timeToCompleteLevel / 100));
        }

        int tempAddToFillPercent;
        int tempAddToIterations;
        if (meleeAttacks >= rangedAttacks)
        {
            tempAddToFillPercent = meleeAttacks / 37;
            tempAddToIterations = meleeAttacks / 25;
            if (tempAddToFillPercent > 3)
            {
                fillPercent = 42;
            }
            else
            {
                fillPercent -= tempAddToFillPercent;
            }
            if (tempAddToIterations > 5)
            {
                iterations = 13;
            }
            else
            {
                iterations += tempAddToIterations;
            }          
        } else
        {
            tempAddToFillPercent = rangedAttacks / 10;
            tempAddToIterations = rangedAttacks / 10;
            if (tempAddToFillPercent > 6)
            {
                fillPercent = 50;
            }
            else
            {
                fillPercent += tempAddToFillPercent;
            }
            if (tempAddToIterations > 7)
            {
                iterations = 2;
            }
            else
            {
                iterations -= tempAddToIterations;
            }
        }

        GenerateLevel();
        System.Random rand = new System.Random();

        //Spawn player in random floor position:
        //int r = rand.Next(spawnableTiles.Count);
        //GameObject player = GameObject.Find("Player");
        //Vector3Int pos = spawnableTiles[r];
        //spawnPlayer(pos, player);

        List<Vector3Int> spawnedOnTiles = new List<Vector3Int>();

        while (spawnedEnemyCount < enemyCount)
        {
            int r2 = rand.Next(spawnableTiles.Count);
            Vector3Int pos = spawnableTiles[r2];
            spawnEnemies(pos, spawnedOnTiles);

        }
        ShowParameterView();
    }

    void GenerateLevel()
    {

        level = new int[width, height]; 
        randomFill();

        for (int i = 0; i < iterations; i++) //Number of smoothing iterations determined with this foor loop
        {
            SmoothLevel();
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {               
                if (level[x,y] == 0) //Places floor tile on every grid postion with a value of 0 and a wall tile where the value is 1
                {
                    fillMapGround(new Vector3Int(x, y));
                    spawnableTiles.Add(new Vector3Int(x, y)); //List containing vectors of all floor tiles, which the player can be spawned on.
                } else if (level[x,y] == 1)
                {
                    fillMapWall(new Vector3Int(x, y));
                }
            }
        }      
    }

    void randomFill()
    {
        if (useRandSeed == true)
        {
            seed = System.DateTime.Now.Ticks.ToString();
        }

        System.Random RandGEN = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) //Ensures all edge tiles are wall tiles.
                {
                    level[x, y] = 1;
                }

                level[x, y] = (RandGEN.Next(0, 100) < fillPercent) ? 1 : 0; //FillPercent acts as a chance, essentially detrmining howm likely it is for a tile to be a wall
            }
        }
    }


    void SmoothLevel()
    {
        int[,] tempLevel; //Temp array to avoid bias to one side while smoothing level

        for (int x = 0; x < width; x++)
        {

            for (int y = 0; y < height; y++)
            {
                tempLevel = level;

                int neighbourWalls = GetWallCount(x, y);
                if (neighbourWalls > 4)
                {
                    tempLevel[x, y] = 1;
                }
                else if (neighbourWalls < 4)
                {
                    tempLevel[x, y] = 0;
                }
                else if (x == 0 || x == width - 1 || y == height - 1 || y == 0)
                {
                    tempLevel[x, y] = 1;
                }

                level = tempLevel;
            }
        }
    }

    int GetWallCount(int x, int y)
    {
        int WallCount = 0;
        for (int neighbourx = x - 1; neighbourx <= x + 1; neighbourx++)
        {
            for (int neighboury = y - 1; neighboury <= y + 1; neighboury++)
            {
                if (neighbourx >= 0 && neighbourx < width && neighboury >= 0 && neighboury < height)
                {
                    if (neighbourx != x || neighboury != y)
                    {
                        WallCount += level[neighbourx, neighboury];
                    }
                }
                else
                {
                    WallCount++;
                }
            }
        }
        return WallCount;
    }



    void fillMapGround(Vector3Int pos)
    {
        currTilemap.SetTile(pos, ground);
    }

    void fillMapWall(Vector3Int pos)
    {
        WallMap.SetTile(pos, wall);
        
    }

    //void spawnPlayer(Vector3Int pos, GameObject player) //Used to spawn player on random tile position passed through from Start method
    //{   
        //player.transform.position = pos;
    //}

    void spawnEnemies(Vector3Int pos, List<Vector3Int> spawnedOnTiles)
    {
        if (spawnedOnTiles.Contains(pos) == false)
        {
            GameObject enemy1 = Instantiate(enemy);
            enemy1.transform.position = pos;
            spawnedOnTiles.Add(pos); //Tracks which tiles have had an enemy spawned on them to avoid spawning more than one enemy on the same tile
            //Debug.Log(pos);
            spawnedEnemyCount++;
        }
    }

    void ShowParameterView() //If room is two times the minimum size on the y axis or more, it is split horizontally
    {
        GameObject txt = GameObject.Find("ParameterView");
        parametersTxt = txt.GetComponent<TextMeshProUGUI>();
        parametersTxt.text = "Time Taken: " + PlayerData.timeToCompleteLevel.ToString() + Environment.NewLine + "Melee Attacks: " + PlayerData.meleeAttacks.ToString() + Environment.NewLine + "Ranged Attacks: " + PlayerData.rangedAttacks.ToString() + Environment.NewLine + "Distance Travelled: " + PlayerData.totalDistance.ToString();
    }
}