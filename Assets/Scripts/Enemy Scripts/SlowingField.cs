using UnityEngine;

public class SlowingField : MonoBehaviour
{
    [SerializeField] float slowSpeed = 5f;
    [SerializeField] PlayerMovement playerMovement;
    bool playerInField = false;
    bool initialized = false;
    public void Initialize(PlayerMovement pMove, float speed)
    {
        playerMovement = pMove;
        slowSpeed = speed;
        initialized = true;
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
        if (collision.CompareTag("Player") && initialized)
        {
            playerMovement.Slowdown(slowSpeed);
            playerInField = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && initialized)
        {
            playerMovement.Speedup();
            playerInField = false;
        }
       
    }
}
