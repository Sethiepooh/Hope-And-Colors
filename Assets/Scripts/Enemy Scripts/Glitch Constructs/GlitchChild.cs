using System.Collections;
using UnityEngine;

public class GlitchChild : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] float attackRange = 1.0f;
    [SerializeField] int damage = 5;
    [SerializeField] float dashDuration = 0.5f;
    public bool alternate = false;
    int beatCount = 0;
    bool swing = false;

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
        if (swing)
        {
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
            foreach (Collider2D objects in hitObjects)
            {
                if (objects.gameObject.CompareTag("Player") || objects.gameObject.CompareTag("Bomb") || objects.gameObject.CompareTag("Obstacle"))
                {
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

        facedDirection.position = new Vector2(transform.position.x + direction.normalized.x, transform.position.y + direction.normalized.y);
        attackIndicator.transform.rotation = Quaternion.LookRotation(Vector3.forward, facedDirection.position - transform.position);

        tRend.emitting = true;
        rb.linearVelocity = direction * moveSpeed;
        swing = true;


        yield return new WaitForSeconds(dashDuration);
        swing = false;
        rb.linearVelocity = Vector2.zero;
        tRend.emitting = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public override void Attack()
    {
        StartCoroutine(DashTowardsPlayer());
    }

    public override void AddToBeatCount()
    {
        //Debug.Log("Glitch Child Beat Added");
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

            if (alternate)
            {
                if (beatCount > 4)
                {
                    sRend.color = attackColor;
                    StopAllCoroutines();
                    Attack();
                }
                else
                {
                    sRend.color = defaultColor;
                }
            }
            else
            {
                if (beatCount <= 4)
                {
                    sRend.color = attackColor;
                    StopAllCoroutines();
                    Attack();
                }
                else
                {
                    sRend.color = defaultColor;
                }
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
        Gizmos.DrawWireSphere(this.transform.position, attackRange);
    }
}
