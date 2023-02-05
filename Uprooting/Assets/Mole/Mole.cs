using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TurnBasedSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Mole : MonoBehaviour
{

    public static Mole Instance { get; private set; }

    public int Points { get; private set; } = 0;

    public Animator animator;
    public SpriteRenderer spriteRenderer;
    
    [SerializeField]
    [Tooltip("Max speed of mole in units per second.")]
    private float speed = 1f;
    
    [FormerlySerializedAs("carrotEatingTime")] [SerializeField]
    private float eatingTime = 1f;
    private float currEatingTime = 0f;
    public bool IsEating => currEatingTime > 0f;

    private Vector2 currMoveInputDirection;
    private bool diggingInput = false;

    public bool IsMoving { get; private set; } = false;
    private MovementType movementType = MovementType.None;
    private float movementPercent = 0f;
    private Tile currTile;
    private Tile nextTile;

    public Tile CurrentTile => currTile;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            Debug.LogError("Multiple Mole instances detected. Destroying duplicate.");
            return;
        }
        Instance = this;
    }

    public void Start() {
        var startPosition = (x: Mathf.RoundToInt(transform.position.x), y: Mathf.RoundToInt(transform.position.y));
        Tilemap.Instance.TryGetTile(startPosition.x, startPosition.y, out currTile);
        if (currTile == null) {
            Debug.LogError($"Mole's starting position {startPosition} is not a valid tile!");
        }
        nextTile = currTile;
        transform.position = new Vector3(currTile.Location.x, currTile.Location.y, 0);
    }

    public void Update()
    {
        if (!TurnSystemController.Instance.IsPlayerTurn) {
            return;
        }
        CheckForMovementStart();
    }

    public void FixedUpdate()
    {
        if (!TurnSystemController.Instance.IsPlayerTurn) {
            return;
        }
        if (IsEating) {
            currEatingTime -= Time.deltaTime;
            return;
        }
        if (IsMoving) {
            HandleMovement();
        }
        
    }

    private void HandleMovement() {
        movementPercent += Time.deltaTime * speed;
        if (movementPercent >= 1f) {
            EndMovement();
        }
        transform.position = Vector2.Lerp(new Vector2(currTile.Location.x, currTile.Location.y),
            new Vector2(nextTile.Location.x, nextTile.Location.y), movementPercent);
    }

    private void EndMovement() {
        if (movementType == MovementType.DigMovement) {
            nextTile = nextTile.DigTunnel();
            Assert.IsNotNull(nextTile, "new Tile after digging is null!");
            AudioManager.Instance.StopAudio("Dig");
            AudioManager.Instance.StopAudio("Eating");
        }
        movementType = MovementType.None;
        movementPercent = 0f;
        currTile = nextTile;
        IsMoving = false;
        EndAnimation();
    }

    public void OnMove(InputValue value)
    {
        currMoveInputDirection = value.Get<Vector2>();
    }

    public void OnDig(InputValue value) {
        diggingInput = value.isPressed;
    }

    private void CheckForMovementStart() {
        if (IsMoving || IsEating) return;
        if (currMoveInputDirection == Vector2.zero) return;

        var moveDirectionInt = (Mathf.RoundToInt(currMoveInputDirection.x), Mathf.RoundToInt(currMoveInputDirection.y));

        if (!currTile.TryGetNeighbour(moveDirectionInt, out var possibleNextTile))
            return;
        if (possibleNextTile.currGrowablePlant != null) {
            DigOutPlant(possibleNextTile);
            return;
        }
        if (!Tilemap.Instance.TryGetMovementType(currTile, possibleNextTile, out movementType))
            return;
        if (!TurnSystemController.Instance.CanDoMovement(movementType)) // only check if we can move, because we might not have the AP we need for stuff like digging
            return;

        if (movementType == MovementType.DigMovement) {
            // if (!diggingInput) {
            //     return;
            // }
            if (!TurnSystemController.Instance.TryDoAction(ActionType.Dig)) {
                // we don't have enough AP for digging
                return;
            }
            // we have enough AP for digging and spent them
            AudioManager.Instance.PlayAudio("Dig");
        }
        
        if (!TurnSystemController.Instance.TryDoMovement(movementType)) Debug.LogError("We should definitely have enough Movement Points");

        // now we know we can move
        StartMovement(possibleNextTile);
    }

    private void DigOutPlant(Tile nextTile) {
        if (!TurnSystemController.Instance.TryDoAction(ActionType.GrabPlant)) {
            return;
        }
        var plant = nextTile.currGrowablePlant;
        if (plant == null) {
            Debug.LogError("Trying to dig out plant, but there is no plant!");
            return;
        }
        AudioManager.Instance.PlayAudio("Eating");
        nextTile.DigOutPlant();
        Points += plant.Points;
        
        currEatingTime = eatingTime;
    }

    private void StartMovement(Tile nextTile) {
        StartAnimation();
        
        this.nextTile = nextTile;
        IsMoving = true;
    }

    private void OnDrawGizmos() {
        if (currTile == null || nextTile == null)
            return;
        if (currTile != nextTile) {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(nextTile.CenterLocation, Vector3.one * 0.5f);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawCube(currTile.CenterLocation, Vector3.one * 0.5f);
    }

    public void StartAnimation() {
        bool isFacingRight = (currMoveInputDirection.x > 0);
        spriteRenderer.flipX = isFacingRight;

        Vector3 rotationForVerticalMove = Vector3.zero;
        bool isFacingDown = (currMoveInputDirection.y < 0);
        bool isFacingUp = (currMoveInputDirection.y > 0);
        if (isFacingUp) {
            rotationForVerticalMove.z = -90;
        } else if (isFacingDown) {
            rotationForVerticalMove.z = 90; 
        }
        spriteRenderer.transform.eulerAngles = rotationForVerticalMove;
        
        animator.SetBool("isDigging", true);
    }
    
    public void EndAnimation() {
        animator.SetBool("isDigging", false);
    }
}
