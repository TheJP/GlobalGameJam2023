using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private bool allowsAirflow = false;
    public bool AllowsAirflow
    {
        get => allowsAirflow;
        set
        {
            allowsAirflow = value;
            Invoke(nameof(InvokeTileChanged), 0);
        }
    }

    [SerializeField]
    private bool isSolid = true;
    public bool IsSolid
    {
        get => isSolid;
        set
        {
            isSolid = value;
            Invoke(nameof(InvokeTileChanged), 0);
        }
    }
    
    public bool IsDiggable => GetComponent<Dirt>() != null;
    
    public Tile DigTunnel()
    {
        if (!IsDiggable) {
            Debug.LogError("Should not Dig into undiggable tile!");
            return null;
        }
        var newTile = Tilemap.ReplaceTile(Location, Tilemap.TunnelTilePrefab);
        Debug.Assert(newTile != null, "tunnel digging failed unexpectedly");
        return newTile;
    }

    [SerializeField]
    private bool isClimbable = false;
    public bool IsClimbable
    {
        get => isClimbable;
        set
        {
            isClimbable = value;
            Invoke(nameof(InvokeTileChanged), 0);
        }
    }

    private bool isWalkable = false;
    /// <summary>A tile is walkable if it is not solid, but the tile underneath it is.</summary>
    public bool IsWalkable
    {
        get => isWalkable;
        set
        {
            isWalkable = value;
            Invoke(nameof(InvokeTileChanged), 0);
        }
    }

    public event Action<Tile> TileChanged;

    public Tilemap Tilemap { get; set; }

    public (int x, int y) Location { get; set; }
    /// <summary>
    /// The center location of the tile, in world space.
    /// </summary>
    public Vector2 CenterLocation => new Vector2(Location.x, Location.y);
    

    private void InvokeTileChanged() => TileChanged?.Invoke(this);

    private void TileAboveUpdateWalkable()
    {
        if (Tilemap.TryGetTile(Location.x, Location.y + 1, out var tileAbove))
        {
            tileAbove.UpdateWalkable();
        }
    }

    public void UpdateWalkable()
    {
        if (IsSolid)
        {
            if (IsWalkable)
            {
                IsWalkable = false;
                TileAboveUpdateWalkable();
            }
            return;
        }

        var tileBelow = Tilemap[Location.x, Location.y - 1];
        if (tileBelow == null || tileBelow.IsSolid)
        {
            if (!IsWalkable)
            {
                IsWalkable = true;
                TileAboveUpdateWalkable();
            }
        }
    }

    public bool TryGetNeighbour((int x, int y) direction, out Tile tile) {
        direction.x = Math.Max(Math.Min(direction.x, 1), -1);
        direction.y = Math.Max(Math.Min(direction.y, 1), -1);
        Tilemap.Instance.TryGetTile(Location.x + direction.x, Location.y + direction.y, out tile);
        return tile != null;
    }

    private void OnDrawGizmos() {
        if (isWalkable) {
            Gizmos.color = Color.gray;
        }
        else {
            Gizmos.color = Color.clear;
        }
        Gizmos.DrawWireCube(CenterLocation, Vector2.one * 0.5f);
    }
}
