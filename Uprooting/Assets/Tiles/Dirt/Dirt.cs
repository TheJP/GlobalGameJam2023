using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tile))]
public class Dirt : MonoBehaviour
{
    private Tile tile;

    public void Start() => tile = GetComponent<Tile>();

    public void DigTunnel()
    {
        var success = tile.Tilemap.ReplaceTile(tile.Location, tile.Tilemap.TunnelTilePrefab);
        Debug.Assert(success, "tunnel digging failed unexpectedly");
    }
}
