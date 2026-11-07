using System.Collections;
using UnityEngine;

public class BeetleBomb : MonoBehaviour
{
    public float explosionRadius = 5f;
    public int explosionDamage = 50;
    public float beetleSpeed = 2f;
    public float beetleLifetime = 5f;
    public float explosionDelay = 0.5f;
    public float despawnDelay = .5f;
    public float postExplosionDelay = 2;
    [SerializeField] Animator anim;
    bool despawn = false;

    static readonly int DespawnHash = Animator.StringToHash("Despawn");
    static readonly int ExplodeHash = Animator.StringToHash("Explode");
    static readonly int DashHash = Animator.StringToHash("Dashing");

    Coroutine explosionCoroutine;

    Rigidbody2D rb;
    float currentLifetime;
    bool isActive;
    bool isExploding;
    Vector2 direction;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        isActive = false;
    }

    public void Initialize(Transform spawnPosition, Vector2 dir)
    {
        if (explosionCoroutine != null)
        {
            explosionCoroutine = null;
        }
        transform.position = spawnPosition.position;
        direction = dir.normalized;
        isActive = true;
        currentLifetime = 0f;
    }

    private void Update()
    {
        if (!isActive || despawn) return;

        // Move the beetle forward
        anim.SetBool(DashHash, true);
        rb.linearVelocityX = direction.x * beetleSpeed;


        if (rb.linearVelocity.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // Check for lifetime expiration

        if (currentLifetime >= beetleLifetime)
        {
            if(explosionCoroutine == null)
                explosionCoroutine = StartCoroutine(ExplodeAfterDelay(explosionDelay));
        }
        else
        {
            currentLifetime += Time.deltaTime;
        }
    }

    IEnumerator ExplodeAfterDelay(float delay)
    {
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(delay);
        anim.SetBool(DashHash, false);
        anim.SetTrigger(ExplodeHash);
        // Detect enemies in the explosion radius
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                // Apply damage to the enemy
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(explosionDamage);
                }
            }
        }
        // Play explosion effect here (if any)
        // Destroy the beetle bomb after explosion
        yield return new WaitForSeconds(postExplosionDelay);
        StartCoroutine(DespawnBeetle());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(ExplodeAfterDelay(explosionDelay));
        }
    }

    IEnumerator DespawnBeetle()
    {
        despawn = true;
        anim.SetTrigger(DespawnHash);
        yield return new WaitForSeconds(despawnDelay);
        isActive = false;
        gameObject.SetActive(false);
    }
}
