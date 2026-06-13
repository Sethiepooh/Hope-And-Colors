using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Turret : EnemyBase
{
    [SerializeField] GameObject projectile;
    [SerializeField] Transform firePoint;
    [SerializeField] List<GameObject> generators = new List<GameObject>();

    Vector3 playerPos;
    public TurretType intensity;
    public enum TurretType
    {
        Easy,
        Medium,
        Hard
    }

    // Update is called once per frame
    void Update()
    {
        if(!active) return;
        playerPos = player.transform.position;
        Vector3 offset = playerPos - transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);
    }

    public void AddGenerator(GameObject generator)
    {
        generators.Add(generator);
        switch (generators.Count)
        {
            case 1:
                intensity = TurretType.Easy;
                break;
            case 2:
                intensity = TurretType.Medium;
                break;
            case 3:
                intensity = TurretType.Hard;
                break;
        }
    }

    public void DeactivateTurret()
    {
        int activeGenerators = 0;
        foreach (GameObject generator in generators)
        {
            if(generator.activeInHierarchy)
            {
                activeGenerators++;
            }
        }

        if(activeGenerators == 0)
        {
            active = false;
            health.onDeathEvent.Invoke();
        }
        else
        {
            switch (activeGenerators)
            {
                case 1:
                    intensity = TurretType.Easy;
                    break;
                case 2:
                    intensity = TurretType.Medium;
                    break;
                case 3:
                    intensity = TurretType.Hard;
                    break;
            }
            Debug.Log("Turret deactivated, remaining generators: " + activeGenerators + "New Intensity: " + intensity);
        }
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
        //Debug.Log("Turret beat count: " + beatCount);

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
