using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class EnemyEvent {

    public abstract float GetProgress();

    public static readonly Dictionary<EnemyEvent, int> EventsAndWeight = new () {
        { new IdleEvent(), 1 },
        { new TractorEvent(), 10 },
    };

    public abstract void TurnStartAction();
    public abstract bool TurnUpdateAction(float deltaTime);
    public abstract void TurnEndAction();

}

public class IdleEvent : EnemyEvent {
    
    public override float GetProgress() => _timeElapsed / duration;
    
    private const float duration = 3f;
    private float _timeElapsed = 0f;
    
    public override void TurnStartAction() {
        AudioManager.Instance.PlayAudio("Snore");
    }

    public override bool TurnUpdateAction(float deltaTime) {
        _timeElapsed += deltaTime;
        return _timeElapsed > duration;
    }

    public override void TurnEndAction() {
        AudioManager.Instance.StopAudio("Snore");
    }
}

public class TractorEvent : EnemyEvent {
    private const float SeedSpawnChance = 0.2f; 

    private const float TractorSpeed = 10f;
    private const float startX = 0f;
    private float endX => Tilemap.Instance.Width;
    
    public override float GetProgress() => _tractor.transform.position.x / endX;
    
    private GameObject _tractor;

    public override void TurnStartAction() {
        _tractor = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Tractor"));
        _tractor.transform.position = new Vector3(startX, 0, 0);
        AudioManager.Instance.PlayAudio("Tractor");
    }
    
    public override bool TurnUpdateAction(float deltaTime) {
        var currX = _tractor.transform.position.x;
        _tractor.transform.Translate(deltaTime * TractorSpeed, 0, 0);
        var newX = _tractor.transform.position.x;
        if (Mathf.RoundToInt(currX) != Mathf.RoundToInt(newX)) {
            TractorMovedToNextTile(Mathf.RoundToInt(newX));
        }
        
        return _tractor.transform.position.x > endX;
    }

    private void TractorMovedToNextTile(int tileX) {
        for (int y = -Tilemap.Instance.Height + 2; y <= 0; y++) {
            if (Tilemap.Instance.TryGetTile(tileX, y, out var tile)) {
                if (tile.IsGrowable) {
                    if (Random.value < SeedSpawnChance) {
                        tile.SpawnSeed();
                    }
                }
                else if (tile.MaybeDoCavein()) {
                    // TODO maybe screenshake, or cavein sound
                }
            }
        }

        if (Mole.Instance.CurrentTile.Location.x == tileX) {
            Mole.Instance.FallDown();
        }
    }

    public override void TurnEndAction() {
        GameObject.Destroy(_tractor);
        AudioManager.Instance.StopAudio("Tractor");
    }
}




