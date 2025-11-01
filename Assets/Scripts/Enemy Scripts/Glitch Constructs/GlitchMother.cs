using UnityEngine;

public class GlitchMother : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] int damage = 5;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] GameObject projectile;
    int beatCount = 0;
    bool slash = false;

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 3.0f;
    Rigidbody2D rb;
    GameObject player;

    EnemyManager enemyManager;
    PulseManager pulseManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void AddToBeatCount()
    {
        if(beatCount == 8)
        {
            Teleport();
            beatCount = 1;
        }
        else
        {
            beatCount++;
        }

        if(beatCount%2 == 0)
        {
            Attack();
        }
    }

    void Teleport()
    {
        Vector2 playerPos = player.transform.position;
        Vector2 teleportLocation = new Vector2((playerPos.x + 3), playerPos.y);   
    }
}
