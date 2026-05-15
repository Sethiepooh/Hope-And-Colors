using System;
using System.Collections;
using UnityEngine;


public class Bruiser : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] float attackRange = 1.0f;
    [SerializeField] float dashDuration = 0.5f;

    [Header("Shotgun Settings")]
    [SerializeField] int pelletCount = 3;
    [SerializeField] float spreadAngle = 45f;

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] LayerMask playerLayer;
    bool clutter;

    // Update is called once per frame
    void Update()
    {
       
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
            int randomInt = UnityEngine.Random.Range(0, 2);
            Vector2 playerPos = player.transform.position;
            direction = (playerPos - rb.position).normalized;
            if (randomInt == 0)
                direction = -direction;
        }

        tRend.emitting = true;
        rb.linearVelocity = direction * moveSpeed;
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = Vector2.zero;
        tRend.emitting = false;
    }

    void FireShotgun()
    {
        Vector2 playerPos = player.transform.position;
        Vector2 direction = (playerPos - (Vector2)transform.position).normalized;
        float angleStep = spreadAngle / (pelletCount - 1);

        for (int i = 0; i < pelletCount; i++)
        {
            float angle = -spreadAngle / 2 + angleStep * i;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 pelletDir = rotation * direction;

            Projectile projectileInstance = projectilePool.GetProjectile(
               transform.position,
               Quaternion.LookRotation(Vector3.forward, pelletDir)
           );
            projectileInstance.Initialize(projectilePool, false, pelletDir);
        }
    }

    public override void Attack()
    {
        FireShotgun();
    }

    public override void AddToBeatCount()
    {
        if (active)
        {
            if (beatCount == 8)
            {
                beatCount = 1;
            }
            else
            {
                beatCount++;
            }

            if (beatCount < 3)
            {
                StartCoroutine(DashTowardsPlayer());
            }
            else if (beatCount > 4 && beatCount < 8)
            {
                FireShotgun();
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
