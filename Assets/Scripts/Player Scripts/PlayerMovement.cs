using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Components
    Rigidbody2D rb;
    Vector2 movement;

    // Movement variables
    [Header("Movement Stats")]
    public float walkSpeed;
    public float sprintSpeed;
    float currentSpeed;
    bool freeze;

    // Dash variables
    [Header("Dash Stats")]
    public float dashSpeed;
    public float maxDashTime;
    float dashTime;
    bool dashing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = walkSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dashing)
        {
            dashTime = 0f;
            if(!freeze)
                rb.linearVelocity = new Vector2(movement.x * currentSpeed, movement.y * currentSpeed);
            else
                rb.linearVelocity = Vector2.zero;
        }
        else
        {
            dashTime += Time.fixedDeltaTime;
            if (dashTime >= maxDashTime)
            {
                rb.linearVelocity = Vector2.zero;
                dashing = false;               
            }
        }           
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!dashing)
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

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !freeze)
        {
            dashing = true;
            rb.AddForce(movement * dashSpeed, ForceMode2D.Impulse);
        }
    }

    public Vector2 GetMovement()
    {
        return movement;
    }

    public void SetFreeze(bool b)
    {
        freeze = b;
    }
}
