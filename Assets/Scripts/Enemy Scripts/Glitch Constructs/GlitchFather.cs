using System.Collections;
using UnityEngine;

public class GlitchFather : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] float attackRange = 5f;
    [SerializeField] int damage = 20;
    [SerializeField] float dashDuration = 0.5f;
    int beatCount = 0;
    bool clutter = false;

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 3.0f;
    Rigidbody2D rb;
    GameObject player;
    [SerializeField] LayerMask playerLayer;

    [Header("Effects")]
    [SerializeField] Color attackColor;
    [SerializeField] GameObject attackIndicator;
    [SerializeField]ParticleSystem telegraph;
    AttackIndicator aIndicate;
    TrailRenderer tRend;
    Color defaultColor;
    SpriteRenderer sRend;


    EnemyManager enemyManager;
    PulseManager pulseManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aIndicate = attackIndicator.GetComponent<AttackIndicator>();
        sRend = GetComponent<SpriteRenderer>();
        defaultColor = sRend.color;
        tRend = GetComponent<TrailRenderer>();
        tRend.emitting = false;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        //enemyManager.AddEnemy(this.gameObject);
        pulseManager = GameObject.FindGameObjectWithTag("RhythmManager").GetComponent<PulseManager>();
        pulseManager.AddEntity(this.gameObject, pulseManager.entitiesToPulse);
    }

    public override void Attack()
    {
        aIndicate.AttackFlash();
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
        foreach(Collider2D hitPlayer in hitPlayers)
        {
            hitPlayer.gameObject.GetComponent<Health>().TakeDamage(damage);
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


        tRend.emitting = true;
        rb.linearVelocity = direction * moveSpeed;


        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = Vector2.zero;
        tRend.emitting = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public override void AddToBeatCount()
    {
        if (active)
        {
            if (beatCount == 5)
            {
                telegraph.Play();
            }

            if (beatCount == 7)
            {
                sRend.color = attackColor;
                Attack();
                beatCount = 0;
            }
            else
            {
                sRend.color = defaultColor;
                beatCount++;
            }

            if (beatCount == 4)
            {
                StartCoroutine(DashTowardsPlayer());
            }
        }
       
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
