using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Components
    Rigidbody2D rb;
    Vector2 movement;
    PulseManager pulseManager;
    Health health;

    // Movement variables
    [Header("Movement Stats")]
    public float walkSpeed;
    public float sprintSpeed;
    float currentSpeed;
    bool freeze;
    [HideInInspector] public bool controlable = true;

    // Dash variables
    [Header("Dash Stats")]
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;
    bool dashing;
    bool canDash = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        pulseManager = GameObject.FindGameObjectWithTag("RhythmManager").GetComponent<PulseManager>();
        pulseManager.AddEntity(this.gameObject, pulseManager.entitiesToPulse);
        currentSpeed = walkSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dashing && controlable)
        {
            if(!freeze)
                rb.linearVelocity = new Vector2(movement.x * currentSpeed, movement.y * currentSpeed);
            else
                rb.linearVelocity = Vector2.zero;
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
        if (context.performed && !freeze && canDash)
        {
            StartCoroutine(HandleDash());
        }
    }

    IEnumerator HandleDash()
    {
        dashing = true;
        rb.AddForce(movement * dashSpeed, ForceMode2D.Impulse);
        canDash = false;
        yield return new WaitForSeconds(dashTime);
        rb.linearVelocity = Vector2.zero;
        dashing = false;
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public Vector2 GetMovement()
    {
        return movement;
    }

    public void SetFreeze(bool b)
    {
        freeze = b;
    }

    public void SetCanDash(bool b)
    {
        canDash = b;
    }
}
