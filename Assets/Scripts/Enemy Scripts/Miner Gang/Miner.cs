using System.Collections;
using UnityEngine;

public class Miner : EnemyBase
{
    [Header("Ground Pound Stats")]
    [SerializeField] float groundPoundRadiusIncrease = 2.0f;
    [SerializeField] float groundPoundBaseRadius = 4f;
    [SerializeField] int groundPoundBaseDamage = 5;
    [SerializeField] int groundPoundDamageIncrease = 2;
    [SerializeField] GameObject groundPoundEffect;
    int currentGroundPoundDamage;
    float currentGroundPoundRadius;

    [Header("Dash Stats")]
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] int damage = 5;
    [SerializeField] Transform attackPoint;
    [SerializeField] float dashDuration = 0.5f;
    public bool alternate = false;
    bool slash = false;
    bool alt = false;

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] LayerMask playerLayer;
    bool clutter;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentGroundPoundRadius = groundPoundBaseRadius;
        currentGroundPoundDamage = groundPoundBaseDamage;
    }

    // Update is called once per frame
    void Update()
    {
        if (slash)
        {
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
            if (hitPlayers.Length > 0)
            {
                foreach (Collider2D player in hitPlayers)
                {
                    if (player.CompareTag("Player"))
                    {
                        player.GetComponent<Health>().TakeDamage(damage);
                        slash = false;
                    }
                    else if (player.CompareTag("Obstacle"))
                    {
                        player.GetComponent<Health>().TakeDamage(damage);
                        slash = false;
                    }
                    else if (player.CompareTag("Bomb"))
                    {
                        player.GetComponent<Health>().TakeDamage(damage);
                        slash = false;
                    }
                }
            }
        }
    }

    IEnumerator DashTowardsPlayer()
    {
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

        tRend.emitting = true;
        rb.linearVelocity = direction * moveSpeed;
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = Vector2.zero;
        slash = false;
        tRend.emitting = false;
    }

    public override void Attack()
    {
        StartCoroutine(DashTowardsPlayer());
        slash = true;
    }

    public void GroundPound(float radius)
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, radius, playerLayer);
        if (hitPlayers.Length > 0)
        {
            foreach (Collider2D player in hitPlayers)
            {
                if (player.CompareTag("Player"))
                {
                    player.GetComponent<Health>().TakeDamage(currentGroundPoundDamage);
                }
                else if (player.CompareTag("Obstacle"))
                {
                    player.GetComponent<Health>().TakeDamage(currentGroundPoundDamage);
                }
                else if (player.CompareTag("Bomb"))
                {
                    player.GetComponent<Health>().TakeDamage(currentGroundPoundDamage);
                }
            }
        }
        GameObject effect = Instantiate(groundPoundEffect, attackPoint.position, Quaternion.identity);
        effect.transform.localScale = new Vector3(radius, radius, 1);

        Destroy(effect, 1f);
    }

    public override void AddToBeatCount()
    {
        //Debug.Log("Glitch Child Beat Added");
        if (active)
        {
            beatCount++;
            

            if (beatCount == 12)
            {
                beatCount = 0;
            }


            if (beatCount < 3)
            {
                sRend.color = attackColor;
               // StopAllCoroutines();
                Attack();               
            }
            else
            {
                sRend.color = defaultColor;
            }

            if (beatCount > 3 && beatCount < 7)
            {
                GroundPound(currentGroundPoundRadius);
                currentGroundPoundRadius += groundPoundRadiusIncrease;
                currentGroundPoundDamage += groundPoundDamageIncrease;
                Debug.Log("Miner Attack on beat " + beatCount);
            }
            else if (beatCount == 7)
            {
                currentGroundPoundRadius = groundPoundBaseRadius;
                currentGroundPoundDamage = groundPoundBaseDamage;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")/* || collision.gameObject.CompareTag("Wall")*/)
        {
            clutter = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")/*|| collision.gameObject.CompareTag("Wall")*/)
        {
            clutter = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, currentGroundPoundRadius);
    }
}
