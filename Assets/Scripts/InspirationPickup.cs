using UnityEngine;

public class InspirationPickup : MonoBehaviour
{
    GameObject player;
    PlayerAttack playerAttack;
    [SerializeField] float pickupRange = 1.5f;
    [SerializeField] float inspirationAmount = 20f;
    [SerializeField] float floatSpeed = 5;
    [SerializeField] Rigidbody2D rb;



    public void Initialize(GameObject player, PlayerAttack attack)
    {
        this.player = player;
        this.playerAttack = attack;
        FindRandomDirection();
    }

    void FindRandomDirection()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        rb.linearVelocity = randomDirection * floatSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        // Check distance to player
        if(Vector3.Distance(transform.position, player.transform.position) <= pickupRange)
        {
            rb.linearVelocity = player.transform.position - transform.position * floatSpeed;
        }
        else
        {
            rb.linearVelocity -= rb.linearVelocity * Time.deltaTime; // Gradually slow down when not near the player
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (player == null) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            playerAttack.AddToCurrentInspiration(inspirationAmount); // Add inspiration to the player
            Destroy(gameObject); // Destroy the pickup after collecting
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
