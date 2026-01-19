using UnityEngine;

public class SlowingField : MonoBehaviour
{
    [SerializeField] float slowSpeed = 5f;
    [SerializeField] PlayerMovement playerMovement;
    bool playerInField = false;
    public void Initialize(PlayerMovement pMove, float speed)
    {
        playerMovement = pMove;
        slowSpeed = speed;
    }

    public void DeactivateField()
    {
        if (playerInField)
        {
            playerMovement.slowed = false;
        }
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") == false) return;
        playerMovement.Slowdown(slowSpeed);
        playerInField = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == false) return;
        playerMovement.Speedup();
        playerInField = false;
    }
}
