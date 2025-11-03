using System.Collections;
using UnityEngine;

public class GlitchFather : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] float attackRange = 5f;
    [SerializeField] int damage = 20;
    [SerializeField] float dashDuration = 0.5f;
    int beatCount = 0;
    bool pound = false;

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 3.0f;
    Rigidbody2D rb;
    GameObject player;
    [SerializeField] LayerMask playerLayer;

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
        if (pound)
        {
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
            if (hitPlayers.Length > 0)
            {
                if (hitPlayers[0].CompareTag("Player"))
                {
                    player.GetComponent<Health>().TakeDamage(damage);
                    Debug.Log("Player Hit!");
                    pound = false;
                }
            }
        }
    }

    public override void Attack()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
        if (hitPlayers.Length > 0)
        {
            if (hitPlayers[0].CompareTag("Player"))
            {
                player.GetComponent<Health>().TakeDamage(damage);
                Debug.Log("Player Hit!");
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

    }

    public override void AddToBeatCount()
    {
        if (beatCount == 8)
        {
            Attack();
            beatCount = 1;
        }
        else
        {
            beatCount++;
        }

        if (beatCount == 4)
        {
            StartCoroutine(DashTowardsPlayer());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
