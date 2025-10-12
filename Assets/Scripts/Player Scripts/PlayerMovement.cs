using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Components
    Rigidbody2D rb;
    Vector2 movement;

    // Movement variables
    float currentSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    bool sprinting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(movement.x * currentSpeed, movement.y * currentSpeed);
    }

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentSpeed = sprintSpeed;
        }
        else if (context.canceled)
        {
            currentSpeed = walkSpeed;
        }
    }
}
