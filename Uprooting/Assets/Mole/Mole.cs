using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Mole : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Max speed of mole in units per second.")]
    private float speed = 1f;

    private Rigidbody2D body;
    private Vector2 move = Vector2.zero;

    public void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        // TODO: Add animation logic here.
    }

    public void FixedUpdate()
    {
        body.velocity = move * speed;
    }

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
        body.velocity = move * speed;
    }
}
