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

    public Tile this[int x, int y] => this[(x, y)];
    public Tile this[(int x, int y) location] => TryGetTile(location, out var tile) ? tile : null;
    public bool TryGetTile(int x, int y, out Tile tile) => TryGetTile((x, y), out tile);
    public bool TryGetTile((int x, int y) location, out Tile tile) => tiles.TryGetValue(location, out tile);

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

    /// <summary>Searches through the maze to figure out if the given location has access to air.</summary>
    public bool HasAir(int x, int y)
    {
        // Level that always has access to air everywhere. And all levels above also always have access to air.
        const int AirLevel = 1;

        if (y >= AirLevel) return true;
        if (TryGetTile(x, y, out var originTile) && !originTile.AllowsAirflow) return false;

        var origin = (x, y);
        var visited = new HashSet<(int x, int y)>();
        var bfs = new Queue<(int x, int y)>();
        visited.Add(origin);
        bfs.Enqueue(origin);

        (int x, int y)[] airMoves = { (-1, 0), (1, 0), (0, -1), (0, 1) };

        while (bfs.Count > 0)
        {
            var current = bfs.Dequeue();
            if (current.y >= AirLevel)
            {
                return true;
            }

            foreach (var move in airMoves)
            {
                var newLocation = (x: current.x + move.x, y: current.y + move.y);
                if (visited.Contains(newLocation))
                {
                    continue;
                }

                if (TryGetTile(newLocation, out var nextTile) && !nextTile.AllowsAirflow)
                {
                    continue;
                }

                visited.Add(newLocation);
                bfs.Enqueue(newLocation);
            }
        }

        return false;
    }
}
