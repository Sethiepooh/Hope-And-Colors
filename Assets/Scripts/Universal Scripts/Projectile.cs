using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    [HideInInspector] public Vector2 direction;
    [SerializeField] Rigidbody2D rb;

    public bool fireFromPlayer = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;

        Vector3 offset = direction;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            Destroy(gameObject);
        }

        if (fireFromPlayer)
        {
            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
            {
                if(collision.gameObject.GetComponent<Health>().damagable)
                    collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            }
            Destroy(gameObject);
        }
       
       
    }
}
