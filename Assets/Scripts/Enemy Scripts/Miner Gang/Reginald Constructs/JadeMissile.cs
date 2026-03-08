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

    public void SpawnProjectiles()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector2 projDirection = Quaternion.Euler(0, 0, i == 0 ? 45 : -45) * -direction;
            // Use the pool to get a projectile
            Projectile projectileInstance = projectilePool.GetProjectile(
                transform.position,
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
            SpawnProjectiles();
            Destroy(gameObject);
        }
    }
}
