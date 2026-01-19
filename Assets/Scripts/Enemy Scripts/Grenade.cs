using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f;
    public float speedDecay = 0.1f;
    public int damage = 20;
    [HideInInspector] public Vector2 direction;
    [SerializeField] Rigidbody2D rb;

    [Header("Explosion Settings")]
    [SerializeField] float explosionDelay = 2f;
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] GameObject explosionIndicator;

    public bool fireFromPlayer = true;
    bool freeze = false;

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (freeze)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        else
        {
            //Projectile movement with speed decay
            rb.linearVelocity = direction * speed;
            speed -= speedDecay;
            if (speed < 0f) speed = 0f;

            // Rotate to face movement direction
            Vector3 offset = direction;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);

            // Handle explosion countdown
            explosionDelay -= Time.fixedDeltaTime;
            if (explosionDelay <= 0f)
            {
                Explode();
            }
        }
    }

    public void ToggleFreeze(bool b)
    {
        freeze = b;
    }

    public void Explode()
    {
        GameObject explosion = Instantiate(explosionIndicator, transform.position, Quaternion.identity);
        explosion.transform.localScale = new Vector3(explosionRadius, explosionRadius, 1f);
        Destroy(explosion, 0.5f);
        // Add explosion effect here (e.g., particle system, sound)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") || hitCollider.CompareTag("Obstacle"))
            {
                hitCollider.GetComponent<Health>().TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}
