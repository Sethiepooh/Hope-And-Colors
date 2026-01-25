using System.Collections;
using UnityEngine;

public class Driller : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] GameObject scatterBomb;
    int beatCount = 0;
    Collider2D drillerCol;

    [Header("Movement Stats")]
    [SerializeField] float dashDuration = 0.5f;
    [SerializeField] float moveSpeed = 3.0f;
    Rigidbody2D rb;
    GameObject player;
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
        // enemyManager.AddEnemy(this.gameObject);
        pulseManager = GameObject.FindGameObjectWithTag("RhythmManager").GetComponent<PulseManager>();
        pulseManager.AddEntity(this.gameObject, pulseManager.entitiesToPulse);
        drillerCol = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    IEnumerator DashTowardsPlayer()
    {
        Instantiate(scatterBomb, transform.position, Quaternion.identity);
        drillerCol.enabled = false;
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
        drillerCol.enabled = true;
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


            if (beatCount < 4)
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
}
