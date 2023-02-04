using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TurnBasedSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mole : MonoBehaviour
{
    public Animator animator;    
    
    [SerializeField]
    [Tooltip("Max speed of mole in units per second.")]
    private float speed = 1f;
    
    [SerializeField]
    [Tooltip("The tile coordinates of the mole's starting position.")]
    private Vector2Int startPosition;
    
    Vector2 currMoveInputDirection;

    private bool isMoving = false;
    private MovementType movementType = MovementType.None;
    private float movementPercent = 0f;
    private Tile currTile;
    private Tile nextTile;

    public void Start() {
        Tilemap.Instance.TryGetTile(startPosition.x, startPosition.y, out currTile);
        nextTile = currTile;
        transform.position = new Vector3(currTile.Location.x, currTile.Location.y, 0);
    }

    public void Update()
    {
        // TODO: Add animation logic here.
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
            movementPercent = 0f;
            currTile = nextTile;
            isMoving = false;
            CheckForMovementStart();
        }
        transform.position = Vector2.Lerp(new Vector2(currTile.Location.x, currTile.Location.y),
            new Vector2(nextTile.Location.x, nextTile.Location.y), movementPercent);
    }

    public void OnMove(InputValue value)
    {
        currMoveInputDirection = value.Get<Vector2>();
        CheckForMovementStart();
    }

    private void CheckForMovementStart() {
        if (isMoving) return;
        var moveDirectionInt = (Mathf.RoundToInt(currMoveInputDirection.x), Mathf.RoundToInt(currMoveInputDirection.y));
        
        if (!currTile.TryGetNeighbour(moveDirectionInt, out var possibleNextTile))
            return;
        if (!Tilemap.Instance.TryGetMovementType(currTile, possibleNextTile, out movementType))
            return;
        if (!TurnSystemController.Instance.TryDoMovement(movementType))
            return;

        // now we know we can move
        animator.SetBool("isMoving", true);
        // TODO set animation variables here

        nextTile = possibleNextTile;
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
