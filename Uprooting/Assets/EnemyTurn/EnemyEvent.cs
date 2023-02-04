using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyEvent {
    
    public static readonly List<EnemyEvent> Events = new () {
        new TractorEvent()
    };

    public abstract void TurnStartAction();
    public abstract bool TurnUpdateAction(float deltaTime);
    public abstract void TurnEndAction();

}

public class TractorEvent : EnemyEvent {
    
    private const float TractorSpeed = 1f;
    
    private GameObject _tractor;

    public override void TurnStartAction() {
        _tractor = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Tractor"));
        _tractor.transform.position = new Vector3(-10, 0, 0);
        // TODO start tractor sound
    }
    
    public override bool TurnUpdateAction(float deltaTime) {
        _tractor.transform.Translate(deltaTime * TractorSpeed, 0, 0);
        return _tractor.transform.position.x > 10;
    }

    public override void TurnEndAction() {
        GameObject.Destroy(_tractor);
        // TODO stop tractor sound
    }
}
