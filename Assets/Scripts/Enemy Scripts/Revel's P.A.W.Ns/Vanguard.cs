using System;
using System.Collections;
using UnityEngine;

public class Vanguard : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] float attackRange = 1.0f;
    [SerializeField] int defaultDamage = 5;
    [SerializeField] int empoweredDamage = 10;
    int damage;
    [SerializeField] float dashDuration = 0.5f;
    bool alt;
    bool charging;

    [Header("Shotgun Settings")]
    [SerializeField] GameObject shotgunPrefab;
    [SerializeField] int pelletCount = 5;
    [SerializeField] float spreadAngle = 45f;

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] LayerMask playerLayer;
    bool clutter;
    public Transform facedDirection;

    [Header("Effects")]
    public GameObject attackIndicator;
    public AttackIndicator aIndicator;

    // Update is called once per frame
    void Update()
    {
        if (empowered)
        {
            damage = empoweredDamage;
        }
        else
        {
            damage = defaultDamage;
        }

        if (charging)
        {
            // Detect Player in range
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
                        roomEncounterManager.TriggerDoubleTime(5f);
                    }
                }
            }
            aIndicator.AttackFlash();
        }
    }

    IEnumerator DashTowardsPlayer()
    {
        Vector2 direction;
        Vector2 playerPos = player.transform.position;
        direction = (playerPos - (Vector2)transform.position).normalized;
        Vector2 rigidDir =  CheckChargeDirection(direction);

        tRend.emitting = true;
        rb.linearVelocity = rigidDir * moveSpeed;
        charging = true;


        yield return new WaitForSeconds(dashDuration);
        charging = false;
        rb.linearVelocity = Vector2.zero;
        tRend.emitting = false;
        alt = !alt;
    }

    void FireShotgun()
    {
        Vector2 playerPos = player.transform.position;
        Vector2 direction = (playerPos - (Vector2)transform.position).normalized;
        Vector2 rigidDir = CheckChargeDirection(direction);
        Vector2 fireDir = facedDirection.localPosition;
        float angleStep = spreadAngle / (pelletCount - 1);

        for (int i = 0; i < pelletCount; i++)
        {
            float angle = -spreadAngle / 2 + angleStep * i;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 pelletDir = rotation * fireDir;

            Projectile projectileInstance = projectilePool.GetProjectile(
               transform.position,
               Quaternion.LookRotation(Vector3.forward, pelletDir)
           );
            projectileInstance.Initialize(projectilePool, false, pelletDir);
        }
        alt = !alt;
    }

    Vector2 CheckChargeDirection(Vector2 dir)
    {
        if (MathF.Abs(dir.x) > MathF.Abs(dir.y))
        {
            dir.y = 0;
            if (dir.x < 0)
            {
                facedDirection.localPosition = new Vector2(-1, 0);
                attackIndicator.transform.rotation = Quaternion.Euler(0, 0, 90f);
            }
            else
            {
                facedDirection.localPosition = new Vector2(1, 0);
                attackIndicator.transform.rotation = Quaternion.Euler(0, 0, -90f);
            }
            dir = facedDirection.localPosition;
            Debug.Log(dir);
            return dir;
        }
        else
        {
            dir.x = 0;
            if (dir.y < 0)
            {
                facedDirection.localPosition = new Vector2(0, -1);
                
                attackIndicator.transform.rotation = Quaternion.Euler(0, 0, 180f);
            }
            else
            {
                facedDirection.localPosition = new Vector2(0, 1);
                
                attackIndicator.transform.rotation = Quaternion.Euler(0, 0, 0f);
            }
            dir = facedDirection.localPosition;
            Debug.Log(dir);
            return dir;
        }
    }

    public override void Attack()
    {
        StartCoroutine(DashTowardsPlayer());
    }

    public override void AddToBeatCount()
    {
        if (active)
        {
            if (beatCount == 4)
            {
                beatCount = 1;
            }
            else
            {
                beatCount++;
            }

            if (alt)
            {
                if(beatCount % 4 == 0)
                {
                    sRend.color = attackColor;
                    Attack();
                }
            }
            else
            {
                if (beatCount % 4 == 0)
                {
                    sRend.color = attackColor;
                    FireShotgun();
                }
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
