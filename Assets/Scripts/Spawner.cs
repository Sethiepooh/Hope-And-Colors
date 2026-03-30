using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] InspirationPickup spawnPrefab;

    GameObject player;
    PlayerAttack playerAttack;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
        }
    }

    public void Spawn()
    {
        InspirationPickup spawnedObject = Instantiate(spawnPrefab, transform.position, Quaternion.identity);
        spawnedObject.Initialize(player, playerAttack);
    }
}
