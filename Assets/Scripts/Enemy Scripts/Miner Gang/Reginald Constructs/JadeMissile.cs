using UnityEngine;

public class JadeMissile : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 direction;
    GameObject player;
    [SerializeField] float speed = 10f;
    [SerializeField] int damage = 10;
    [SerializeField] Projectile onHitProjectiles;

    bool firing;

    public JadeMissile Initialize(GameObject player)
    {
        rb = GetComponent<Rigidbody2D>();
        this.player = player;
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
            var projectileInstance = Instantiate(onHitProjectiles, transform.position, Quaternion.identity);
            Vector2 projDirection = Quaternion.Euler(0, 0, i == 0 ? 45 : -45) * -direction;
            projectileInstance.direction = projDirection.normalized;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!firing)
        {
            Vector3 offset = player.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);
        }
        else
        {
            rb.linearVelocity = direction * speed;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
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
