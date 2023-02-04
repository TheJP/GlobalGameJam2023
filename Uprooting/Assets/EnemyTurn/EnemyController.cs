using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private EnemyEvent currentEvent = null;
    
    public void StartTurn() {
        var random = Random.Range(0, EnemyEvent.Events.Count);
        currentEvent = EnemyEvent.Events[random];
        
        currentEvent.TurnStartAction();
    }

    private void Update() {
        if (currentEvent != null) {
            var shouldEnd = currentEvent.TurnUpdateAction(Time.deltaTime);
            if (shouldEnd) {
                currentEvent.TurnEndAction();
                currentEvent = null;
                TurnSystemController.Instance.EndEnemyTurn();
            }
        }
    }
}
