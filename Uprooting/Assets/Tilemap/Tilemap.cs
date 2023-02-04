using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Tilemap : MonoBehaviour
{
    [SerializeField]
    private Tile dirtTilePrefab;

    [Header("Tilemap Generation Settings")]

    [SerializeField]
    private int dirtRows = 30;

    [SerializeField]
    private int dirtColumns = 60;

    private readonly Dictionary<(int x, int y), Tile> tiles = new();

    public void Start()
    {
        // Generate Tilemap
        Debug.Assert(dirtRows >= 0);
        Debug.Assert(dirtColumns >= 0);
        for (int y = -1; y >= -dirtRows; --y)
        {
            for (int x = 0; x < dirtColumns; ++x)
            {
                AddTile(x, y, dirtTilePrefab);
            }
        }
    }

    public bool AddTile(int x, int y, Tile tilePrefab) => AddTile((x, y), tilePrefab);
    public bool AddTile((int x, int y) location, Tile tilePrefab)
    {
        if (tiles.ContainsKey(location))
        {
            return false;
        }

        var position = new Vector3(location.x, location.y, tilePrefab.transform.position.z);
        var tile = Instantiate(tilePrefab, position, tilePrefab.transform.rotation, transform);
        tile.Tilemap = this;

        tiles.Add(location, tile);
        return true;
    }

    public Tile RemoveTile(int x, int y) => RemoveTile((x, y));
    public Tile RemoveTile((int x, int y) location)
    {
        if (tiles.TryGetValue(location, out var tile))
        {
            tiles.Remove(location);
            Destroy(tile.gameObject);
            return tile;
        }
        else
        {
            return null;
        }
    }
}
