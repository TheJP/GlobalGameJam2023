using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TurnBasedSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mole : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Max speed of mole in units per second.")]
    private float speed = 1f;
    
    [SerializeField]
    [Tooltip("The tile coordinates of the mole's starting position.")]
    private Vector2Int startPosition;
    
    private bool isMoving = false;
    private MovementType movementType = MovementType.None;
    private float movementPercent = 0f;
    private Tile currTile;
    private Tile nextTile;

    public void Start() {
        Tilemap.Instance.TryGetTile(startPosition.x, startPosition.y, out currTile);
        transform.position = new Vector3(currTile.Location.x, currTile.Location.y, 0);
    }

    public void Update()
    {
        // TODO: Add animation logic here.
    }

    public void FixedUpdate()
    {
        // body.velocity = move * speed;
        if (isMoving)
        {
            movementPercent += Time.deltaTime * speed;
            if (movementPercent >= 1f)
            {
                movementPercent = 0f;
                currTile = nextTile;
                isMoving = false;
            }
            transform.position = Vector2.Lerp(new Vector2(currTile.Location.x, currTile.Location.y), new Vector2(nextTile.Location.x, nextTile.Location.y), movementPercent);
        }
        
    }

    public void OnMove(InputValue value)
    {
        if (isMoving) return;
        var moveDirection = value.Get<Vector2>();
        var moveDirectionInt = (Mathf.RoundToInt(moveDirection.x), Mathf.RoundToInt(moveDirection.y));
        currTile.TryGetNeighbour(moveDirectionInt, out var possibleNextTile);
        if (possibleNextTile == null) return; // TODO what to do when we are at the edge of the map?
        
        if (!Tilemap.Instance.TryGetMovementType(currTile, possibleNextTile, out var movementType))
            return;
        
        isMoving = true;
    }
}
