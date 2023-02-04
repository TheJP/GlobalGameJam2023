using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class EnemyEvent {
    
    public static readonly Dictionary<EnemyEvent, int> EventsAndWeight = new () {
        { new IdleEvent(), 1 },
        { new TractorSeedEvent(), 10 },
    };

    public abstract void TurnStartAction();
    public abstract bool TurnUpdateAction(float deltaTime);
    public abstract void TurnEndAction();

}

public class IdleEvent : EnemyEvent {
    
    private const float duration = 3f;
    private float _timeElapsed = 0f;
    
    public override void TurnStartAction() {
        // TODO start idle sound
    }

    public override bool TurnUpdateAction(float deltaTime) {
        _timeElapsed += deltaTime;
        return _timeElapsed > duration;
    }

    public override void TurnEndAction() {
        // TODO stop idle sound
    }
}

public abstract class TractorEvent : EnemyEvent {
    
    private const float TractorSpeed = 3f;
    private const float startX = 0f;
    private const float endX = 20f;
    
    private GameObject _tractor;

    public override void TurnStartAction() {
        _tractor = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Tractor"));
        _tractor.transform.position = new Vector3(startX, 0, 0);
        AudioManager.Instance.PlayAudio("Tractor");
    }
    
    public override bool TurnUpdateAction(float deltaTime) {
        _tractor.transform.Translate(deltaTime * TractorSpeed, 0, 0);
        return _tractor.transform.position.x > endX;
    }

    public override void TurnEndAction() {
        GameObject.Destroy(_tractor);
        AudioManager.Instance.StopAudio("Tractor");
    }
}

public class TractorSeedEvent : TractorEvent {
    private const float SeedSpawnChance = 0.3f; 
    public override void TurnEndAction() {
        base.TurnEndAction();

        foreach (var growableTile in Tilemap.Instance.GetGrowableTiles()) {
            if (Random.value < SeedSpawnChance) {
                growableTile.SpawnSeed();
            }
        }
    }
}



