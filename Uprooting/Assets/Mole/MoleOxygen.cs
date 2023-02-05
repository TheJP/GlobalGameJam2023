using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mole))]
public class MoleOxygen : MonoBehaviour
{

    [SerializeField]
    private Material postProcessing;

    [SerializeField]
    private float radiusNoOxygen = 0.5f;

    [SerializeField]
    private float animationDuration = 1f;

    public bool HasOxygen { get; private set; } = true;

    private float previousRadius = 0f;
    private float currentRadius;
    private float animationStart;

    public IEnumerator Start()
    {
        animationStart = Time.time;
        var mole = GetComponent<Mole>();
        yield return null; // Make sure tilemap is generated
        SetOxygen(Tilemap.Instance.HasOxygen(mole.CurrentTile.Location));
        Tilemap.Instance.TileChanged += _ => SetOxygen(Tilemap.Instance.HasOxygen(mole.CurrentTile.Location));
    }

    public void Update()
    {
        var time = Time.time - animationStart;
        var delta = time / animationDuration;
        var targetRadius = HasOxygen ? 1f : radiusNoOxygen;

        if (delta > 1) { previousRadius = targetRadius; }

        currentRadius = Mathf.Lerp(previousRadius, targetRadius, delta);
        postProcessing.SetFloat("_BlackRadius", currentRadius);
    }

    public void SetOxygen(bool value)
    {
        Debug.Log($"Set Oxygen {value}");
        if (HasOxygen == value)
        {
            return;
        }

        HasOxygen = value;
        previousRadius = currentRadius;
        animationStart = Time.time;
    }
}
