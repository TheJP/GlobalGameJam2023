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

    public event Action<Tile> TileChanged;

    public Tilemap Tilemap { get; set; }

    public (int x, int y) Location { get; set; }

    private void InvokeTileChanged() => TileChanged?.Invoke(this);
}
