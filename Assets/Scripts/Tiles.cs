using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tiles : MonoBehaviour
{
    public List<TileBase> tileList = new List<TileBase>();

    public bool isLoaded = false;

    // Start is called before the first frame update
    void Awake()
    {
        tileList = GetTiles();

        isLoaded = true;
    }

    // Load all the tile assets. Done in resources to allow for built versions.
    private List<TileBase> GetTiles()
    {
        Object[] tiles = Resources.LoadAll("Tiles", typeof(TileBase));

        List<TileBase> tileList = new List<TileBase>();

        foreach(TileBase t in tiles)
        {
            tileList.Add(t);
        }

        return tileList;
    }
}
