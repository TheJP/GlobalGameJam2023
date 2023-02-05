using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tile))]
public class Tunnel : MonoBehaviour
{
    private Tile tile;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public void Start()
    {
        tile = GetComponent<Tile>();
        var background = Tilemap.Instance.BackgroundTunnelSpriteRenderer;
        var x = spriteRenderer.bounds.min.x - background.bounds.min.x;
        var y = spriteRenderer.bounds.min.y - background.bounds.min.y;
        while (x < 0) x += background.bounds.size.x;
        while (x > background.bounds.size.x) x -= background.bounds.size.x;
        Debug.Assert(y >= 0);
        var factor = background.sprite.pixelsPerUnit;
        var crop = new Rect(x * factor, y * factor, 1f * factor, 1f * factor);
        var sprite = Sprite.Create(background.sprite.texture, crop, new Vector2(0.5f, 0.5f), background.sprite.pixelsPerUnit);
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = Color.white;
    }

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
