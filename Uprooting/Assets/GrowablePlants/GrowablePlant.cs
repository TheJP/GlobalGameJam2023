using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SpriteRenderer))]
public class GrowablePlant : MonoBehaviour {
    
    [SerializeField]
    private Sprite[] spriteGrowthStages;
    
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    
    private int FullGrownStage => spriteGrowthStages.Length - 1;
    
    private int growTimer = 0;

    
    public void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteGrowthStages is { Length: > 0 }) {
            spriteRenderer.sprite = spriteGrowthStages[0];
        }

        TurnSystemController.Instance.OnEnemyTurnEnd += Grow;
    }

    private void Grow(object sender, EventArgs args) {
        spriteRenderer.sprite = spriteGrowthStages[++growTimer];
        if (growTimer >= FullGrownStage) {
            TurnSystemController.Instance.OnEnemyTurnEnd -= Grow;
        }
    }
}