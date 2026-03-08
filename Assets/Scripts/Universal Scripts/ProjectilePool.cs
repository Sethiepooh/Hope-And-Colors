using UnityEngine;
using System.Collections.Generic;


public class ProjectilePool : MonoBehaviour
{
    [Header("Pool Settings")]
    public Projectile projectilePrefab;
    public int poolSize = 20;

    private Queue<Projectile> pool = new Queue<Projectile>();

    void Awake()
    {
        // Pre-instantiate projectiles and add them to the pool
        for (int i = 0; i < poolSize; i++)
        {
            Projectile proj = Instantiate(projectilePrefab, transform);
            proj.gameObject.SetActive(false);
            pool.Enqueue(proj);
        }
    }

    // Get a projectile from the pool
    public Projectile GetProjectile(Vector3 position, Quaternion rotation)
    {
        Projectile proj;
        if (pool.Count > 0)
        {
            proj = pool.Dequeue();
        }
        else
        {
            // Optionally expand the pool if needed
            proj = Instantiate(projectilePrefab, transform);
        }

        proj.transform.position = position;
        proj.transform.rotation = rotation;
        proj.gameObject.SetActive(true);
        return proj;
    }

    // Return a projectile to the pool
    public void ReturnProjectile(Projectile proj)
    {
        proj.gameObject.SetActive(false);
        pool.Enqueue(proj);
    }
}
