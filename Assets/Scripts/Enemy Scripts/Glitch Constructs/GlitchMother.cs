
using UnityEngine;


public class GlitchMother : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] Transform projectileSpawn;

    Vector2 direction;

    [Header("Effects")]
    [SerializeField]ParticleSystem telegraph;


    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            Vector3 offset = player.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);
        }
            
    }

    public override void Attack()
    {
        Projectile projectileInstance = projectilePool.GetProjectile(
               transform.position,
               Quaternion.LookRotation(Vector3.forward, (player.transform.position - transform.position).normalized)
           );
        projectileInstance.Initialize(projectilePool, false, (player.transform.position - transform.position).normalized);
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
