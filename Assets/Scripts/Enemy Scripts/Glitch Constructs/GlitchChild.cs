using System.Collections;
using UnityEngine;

public class GlitchChild : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] float attackRange = 1.0f;
    [SerializeField] int damage = 5;
    [SerializeField] Transform attackPoint;
    [SerializeField] float dashDuration = 0.5f;
    int beatCount = 0;
    bool slash = false;

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 3.0f;
    Rigidbody2D rb;
    GameObject player;
    LayerMask playerLayer;

    EnemyManager enemyManager;
    PulseManager pulseManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
                }
            }          
        }
    }

    IEnumerator DashTowardsPlayer()
    {
        Vector2 playerPos = player.transform.position;
        Vector2 direction = (playerPos - rb.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = Vector2.zero;
        slash = false;
    }

    public override void Attack()
    {
        StartCoroutine(DashTowardsPlayer());
        slash = true;
    }

    public override void AddToBeatCount()
    {
        if(beatCount == 8)
        {
            beatCount = 1;
        }
        else
        {
            beatCount++;
        }

        if(beatCount > 4)
        {
            Attack();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
