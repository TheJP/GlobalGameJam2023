using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [field: SerializeField]
    public bool AllowsAirflow { get; set; } = false;

    public Tilemap Tilemap { get; set; }
}
