using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    [HideInInspector] public Vector2 direction;
    [SerializeField] Rigidbody2D rb;

    public bool fireFromPlayer = true;
    bool freeze = false;

    private ProjectilePool pool; // Reference to the pool

    // Called by the pool when the projectile is spawned
    public void Initialize(ProjectilePool pool, bool playerProj, Vector2 dir)
    {
        this.pool = pool;
        fireFromPlayer = playerProj;
        direction = dir.normalized;
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

        rb.linearVelocity = direction * speed;

        Vector3 offset = direction;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);
    }

    public void ToggleFreeze(bool b)
    {
        freeze = b;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Bomb"))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            Debug.Log("Projectile hit: " + collision.gameObject.name);
            ReturnToPool();
            return;
        }

        if (fireFromPlayer)
        {
            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
            {
                if (collision.gameObject.GetComponent<Health>().damagable)
                    collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            }
            Debug.Log("Projectile hit: " + collision.gameObject.name);
            ReturnToPool();
        }
        else
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            }
            Debug.Log("Projectile hit: " + collision.gameObject.name);
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (pool != null)
            pool.ReturnProjectile(this);
        else
            gameObject.SetActive(false); // Fallback if not pooled
    }
}
