using UnityEngine;

public class Crystal : BreakableObjectBase
{
    [SerializeField] InspirationPickup spawnPrefab;
    [SerializeField] Collider2D col;
    [SerializeField] int minSpawnAmount = 2;    
    [SerializeField] int maxSpawnAmount = 5;
    int spawnAmount;

    GameObject player;
    PlayerAttack playerAttack;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        col = GetComponent<Collider2D>();
        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
        }

        spawnAmount = Random.Range(minSpawnAmount, maxSpawnAmount + 1);
    }

    public void Spawn()
    {
        InspirationPickup spawnedObject = Instantiate(spawnPrefab, transform.position, Quaternion.identity);
        spawnedObject.Initialize(player, playerAttack);
    }

    public override void OnDeath()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Spawn();
        }
        col.enabled = false;
        if (deathParticles != null)
            deathParticles.Play();
        Destroy(gameObject);
    }
}
