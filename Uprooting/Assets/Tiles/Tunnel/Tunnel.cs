using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tile))]
public class Tunnel : MonoBehaviour
{
    private Tile tile;

    public void Start() => tile = GetComponent<Tile>();
    
    /// <summary>
    /// Fills the tunnel with dirt.
    /// </summary>
    /// The tunnel tile will be replaced with a dirt tile.
    /// <returns>The newly created tile</returns>
    public Tile FillWithDirt()
    {
        var newTile = tile.Tilemap.ReplaceTile(tile.Location, tile.Tilemap.DirtTilePrefab);
        Debug.Assert(newTile != null, "filling tunnel with dirt failed unexpectedly");
        return newTile;
    }
}
