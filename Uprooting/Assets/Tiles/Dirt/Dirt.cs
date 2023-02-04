using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tile))]
public class Dirt : MonoBehaviour
{
    [SerializeField]
    private Sprite[] spriteVariations;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private Tile tile;

    public void Start()
    {
        tile = GetComponent<Tile>();
        if (spriteVariations != null && spriteVariations.Length > 0)
        {
            spriteRenderer.sprite = spriteVariations[Random.Range(0, spriteVariations.Length)];
        }
    }

    public void DigTunnel()
    {
        var success = tile.Tilemap.ReplaceTile(tile.Location, tile.Tilemap.TunnelTilePrefab);
        Debug.Assert(success, "tunnel digging failed unexpectedly");
    }
}
