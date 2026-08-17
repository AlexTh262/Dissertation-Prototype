using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGeneration : MonoBehaviour
{

    Tilemap[,] tilemap;
    public Tile tile;
    public int width;
    public int height;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tilemap = new Tilemap[width, height];

        for (int x  = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tilemap[x,y].SetTile(new Vector3Int(-x + width / 2, -y + height / 2), tile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
