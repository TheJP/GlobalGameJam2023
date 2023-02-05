using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TurnBasedSystem;
using UnityEngine;

public class Tilemap : MonoBehaviour
{
    [field: Header("Prefabs")]

    [field: SerializeField]
    public Tile DirtTilePrefab { get; private set; }

    [field: SerializeField]
    public Tile TunnelTilePrefab { get; private set; }
    
    [field: SerializeField]
    public GrowablePlant CarrotPrefab { get; private set; }

    [field: Header("Background")]

    [field: SerializeField]
    public SpriteRenderer BackgroundTunnelSpriteRenderer { get; private set; }

    [Header("Tilemap Generation Settings")]

    [SerializeField]
    private int dirtRows = 30;

    [SerializeField]
    private int dirtColumns = 60;
    
    public int Width => dirtColumns;
    public int Height => dirtRows;

    public event Action<(int x, int y)> TileChanged;

    private readonly Dictionary<(int x, int y), Tile> tiles = new();

    public static Tilemap Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("Multiple Tilemaps detected. Destroying duplicate.");
            return;
        }
        Instance = this;
    }

    public void Start()
    {
        var mole = FindObjectOfType<Mole>();
        var moleLocation = (x: (int)mole.transform.position.x, y: (int)mole.transform.position.y);

        // Generate Tilemap
        Debug.Assert(dirtRows >= 0);
        Debug.Assert(dirtColumns >= 0);
        for (int y = 0; y >= -dirtRows; --y)
        {
            for (int x = 0; x < dirtColumns; ++x)
            {
                var nearMole = (moleLocation.y - 1 <= y && y <= moleLocation.y + 1) &&
                    (moleLocation.x - 1 <= x && x <= moleLocation.x + 1);
                AddTile(x, y, nearMole ? TunnelTilePrefab : DirtTilePrefab);
            }
        }

        var airTunnelX = moleLocation.x - 2;
        for (int airTunnelY = moleLocation.y + 1; airTunnelY <= 0; ++airTunnelY)
        {
            ReplaceTile(airTunnelX, airTunnelY, TunnelTilePrefab);
        }

        foreach (var tile in tiles.Values)
        {
            tile.UpdateWalkable();
        }
    }

    public Tile this[int x, int y] => this[(x, y)];
    public Tile this[(int x, int y) location] => TryGetTile(location, out var tile) ? tile : null;
    public bool TryGetTile(int x, int y, out Tile tile) => TryGetTile((x, y), out tile);
    public bool TryGetTile((int x, int y) location, out Tile tile) => tiles.TryGetValue(location, out tile);

    private bool AddTile(int x, int y, Tile tilePrefab) => AddTile((x, y), tilePrefab);
    private bool AddTile((int x, int y) location, Tile tilePrefab)
    {
        if (tiles.ContainsKey(location))
        {
            return false;
        }

        var position = new Vector3(location.x, location.y, tilePrefab.transform.position.z);
        var tile = Instantiate(tilePrefab, position, tilePrefab.transform.rotation, transform);
        tile.Tilemap = this;
        tile.Location = location;
        tile.TileChanged += OnTileChangedInternally;

        tiles.Add(location, tile);
        InvokeTileChangedNextFrame(location);
        return true;
    }

    public Tile ReplaceTile(int x, int y, Tile tilePrefab) => ReplaceTile((x, y), tilePrefab);
    /// <summary>
    /// Replace the tile at the given location with the given tile prefab.
    /// </summary>
    /// <returns>The newly created Tile.</returns>
    public Tile ReplaceTile((int x, int y) location, Tile tilePrefab)
    {
        if (tiles.TryGetValue(location, out var tile))
        {
            tiles.Remove(location);
            tile.TileChanged -= OnTileChangedInternally; // avoid false positive event invocations
            Destroy(tile.gameObject);

            var success = AddTile(location, tilePrefab);
            Debug.Assert(success);
            this[location]?.UpdateWalkable();
            this[location.x, location.y + 1]?.UpdateWalkable();

            InvokeTileChangedNextFrame(location);
            return this[location.x, location.y];
        }
        else
        {
            return null;
        }
    }

    public bool HasOxygen((int x, int y) location) => HasOxygen(location.x, location.y);
    /// <summary>Searches through the maze to figure out if the given location has access to air.</summary>
    public bool HasOxygen(int x, int y)
    {
        // Level that always has access to air everywhere. And all levels above also always have access to air.
        const int AirLevel = 0;

        if (y >= AirLevel) return true;
        if (!TryGetTile(x, y, out var originTile) || !originTile.AllowsAirflow) return false;

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

                if (!TryGetTile(newLocation, out var nextTile) || !nextTile.AllowsAirflow)
                {
                    continue;
                }

                visited.Add(newLocation);
                bfs.Enqueue(newLocation);
            }
        }

        return false;
    }

    private void OnTileChangedInternally(Tile tile) => TileChanged?.Invoke(tile.Location);

    private void InvokeTileChangedNextFrame((int x, int y) location) => StartCoroutine(InvokeTileCoroutine(location));

    private IEnumerator InvokeTileCoroutine((int x, int y) location)
    {
        yield return null;
        TileChanged?.Invoke(location);
    }

    /// <summary>
    /// Returns the movement type required to move from the current tile to the next tile.
    /// Returns false if the movement is not possible or the tiles are not next to each other.
    /// </summary>
    /// <param name="currTile">The start Tile of the movement</param>
    /// <param name="nextTile">The end Tile of the movement</param>
    /// <param name="movementType">The type of the movement that's needed between these Tile, or None, if movement not possible</param>
    /// <returns>Returns true if the movement is possible, false if the movement is not possible or the tiles are not next to each other.</returns>
    public bool TryGetMovementType(Tile currTile, Tile nextTile, out MovementType movementType)
    {
        if (currTile == null || nextTile == null)
        {
            movementType = MovementType.None;
            return false;
        }

        if (!AreTilesNeighbour(currTile, nextTile))
        {
            movementType = MovementType.None;
            return false;
        }

        if (nextTile.IsWalkable && currTile.IsWalkable)
        {
            movementType = MovementType.Walk;
        }
        else if (currTile.IsClimbable && nextTile.IsClimbable)
        {
            movementType = MovementType.EasyClimb;
        }
        else if (!nextTile.IsSolid)
        {
            movementType = MovementType.Climb;
        }
        else if (nextTile.IsDiggable)
        {
            movementType = MovementType.DigMovement;
        }
        else
        {
            movementType = MovementType.None;
        }
        return movementType != MovementType.None;
    }

    public bool AreTilesNeighbour(Tile currTile, Tile nextTile)
    {

        if (currTile.Location.x == nextTile.Location.x)
        {
            return Mathf.Abs(currTile.Location.y - nextTile.Location.y) == 1;
        }
        if (currTile.Location.y == nextTile.Location.y)
        {
            return Mathf.Abs(currTile.Location.x - nextTile.Location.x) == 1;
        }
        return false;

    }
    
    public List<Tile> GetGrowableTiles() {
        return tiles.Values.Where(tile => tile.IsGrowable).ToList();
    }

}