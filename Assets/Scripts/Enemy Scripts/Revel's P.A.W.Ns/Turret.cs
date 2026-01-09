using UnityEngine;

public class Turret : EnemyBase
{
    [SerializeField] GameObject projectile;
    [SerializeField] Transform firePoint;

    Vector3 playerPos;
    GameObject player;
    int beatCount = 0;
    public TurretType intensity;

    public enum TurretType
    {
        Easy,
        Medium,
        Hard
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(!active) return;
        playerPos = player.transform.position;
        Vector3 offset = playerPos - transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);
    }

    public void DeactivateTurret()
    {
        active = false;
    }

    public override void Attack()
    {
        var proj = Instantiate(projectile, firePoint.position, Quaternion.identity);
        Vector3 trajectory = playerPos - transform.position;
        proj.GetComponent<Projectile>().direction = trajectory.normalized;
    }

    public override void AddToBeatCount()
    {
        beatCount++;

        if (active)
        {
            if(intensity == TurretType.Easy)
            {
                if(beatCount %4 == 0)
                {
                    Attack();
                }
            }
            else if(intensity == TurretType.Medium)
            {
                if(beatCount %2 == 0)
                {
                    Attack();
                }
            }
            else if(intensity == TurretType.Hard)
            {
                Attack();
            }
        }
    }
}
