using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour {
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
        currentEvent = GetRandomEnemyEvent();

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

    private static EnemyEvent GetRandomEnemyEvent() {
        EnemyEvent result = null;
        var totalWeight = EnemyEvent.EventsAndWeight.Sum(cardNumber => cardNumber.Value);

        var randNumber = Random.Range(0, totalWeight);

        foreach (var (enemyEvent, weight) in EnemyEvent.EventsAndWeight) {
            if (randNumber >= weight) {
                randNumber -= weight;
            }
            else {
                result = enemyEvent;
                break;
            }
        }

        return result;
    }
}