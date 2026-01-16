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
    PlayerAttack playerAttack;

    // Movement variables
    [Header("Movement Stats")]
    public float walkSpeed;
    public float sprintSpeed;
    bool slowed = true;
    public bool sprintByDefault;
    public bool allegro;
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
        playerAttack = GetComponent<PlayerAttack>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        pulseManager = GameObject.FindGameObjectWithTag("RhythmManager").GetComponent<PulseManager>();
        pulseManager.AddEntity(this.gameObject, pulseManager.entitiesToPulse);
        currentSpeed = sprintSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (slowed)
        {
            canDash = false;
            currentSpeed = walkSpeed;
        }
            

        if (!dashing && controlable)
        {
            if (!freeze)
            {
                if (allegro)
                {
                    rb.linearVelocity = new Vector2(movement.x * currentSpeed, movement.y * currentSpeed * 2);
                }
                else
                {
                    rb.linearVelocity = new Vector2(movement.x * currentSpeed, movement.y * currentSpeed);
                }
            }
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
        if(!canSprint) return;

        if (context.performed)
        {
            if (sprintByDefault)
            {
                currentSpeed = walkSpeed;
            }
            else
            {
                currentSpeed = sprintSpeed;
            }
           
        }
        else if (context.canceled)
        {
            if (sprintByDefault)
            {
                currentSpeed = sprintSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !freeze && canDash)
        {
            StartCoroutine(HandleDash());
        }
    }

   

    public void Dive()
    {
        if (!freeze && canDash)
        {
            StartCoroutine(HandleDive());
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

    IEnumerator HandleDive()
    {
        dashing = true;
        rb.AddForce(movement * (dashSpeed / .75f), ForceMode2D.Impulse);
        canDash = false;
        yield return new WaitForSeconds(dashTime * 2);
        rb.linearVelocity = Vector2.zero;
        dashing = false;
        canDash = true;
        playerAttack.sErupt.ReleaseAttack();
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
