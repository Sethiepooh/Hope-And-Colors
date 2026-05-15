using UnityEngine;

public class Turret : EnemyBase
{
    [SerializeField] GameObject projectile;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject[] generators;

    Vector3 playerPos;
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
        foreach (GameObject generator in generators)
        {
            if(generator.activeInHierarchy)
            {
                return;
            }
        }
        active = false;
        transform.parent.gameObject.SetActive(false);
    }

    public override void Attack()
    {
        Projectile projectileInstance = projectilePool.GetProjectile(
                firePoint.position,
                Quaternion.LookRotation(Vector3.forward, (player.transform.position - transform.position).normalized)
            );
        projectileInstance.Initialize(projectilePool, false, (player.transform.position - transform.position).normalized);
    }

    public override void AddToBeatCount()
    {
        beatCount++;
        Debug.Log("Turret beat count: " + beatCount);

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
