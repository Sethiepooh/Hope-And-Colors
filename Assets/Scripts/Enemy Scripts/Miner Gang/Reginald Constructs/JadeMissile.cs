using UnityEngine;

public class JadeMissile : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 direction;
    GameObject player;
    [SerializeField] float speed = 10f;
    [SerializeField] int damage = 10;
    [SerializeField] LayerMask damageLayer;
    [SerializeField] ProjectilePool projectilePool; // Add this line
    [SerializeField] int projectilesOnHit = 3;
    [SerializeField] Transform spawnPoint;

    bool firing;

    public JadeMissile Initialize(GameObject player, ProjectilePool pool)
    {
        rb = GetComponent<Rigidbody2D>();
        this.player = player;
        this.projectilePool = pool; // Set the pool reference
        return this;
    }

    public void Fire(Vector2 direction)
    {
        this.direction = direction.normalized;
        firing = true;
    }

    public void SpawnProjectiles(int count)
    {
        if (count < 1) return;

        Vector2 spawn = spawnPoint != null ? spawnPoint.position : transform.position;

        float angleStep = 360f / count;
        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep;
            Vector2 projDirection = Quaternion.Euler(0, 0, angle) * -direction;
            Projectile projectileInstance = projectilePool.GetProjectile(
                spawn,
                Quaternion.LookRotation(Vector3.forward, projDirection)
            );
            projectileInstance.Initialize(projectilePool, false, projDirection.normalized);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!firing)
        {
            Vector3 offset = player.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);
        }
        else
        {
            rb.linearVelocity = direction * speed;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & damageLayer) != 0)
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            SpawnProjectiles(projectilesOnHit);
            Destroy(gameObject);
        }
    }
}
