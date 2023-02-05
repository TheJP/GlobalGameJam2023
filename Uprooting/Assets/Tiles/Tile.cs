using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public bool IsGrowable => Location.y == 0 && IsSolid; // TODO set to the layer of the ground
    public GrowablePlant currGrowablePlant = null;


    public bool IsDiggable => GetComponent<Dirt>() != null;

    public Tile DigTunnel()
    {
        if (!IsDiggable)
        {
            Debug.LogError("Should not Dig into undiggable tile!");
            return null;
        }
        var newTile = Tilemap.ReplaceTile(Location, Tilemap.TunnelTilePrefab);
        Debug.Assert(newTile != null, "tunnel digging failed unexpectedly");
        return newTile;
    }
    
    /// <summary>
    /// Fills the tunnel with dirt.
    /// </summary>
    /// The tunnel tile will be replaced with a dirt tile.
    /// <returns>The newly created tile</returns>
    public Tile FillWithDirt()
    {
        var newTile = Tilemap.ReplaceTile(Location, Tilemap.DirtTilePrefab);
        Debug.Assert(newTile != null, "filling tunnel with dirt failed unexpectedly");
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

    private void SetWalkableAndUpdate(bool value)
    {
        if (IsWalkable == value) return;

        IsWalkable = value;
        TileAboveUpdateWalkable();
    }

    public void UpdateWalkable()
    {
        if (IsSolid)
        {
            SetWalkableAndUpdate(false);
            return;
        }

        var tileBelow = Tilemap[Location.x, Location.y - 1];
        SetWalkableAndUpdate(tileBelow == null || tileBelow.IsSolid);
    }

    public bool TryGetNeighbour((int x, int y) direction, out Tile tile)
    {
        direction.x = Math.Max(Math.Min(direction.x, 1), -1);
        direction.y = Math.Max(Math.Min(direction.y, 1), -1);
        return Tilemap.Instance.TryGetTile(Location.x + direction.x, Location.y + direction.y, out tile);
    }

    private void OnDrawGizmos()
    {
        if (isWalkable)
        {
            Gizmos.color = Color.gray;
        }
        else
        {
            Gizmos.color = Color.clear;
        }
        Gizmos.DrawWireCube(CenterLocation, Vector2.one * 0.5f);
    }

    public void SpawnSeed() {
        if (!IsGrowable) {
            Debug.LogError("Should not spawn seed on non-growable tile!");
            return;
        }

        if (currGrowablePlant != null) {
            return; // there is already a plant
        }
        currGrowablePlant = Instantiate(Tilemap.CarrotPrefab, transform); // TODO here different plants can be spawned
        currGrowablePlant.transform.position = new Vector3(CenterLocation.x, CenterLocation.y, -1);
    }

    /// <summary>
    /// Does a cavein with a certain probability. (higher probability, the higher the tile (and only if it is below a ceiling))
    /// </summary>
    public bool MaybeDoCavein() {
        if (IsSolid ||
            (TryGetNeighbour((0, -1), out var lowerNeighbour) && !lowerNeighbour.IsSolid)
            || Mole.Instance.CurrentTile.Location == this.Location
            || !HasCeiling()) {
            return false;
        }

        var caveinProbability = Location.y switch {
            -1 => 0.4f,
            -2 => 0.3f,
            -3 => 0.2f,
            _ => 0.1f
        };
        if (Random.value < caveinProbability) {
            FillWithDirt();
            ScreenShaker.Instance.TriggerShake();
            return true;
        }

        return false;
    }

    public bool HasCeiling() {
        return true; // TODO
    }

    public void DigOutPlant() {
        if (currGrowablePlant == null) {
            Debug.LogError("Should not dig out plant on tile without plant!");
            return;
        }
        Destroy(currGrowablePlant.gameObject);
        currGrowablePlant.HasBeenDestroyed();
        currGrowablePlant = null;
    }
}
