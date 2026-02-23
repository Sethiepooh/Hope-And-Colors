
using UnityEngine;


public class GlitchMother : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] Transform projectileSpawn;
    [SerializeField] GameObject projectile;
    int beatCount = 0;

    [Header("Movement Stats")]
    Rigidbody2D rb;
    GameObject player;
    Vector2 direction;

    EnemyManager enemyManager;
    PulseManager pulseManager;
    [Header("Effects")]
    [SerializeField]ParticleSystem telegraph;

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
        var projectileInstance = Instantiate(projectile, projectileSpawn.position, Quaternion.identity);
        projectileInstance.GetComponent<Projectile>().direction =  (player.transform.position - transform.position).normalized;  
    }

    public override void AddToBeatCount()
    {
        if (active)
        {
            if (beatCount == 8)
            {
                beatCount = 0;
            }

            if (beatCount == 7)
            {
                Teleport();
                beatCount++;
            }
            else
            {
                beatCount++;
            }

            if (beatCount == 1 || beatCount == 3)
            {
                telegraph.Play();
            }

            if (beatCount % 2 == 0 && beatCount < 5)
            {
                Attack();
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
        Collider2D intersectingObjects = Physics2D.OverlapCircle(teleportLocation, 0.5f);
        if (intersectingObjects == null)
        {
            this.transform.position = teleportLocation;
            return;
        }
        else
        {
            Debug.Log("Teleport location obstructed, trying again...");
            Teleport();
        }
            
    }
}
