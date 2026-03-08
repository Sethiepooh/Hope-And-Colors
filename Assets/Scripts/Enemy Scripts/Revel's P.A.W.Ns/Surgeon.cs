using UnityEngine;

public class Surgeon : EnemyBase
{

    [Header("Attack Stats")]
    [SerializeField] Transform projectileSpawn;
    [SerializeField] ProjectilePool projectilePool;
    [SerializeField] GameObject grenade;
    int beatCount = 0;

    [Header("Movement Stats")]
    Rigidbody2D rb;
    GameObject player;
    Vector2 direction;

    EnemyManager enemyManager;
    PulseManager pulseManager;
    [Header("Effects")]
    [SerializeField] ParticleSystem telegraph;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        //enemyManager.AddEnemy(this.gameObject);
        pulseManager = GameObject.FindGameObjectWithTag("RhythmManager").GetComponent<PulseManager>();
        pulseManager.AddEntity(this.gameObject, pulseManager.entitiesToPulse);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = player.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);
    }

    public override void Attack()
    {
        Projectile projectileInstance = projectilePool.GetProjectile(
               transform.position,
               Quaternion.LookRotation(Vector3.forward, (player.transform.position - transform.position).normalized)
           );
        projectileInstance.Initialize(projectilePool, false, (player.transform.position - transform.position).normalized);
    }

    public void ThrowGrenade()
    {
        var grenadeInstance = Instantiate(grenade, projectileSpawn.position, Quaternion.identity);
        grenadeInstance.GetComponent<Grenade>().direction = (player.transform.position - transform.position).normalized;
    }

    public override void AddToBeatCount()
    {
        if (active)
        {
            beatCount++;
            if(beatCount == 11)
            {
                telegraph.Play();
            }

            if (beatCount == 12)
            {
                Teleport();
                beatCount = 0;
            }

            if (beatCount < 5 && beatCount > 0)
            {
                Attack();
            }

            if(beatCount > 5 && beatCount < 9 && beatCount % 2 == 0)
            {
                ThrowGrenade();
            }
        }

    }

    void Teleport()
    {
        float tpX = Random.Range(-4, 4);
        float tpY = Random.Range(-4, 4);
        if (tpX >= -1 && tpX <= 1)
            tpX += 3;
        if (tpY >= -1 && tpY <= 1)
            tpY += 3;

        Vector2 playerPos = player.transform.position;
        Vector2 teleportLocation = new Vector2((playerPos.x + tpX), (playerPos.y + tpY));
        this.transform.position = teleportLocation;
    }
}
