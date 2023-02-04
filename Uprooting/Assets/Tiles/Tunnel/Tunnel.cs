using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tile))]
public class Tunnel : MonoBehaviour
{
    private Tile tile;

    public void Start() => tile = GetComponent<Tile>();

    public void FillWithDirt()
    {
        var success = tile.Tilemap.RemoveTile(tile.Location) != null;
        success &= tile.Tilemap.AddTile(tile.Location, tile.Tilemap.DirtTilePrefab);
        Debug.Assert(success, "filling tunnel with dirt failed unexpectedly");
    }
}
