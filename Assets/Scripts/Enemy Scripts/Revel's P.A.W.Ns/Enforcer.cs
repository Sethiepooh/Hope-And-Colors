using UnityEngine;
using System.Collections;

public class Enforcer : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] float attackRange = 1.0f;
    [SerializeField] int defaultDamage = 5;
    [SerializeField] int empoweredDamage = 10;
    int damage;
    [SerializeField] float dashDuration = 0.5f;
    bool swing;
    [SerializeField] bool canDoubleTime;

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] LayerMask playerLayer;
    bool clutter;
    public Transform facedDirection;

    [Header("Effects")]
    //public GameObject attackIndicator;
    //public AttackIndicator aIndicator;
    public Animator anim;
    [SerializeField] ParticleSystem dashParticles;
    [SerializeField] DashEffectHandler dashEffectHandler;


    // Update is called once per frame
    void Update()
    {
        if(empowered)
        {
            damage = empoweredDamage;
        }
        else
        {
            damage = defaultDamage;
        }

        if (swing)
        {
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
            foreach (Collider2D objects in hitObjects)
            {
                if (objects.gameObject.CompareTag("Player") || objects.gameObject.CompareTag("Bomb") || objects.gameObject.CompareTag("Obstacle"))
                {
                    Debug.Log("Hit Detected");
                    // Check if enemy is in front of player
                    Vector2 relativePos = objects.transform.position - transform.position;
                    Vector2 forward = (Vector2)facedDirection.position - (Vector2)transform.position;
                    float angle = Vector3.Angle(relativePos, forward);
                    if (angle < 90f)
                    {
                        //Apply damage to player
                        Health hp = objects.gameObject.GetComponent<Health>();
                        hp.TakeDamage(damage);

                        if(canDoubleTime)
                        roomEncounterManager.TriggerDoubleTime(5f);
                    }
                }
            }
            //aIndicator.AttackFlash();
        }
    }

    IEnumerator DashTowardsPlayer()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        Vector2 direction;
        if (!clutter)
        {
            Vector2 playerPos = player.transform.position;
            direction = (playerPos - rb.position).normalized;
        }
        else
        {
            int randomInt = Random.Range(0, 2);
            Vector2 playerPos = player.transform.position;
            direction = (playerPos - rb.position).normalized;
            if (randomInt == 0)
                direction = -direction;
        }

        anim.SetBool("Hold", false);

        facedDirection.position = new Vector2(transform.position.x + direction.normalized.x,transform.position.y);
        this.GetComponent<SpriteRenderer>().flipX = direction.x > 0;
        if(direction.x > 0)
        {
            dashEffectHandler.FlipDashEffect(true);
        }
        else
        {
            dashEffectHandler.FlipDashEffect(false);
        }
        //attackIndicator.transform.rotation = Quaternion.LookRotation(Vector3.forward, facedDirection.position - transform.position);

        //tRend.emitting = true;
        dashEffectHandler.SetDashEffect();
        rb.linearVelocity = direction * moveSpeed;
        swing = true;
        anim.SetBool("Hold", true);
        dashParticles.Play();


        yield return new WaitForSeconds(dashDuration);
        dashEffectHandler.SetDashEffect(0, false);
        dashParticles.Stop();
        swing = false;
        anim.SetBool("Hold", false);
        rb.linearVelocity = Vector2.zero;
        //tRend.emitting = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public override void Attack()
    {
        StartCoroutine(DashTowardsPlayer());
    }

    public override void AddToBeatCount()
    {
        if (active)
        {
            if (beatCount == 16)
            {
                beatCount = 1;
            }
            else
            {
                beatCount++;
            }

            if(beatCount < 9)
            {

                if (beatCount % 2 == 0)
                {
                    anim.SetBool("Attack", true);
                    sRend.color = attackColor;
                    StopAllCoroutines();
                    Attack();
                }
            }
            else
            {
                anim.SetBool("Attack", false);
                sRend.color = defaultColor;
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            clutter = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            clutter = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
