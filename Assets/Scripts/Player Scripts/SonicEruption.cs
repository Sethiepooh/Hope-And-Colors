using System.Collections;
using UnityEngine;

public class SonicEruption : MonoBehaviour
{
    Vector3 direction;
    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer sRend;

    [Header("Settings")]
    [SerializeField] int damage = 50;
    [SerializeField] float speed = 15;
    [SerializeField] float lifetime = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sRend = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Vector3 dir)
    {
        this.direction = dir;

        if(dir.x < 0)
        {
            sRend.flipX = true;
        }
        StartCoroutine(DestroyAfterLifetime());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = direction.normalized * speed;
    }
  
    private void OnTriggerEnter2D(Collider2D enemy)
    {
        if(enemy.CompareTag("Enemy") || enemy.CompareTag("Boss") || enemy.CompareTag("Obstacle") || enemy.CompareTag("Shield"))
        {
            enemy.GetComponent<Health>().TakeDamage(damage);
        }
    }

    IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
    }
}
