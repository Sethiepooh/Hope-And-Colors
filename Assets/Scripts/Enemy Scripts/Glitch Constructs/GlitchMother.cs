
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class GlitchMother : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] Transform projectileSpawnOne;
    [SerializeField] Transform projectileSpawnTwo;
    [SerializeField] float fireInterval;

    Vector2 direction;

    [Header("Effects")]
    [SerializeField]ParticleSystem telegraph;
    [SerializeField] Animator anim;

    [Header("Teleport")]
    [SerializeField] float vanishDuration = 0.2f;   // length of the "teleporting away" clip
    [SerializeField] float appearDuration = 0.2f;   // length of the "appearing" clip
    [SerializeField] int maxTeleportAttempts = 20;

    bool isTeleporting;
    Coroutine teleportRoutine;

    static readonly int DespawnHash = Animator.StringToHash("Vanish");
    static readonly int AttackHash = Animator.StringToHash("Attacking");
    static readonly int AppearHash = Animator.StringToHash("Appear");


    // Update is called once per frame
    void Update()
    {
        if(player != null && !isTeleporting)
        {
            Vector3 offset = player.transform.position - transform.position;
            if(offset.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
            
    }

    public override void Attack()
    {
        anim.SetTrigger(AttackHash);
        StartCoroutine(FireWithDelay(fireInterval));
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
                StartTeleport();
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

    void StartTeleport()
    {
        if (isTeleporting) return;
        if (!TryFindTeleportLocation(out Vector2 destination)) return;

        teleportRoutine = StartCoroutine(TeleportRoutine(destination));
    }

    IEnumerator TeleportRoutine(Vector2 destination)
    {
        isTeleporting = true;

        // 1. Vanish
        anim.SetTrigger(DespawnHash);
        yield return new WaitForSeconds(vanishDuration);

        // 2. Move while invisible (last frame of the vanish clip should be fully hidden)
        transform.position = destination;

        // 3. Appear
        anim.SetTrigger(AppearHash);
        yield return new WaitForSeconds(appearDuration);

        isTeleporting = false;
        teleportRoutine = null;
    }

    IEnumerator FireWithDelay(float delay)
    {
        Projectile projectileOne = projectilePool.GetProjectile(
             projectileSpawnOne.position,
             Quaternion.LookRotation(Vector3.forward, (player.transform.position - transform.position).normalized)
         );
        projectileOne.Initialize(projectilePool, false, (player.transform.position - transform.position).normalized);

        yield return new WaitForSeconds(delay);

        Projectile projectileTwo = projectilePool.GetProjectile(
               projectileSpawnTwo.position,
               Quaternion.LookRotation(Vector3.forward, (player.transform.position - transform.position).normalized)
           );
        projectileTwo.Initialize(projectilePool, false, (player.transform.position - transform.position).normalized);
    }

    bool TryFindTeleportLocation(out Vector2 result)
    {
        Vector2 playerPos = player.transform.position;

        for (int i = 0; i < maxTeleportAttempts; i++)
        {
            float tpX = Random.Range(-4, 4);
            float tpY = Random.Range(-4, 4);
            if (tpX >= -1 && tpX <= 1) tpX += 3;
            if (tpY >= -1 && tpY <= 1) tpY += 3;

            Vector2 candidate = new Vector2(playerPos.x + tpX, playerPos.y + tpY);
            if (Physics2D.OverlapCircle(candidate, 0.5f) == null)
            {
                result = candidate;
                return true;
            }
        }

        result = default;
        return false;
    }

    void OnDisable()
    {
        // Don't get stuck mid-teleport if the enemy is disabled or pooled
        if (teleportRoutine != null) StopCoroutine(teleportRoutine);
        isTeleporting = false;
    }
}

