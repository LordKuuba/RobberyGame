using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [Space]
    [SerializeField] private float m_playerSpeed;

    public bool CanMove { get; private set; }

    private Vector3 movement;
    private Vector3 velocity;

    void FixedUpdate()
    {
        if (CanMove)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            if (movement.magnitude > 1)
            {
                movement.Normalize();
            }

            // Apply the movement force to the Rigidbody
            rb.MovePosition(rb.position + movement * m_playerSpeed * Time.fixedDeltaTime);
        }
    }

    public void ChangeInputState(bool newState)
    {
        CanMove = newState;
    }
}
