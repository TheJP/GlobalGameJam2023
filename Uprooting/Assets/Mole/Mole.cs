using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TurnBasedSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class Mole : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Max speed of mole in units per second.")]
    private float speed = 1f;

    private Vector2 currMoveInputDirection;
    private bool diggingInput = false;

    private bool isMoving = false;
    private MovementType movementType = MovementType.None;
    private float movementPercent = 0f;
    private Tile currTile;
    private Tile nextTile;

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
        CheckForMovementStart();
    }

    public void FixedUpdate()
    {
        // body.velocity = move * speed;
        if (isMoving) {
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
        }
        movementType = MovementType.None;
        movementPercent = 0f;
        currTile = nextTile;
        isMoving = false;
    }

    public void OnMove(InputValue value)
    {
        currMoveInputDirection = value.Get<Vector2>();
    }

    public void OnDig(InputValue value) {
        diggingInput = value.isPressed;
    }

    private void CheckForMovementStart() {
        if (isMoving) return;
        if (currMoveInputDirection == Vector2.zero) return;
        
        var moveDirectionInt = (Mathf.RoundToInt(currMoveInputDirection.x), Mathf.RoundToInt(currMoveInputDirection.y));
        
        if (!currTile.TryGetNeighbour(moveDirectionInt, out var possibleNextTile))
            return;
        if (!Tilemap.Instance.TryGetMovementType(currTile, possibleNextTile, out movementType))
            return;
        if (!TurnSystemController.Instance.CanDoMovement(movementType)) // only check if we can move, because we might not have the AP we need for stuff like digging
            return;

        if (movementType == MovementType.DigMovement) {
            if (!diggingInput) {
                return;
            }
            if (!TurnSystemController.Instance.TryDoAction(ActionType.Dig)) {
                // we don't have enough AP for digging
                return;
            }
            // we have enough AP for digging and spent them
        }
        
        if (!TurnSystemController.Instance.TryDoMovement(movementType)) Debug.LogError("We should definitely have enough Movement Points");

        // now we know we can move
        StartMovement(possibleNextTile);
    }

    private void StartMovement(Tile nextTile) {
        // TODO set animation variables here

        this.nextTile = nextTile;
        isMoving = true;
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
}
