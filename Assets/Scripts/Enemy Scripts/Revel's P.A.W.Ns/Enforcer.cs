using UnityEngine;
using System.Collections;
using static EnemyManager;

public class Enforcer : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] float attackRange = 1.0f;
    [SerializeField] int defaultDamage = 5;
    [SerializeField] int empoweredDamage = 10;
    int damage;
    [SerializeField] float dashDuration = 0.5f;
    int beatCount = 0;

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 3.0f;
    Rigidbody2D rb;
    GameObject player;
    [SerializeField] LayerMask playerLayer;
    bool clutter;
    public Transform facedDirection;

    EnemyManager enemyManager;
    PulseManager pulseManager;
    [Header("Effects")]
    [SerializeField] Color attackColor;
    TrailRenderer tRend;
    Color defaultColor;
    SpriteRenderer sRend;
    public GameObject attackIndicator;
    public AttackIndicator aIndicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sRend = GetComponent<SpriteRenderer>();
        defaultColor = sRend.color;
        tRend = GetComponent<TrailRenderer>();
        tRend.emitting = false;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        // enemyManager.AddEnemy(this.gameObject);
        pulseManager = GameObject.FindGameObjectWithTag("RhythmManager").GetComponent<PulseManager>();
        pulseManager.AddEntity(this.gameObject, pulseManager.entitiesToPulse);
    }

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

        facedDirection.position = new Vector2(transform.position.x + direction.normalized.x,transform.position.y + direction.normalized.y);
        attackIndicator.transform.rotation = Quaternion.LookRotation(Vector3.forward, facedDirection.position - transform.position);
        
        tRend.emitting = true;
        rb.linearVelocity = direction * moveSpeed;
        yield return new WaitForSeconds(dashDuration /2);

        // Detect Player in range
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
        foreach(Collider2D objects in hitObjects)
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
                }
            }
        }
        aIndicator.AttackFlash();   
        yield return new WaitForSeconds(dashDuration / 2);
        rb.linearVelocity = Vector2.zero;
        tRend.emitting = false;
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

            if(beatCount %2 == 0 && beatCount < 9)
            {
                sRend.color = attackColor;
                Attack();
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
