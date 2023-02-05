using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SpriteRenderer))]
public class GrowablePlant : MonoBehaviour {
    
    [SerializeField]
    private Sprite plantSprite;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    
    [SerializeField]
    private int growthStagesCount = 3;
    
    [SerializeField]
    private float minScale = 0.5f;

    private float CurrentScale => Mathf.Lerp(minScale, 1f, _currGrowthStage / (float)(growthStagesCount - 1));

    private int _currGrowthStage = 0;
    private bool _hasJustBeenPlanted = true;

    
    public void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = plantSprite ? plantSprite : spriteRenderer.sprite;
        transform.localScale = Vector3.one * CurrentScale;

        TurnSystemController.Instance.OnEnemyTurnEnd += Grow;
    }

    private void Grow(object sender, EventArgs args) {
        if (_hasJustBeenPlanted) {
            _hasJustBeenPlanted = false;
            return;
        }
        _currGrowthStage++;
        transform.localScale = Vector3.one * CurrentScale;

        if (_currGrowthStage == growthStagesCount - 1) {
            TurnSystemController.Instance.OnEnemyTurnEnd -= Grow;
        }
    }
}