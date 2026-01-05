using System.Collections;
using UnityEngine;

public class HeartthrobSoloProjectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float damage = 25f;
    [SerializeField] float lifetime = 5f;
    [HideInInspector] public GameObject player;
    [HideInInspector]public Vector3 direction;
    [HideInInspector] public PlayerAttack playerAttack;
    Vector3 returnTarget;
    Rigidbody2D rb;
    bool returning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(ReturnAfterTimer());
    }

    public void Initialize(PlayerAttack pAttack, Vector3 dir, GameObject player)
    {
        this.player = player;
        this.direction = dir;
        this.playerAttack = pAttack;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!returning)
        {
            rb.linearVelocity = direction.normalized * speed;
        }
        else
        {
            returnTarget = player.transform.position;
            Vector3 trajectory = returnTarget - transform.position;

            rb.linearVelocity = trajectory.normalized * speed;
        }
    }

    IEnumerator ReturnAfterTimer()
    {
        yield return new WaitForSeconds(lifetime);
        returning = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Obstacle"))
        {
            Health hp = collision.GetComponent<Health>();
        }

        if (!returning)
        {
            if (collision.CompareTag("Wall"))
            {
                returning = true;
            }          
        }
        else
        {
            if (collision.CompareTag("Player"))
            {
                Destroy(gameObject);
                playerAttack.ResetHearttrhobSolo();
            }
        }
       
    }
}
