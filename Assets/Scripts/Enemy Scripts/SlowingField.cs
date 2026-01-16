using UnityEngine;

public class SlowingField : MonoBehaviour
{
    PlayerMovement playerMovement;
    public void Initialize(PlayerMovement pMove)
    {
        playerMovement = pMove;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
