using System.Collections;
using UnityEngine;

public class GlitchChild : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] float attackRange = 1.0f;
    [SerializeField] int damage = 5;
    [SerializeField] Transform attackPoint;
    [SerializeField] float dashDuration = 0.5f;
    public bool alternate = false;
    int beatCount = 0;
    bool slash = false;

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 3.0f;
    Rigidbody2D rb;
    GameObject player;
    [SerializeField]LayerMask playerLayer;
    bool clutter;

    EnemyManager enemyManager;
    PulseManager pulseManager;
    [Header("Effects")]
    [SerializeField] Color attackColor;
    TrailRenderer tRend;
    Color defaultColor;
    SpriteRenderer sRend;

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
        enemyManager.AddEnemy(this.gameObject);
        pulseManager = GameObject.FindGameObjectWithTag("RhythmManager").GetComponent<PulseManager>();
        pulseManager.AddEntity(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (slash)
        {
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
            if (hitPlayers.Length > 0)
            {
                if (hitPlayers[0].CompareTag("Player"))
                {
                    player.GetComponent<Health>().TakeDamage(damage);
                    Debug.Log("Player Hit!");
                    slash = false;
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
            direction = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
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

    public override void AddToBeatCount()
    {
        if (!death)
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
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
