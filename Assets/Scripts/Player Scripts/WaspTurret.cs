using System.Collections;
using UnityEngine;

public class WaspTurret : MonoBehaviour
{
    [Header("Turret Settings")]
    public float projectileSpeed = 10f;
    public float fireIntervalBeats = 2f; // Fires every other beat
    public int maxBeats = 16;
    public float fireInterval;
    public Transform attackPointOne;
    public Transform attackPointTwo;
    public float despawnDelay;
    bool despawn = false;

    private int currentBeats = 0;
    private bool isActive = false;
    public ProjectilePool projectilePool; // Reference to the projectile pool
    [SerializeField] Animator anim;

    static readonly int DespawnHash = Animator.StringToHash("Vanish");
    static readonly int AttackHash = Animator.StringToHash("Attacking");
    static readonly int AppearHash = Animator.StringToHash("Appear");

    // Called by PlayerAttack when summoning the turret
    public void Initialize(Transform spawn)
    {
        despawn = false;
        anim.SetTrigger(AppearHash);
        currentBeats = 0;
        transform.position = spawn.position;
        isActive = true;
    }

    // This should be called externally on every beat
    public void OnBeat()
    {
        if (!isActive) return;

        currentBeats++;

        // Fire every other beat (on even beats)
        if (currentBeats % fireIntervalBeats == 0)
        {
            FireAtNearestEnemy();
        }

        if (currentBeats >= maxBeats)
        {
            StartCoroutine(DespawnTurret());
        }
    }

    private void FireAtNearestEnemy()
    {
        GameObject target = FindClosestEnemy();
        if (target == null) return;

        Vector2 direction = (target.transform.position - transform.position).normalized;

        anim.SetTrigger(AttackHash);
        StartCoroutine(FireWithDelay(fireInterval, direction));      
    }

    IEnumerator FireWithDelay(float delay, Vector2 dir)
    {
        Projectile projectileOne = projectilePool.GetProjectile(
             attackPointOne.position,
             Quaternion.LookRotation(Vector3.forward, dir)
         );
        projectileOne.Initialize(projectilePool, true, dir);

        yield return new WaitForSeconds(delay);

        Projectile projectileTwo = projectilePool.GetProjectile(
               attackPointTwo.position,
               Quaternion.LookRotation(Vector3.forward, dir)
           );
        projectileTwo.Initialize(projectilePool, true, dir);
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(enemy.transform.position, currentPos);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }
        return closest;
    }

    IEnumerator DespawnTurret()
    {
        despawn = true;
        anim.SetTrigger(DespawnHash);
        yield return new WaitForSeconds(despawnDelay);
        isActive = false;
        gameObject.SetActive(false);
    }
}
