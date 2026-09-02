using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Components
    Rigidbody2D rb;
    Vector2 movement;
    //PulseManager pulseManager;
    Health health;
    PlayerAttack playerAttack;
    Vector2 lastPos;
    Animator animator;

    // Movement variables
    [Header("Movement Stats")]
    public float walkSpeed;
    public float sprintSpeed;
    public bool slowed = false;
    public bool sprintByDefault;
    public bool allegro;
    public float allegroAfterimageInterval;
    float afterimageIntervalTimer;
    float currentSpeed;
    bool freeze;
    [HideInInspector] public bool controlable = true;

    // Dash variables
    [Header("Dash Stats")]
    public bool dashDisabled;
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;
    bool dashing;
    bool canDash = true;
    Collider2D playerCollider;
    [SerializeField] LayerMask dodgeLayer;
    [SerializeField] AfterimageEffect effect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        currentSpeed = sprintSpeed;
        animator = GetComponent<Animator>();
        animator.SetBool("Sprinting", sprintByDefault);
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
                    rb.linearVelocity = new Vector2(movement.x * currentSpeed, movement.y * currentSpeed) * 2;
                }
                else
                {
                    rb.linearVelocity = new Vector2(movement.x * currentSpeed, movement.y * currentSpeed);
                }
            }
            else
                rb.linearVelocity = Vector2.zero;
        }

        if (allegro)
        {
            afterimageIntervalTimer += Time.fixedDeltaTime;
            if(afterimageIntervalTimer > allegroAfterimageInterval)
            {
                effect.GenerateAfterimage();
                afterimageIntervalTimer = 0;
            }
        }
    }

    public void Slowdown(float slowSpeed)
    {
        slowed = true;
        canDash = false;
        currentSpeed = slowSpeed;
    }

    public void Speedup()
    {
        slowed = false;
        canDash = true;
        currentSpeed = sprintByDefault ? sprintSpeed : walkSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetBool("Moving", true);
        }
        else
        {
            animator.SetBool("Moving", false);
        }
            movement = context.ReadValue<Vector2>();
        
        if(movement.x > 0)
        {
            animator.SetFloat("Dir", 0);
        }
        else if(movement.x < 0)
        {
            animator.SetFloat("Dir", 1);
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if(slowed) return;

        if (context.performed)
        {
            if (sprintByDefault)
            {
                animator.SetBool("Sprinting", false);
                currentSpeed = walkSpeed;
            }
            else
            {
                animator.SetBool("Sprinting", true);
                currentSpeed = sprintSpeed;
            }
           
        }
        else if (context.canceled)
        {
            if (sprintByDefault)
            {
                animator.SetBool("Sprinting", true);
                currentSpeed = sprintSpeed;
            }
            else
            {
                animator.SetBool("Sprinting", false);
                currentSpeed = walkSpeed;
            }
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(dashDisabled) return;
        if (context.performed && !freeze && canDash && controlable)
        {
            StartCoroutine(HandleDash());
        }
    }

    public void ActivateHitStun()
    {
        StartCoroutine(HitStun());
    }

    IEnumerator HitStun()
    {
        freeze = true;
        controlable = false;
        rb.linearVelocity  = Vector2.zero;

        yield return new WaitForSeconds(.2f);

        freeze = false;
        controlable = true;
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
        StartCoroutine(HandleAfterimages(1));
        Vector2 dir = movement;
        dashing = true;
        playerCollider.excludeLayers = dodgeLayer;
        health.damagable = false;
        rb.AddForce(dir * dashSpeed, ForceMode2D.Impulse);
        canDash = false;
        yield return new WaitForSeconds(dashTime);
        rb.linearVelocity = Vector2.zero;
        dashing = false;
        StartCoroutine(DashCooldown());
        yield return new WaitForSeconds(.1f);
        playerCollider.excludeLayers = 0;
        health.damagable = true;
    }

    IEnumerator HandleAfterimages(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            effect.GenerateAfterimage();
            yield return new WaitForSeconds(dashTime/amount);
        }
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
        animator.SetBool("Stage Dive", false);
        playerAttack.FireDiveWave();
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

    public Vector2 GetMovementLateral()
    {
        if(movement.x < 0)
        {
            lastPos = new Vector2(-1, 0);
        }
        else if(movement.x > 0)
        {
            lastPos = new Vector2(1, 0);
        }

        return lastPos;
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
