using UnityEngine;

public class SeekerProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public int damage = 10;
    [SerializeField] Rigidbody2D rb;
    [HideInInspector] public GameObject target;
    [HideInInspector] public Vector2 intialVelocity;

    [Header("Explosion Settings")]
    [SerializeField] float explosionRadius = 2f;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] GameObject projectileObject;
    bool exploded;

    [Header("Timing Settings")]
    [SerializeField] float initialDelay = 0.5f;
    float delayTimer = 0f;
    [SerializeField] float lifetime = 5f;
    float timer = 0f;

    bool freeze = false;
    Collider2D col;

    // Called by the pool when the projectile is spawned
    public void Initialize(GameObject target, Vector2 initVel)
    {
        this.target = target;
        this.intialVelocity = initVel;
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        freeze = false;
    }

    void FixedUpdate()
    {
        if (freeze)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if(exploded)
        {
            return;
        }

        if(delayTimer < initialDelay)
        {
            delayTimer += Time.fixedDeltaTime;
            rb.linearVelocity = intialVelocity * speed;
            return;
        }

        if (timer < lifetime)
        {
            timer += Time.fixedDeltaTime;
            Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;

            rb.linearVelocity = direction * speed;

            Vector3 offset = direction;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);
        }
        else
        {
            Explode();
            exploded = true;
        }


    }

    public void ToggleFreeze(bool b)
    {
        freeze = b;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Bomb") || collision.gameObject.CompareTag("Player"))
        {
            Explode();
            exploded = true;
        }
    }

    public void Explode()
    {
        rb.linearVelocity = Vector2.zero;
        explosionEffect.SetActive(true);
        projectileObject.SetActive(false);
        col.enabled = false;
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach(Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player") || hit.CompareTag("Obstacle") || hit.CompareTag("Bomb"))
            {
                hit.GetComponent<Health>().TakeDamage(damage);
            }
        }
        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
