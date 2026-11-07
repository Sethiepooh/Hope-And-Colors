using System.Collections;
using UnityEngine;

public class GlitchFather : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] float attackRange = 5f;
    [SerializeField] int damage = 20;
    [SerializeField] float dashDuration = 0.5f;
    bool clutter = false;

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] LayerMask playerLayer;

    [Header("Effects")]
    [SerializeField] GameObject attackIndicator;
    [SerializeField]ParticleSystem telegraph;
    AttackIndicator aIndicate;
    [SerializeField] Animator anim;

    static readonly int DespawnHash = Animator.StringToHash("Despawn");
    static readonly int ExplodeHash = Animator.StringToHash("Explode");
    static readonly int DashHash = Animator.StringToHash("Dashing");


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aIndicate = attackIndicator.GetComponent<AttackIndicator>();
    }

    public override void Attack()
    {    
        //aIndicate.AttackFlash();
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
        foreach(Collider2D hitPlayer in hitPlayers)
        {
            hitPlayer.gameObject.GetComponent<Health>().TakeDamage(damage);
        }
    }

    IEnumerator DashTowardsPlayer()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        Vector2 direction;
        if (!clutter)
        {
            Vector2 playerPos = player.transform.position;
            direction = (playerPos - rb.position).normalized;
        }
        else
        {
            int randomInt = Random.Range(0, 2);
            Vector2 playerPos = player.transform.position;
            direction = (playerPos - rb.position).normalized;
            if (randomInt == 0)
                direction = -direction;
        }


        tRend.emitting = true;
        rb.linearVelocity = direction * moveSpeed;

        Vector3 offset = player.transform.position - transform.position;
        if (offset.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        anim.SetBool(DashHash, true);
        yield return new WaitForSeconds(dashDuration);
        anim.SetBool(DashHash, false);
        rb.linearVelocity = Vector2.zero;
        tRend.emitting = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public override void AddToBeatCount()
    {
        if (active)
        {
            if (beatCount == 5)
            {
                telegraph.Play();
            }

            if (beatCount == 6)
            {
                anim.SetTrigger(ExplodeHash);
            }
            if (beatCount == 7)
            {
                sRend.color = attackColor;
                Attack();
                beatCount = 0;
            }
            else
            {
                sRend.color = defaultColor;
                beatCount++;
            }

            if (beatCount == 4)
            {
                StartCoroutine(DashTowardsPlayer());
            }
        }
       
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
