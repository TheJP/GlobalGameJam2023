using System;
using TurnBasedSystem;
using UnityEngine;

public class TurnSystemController : MonoBehaviour {
    [field: SerializeField] public int PlayerAPPerTurn { get; private set; } = 10;
    [field: SerializeField] public int PlayerMovementPerTurn { get; private set; } = 10;

    public int TurnCount { get; private set; } = 0;
    
    /**
     * Singleton instance.
     */
    public static TurnSystemController Instance { get; private set; }
    
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    
    private void Start() {
        StartPlayerTurn();
    }

    #region Turn Start/End Events
    
    /**
     * Called when the player's turn ends.
     */
    public event OnPlayerTurnEnd OnPlayerTurnEnd;
    
    /**
     * Called when the enemy's turn ends.
     */
    public event OnEnemyTurnEnd OnEnemyTurnEnd;

    private void StartPlayerTurn() {
        CurrentAPLeft = PlayerAPPerTurn;
        CurrentMovementLeft = PlayerMovementPerTurn;
    }

    public void EndPlayerTurn() {
        TurnCount++;
        OnPlayerTurnEnd?.Invoke(this, EventArgs.Empty);
        StartEnemyTurn();
    }

    private void StartEnemyTurn() {
        // TODO: Do stuff in enemy turn
        EndEnemyTurn();
    }
    
    public void EndEnemyTurn() {
        OnEnemyTurnEnd?.Invoke(this, EventArgs.Empty);
        StartPlayerTurn();
    }
    
    #endregion

    
    #region Action
    
    public int CurrentAPLeft { get; private set; }

    /**
     * Returns true if the action can be performed (enough AP), false otherwise.
     */
    public bool CanDoAction(ActionType actionType) {
        return CurrentAPLeft >= ActionHandler.ActionCosts[actionType];
    }

    /**
     * Returns true if the action was successfully performed (AP are subtracted), false otherwise.
     */
    public bool TryDoAction(ActionType actionType) {
        if (!CanDoAction(actionType)) {
            return false;
        }
        CurrentAPLeft -= ActionHandler.ActionCosts[actionType];
        return true;
    }

    #endregion
    
    
    #region Movement
    
    public int CurrentMovementLeft { get; private set; }

    /**
     * Returns true if the movement can be performed (enough movement points), false otherwise.
     */
    public bool CanDoMovement(MovementType movementType) {
        return CurrentMovementLeft >= MovementHandler.MovementCosts[movementType];
    }

    /**
     * Returns true if the movement was successfully performed (movement points are subtracted), false otherwise.
     */
    public bool TryDoMovement(MovementType movementType) {
        if (!CanDoMovement(movementType)) {
            return false;
        }
        CurrentMovementLeft -= MovementHandler.MovementCosts[movementType];
        return true;
    }

    #endregion

    
}

public delegate void OnPlayerTurnEnd(object sender, EventArgs args);
public delegate void OnEnemyTurnEnd(object sender, EventArgs args);