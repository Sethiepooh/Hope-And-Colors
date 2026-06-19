using System.Collections;
using UnityEngine;

public class Bomb : BreakableObjectBase
{
    [Header("Bomb settings")]
    [SerializeField] ParticleSystem blastParticles;
    public float blastRadius;
    Collider2D col;

    [SerializeField] GameObject projectile;
    [SerializeField] int projectileNum;

    bool exploding;

    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    public void ScatterShot()
    {
        Debug.Log("ScatterShot");
        float angleStep = 360f / (float)projectileNum;
        float angle = 0f;
        for (int i = 0; i < projectileNum; i++)
        {
            float projectileDirXPosition = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float projectileDirYPosition = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);
            Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
            Vector3 projectileMoveDirection = (projectileVector - transform.position).normalized;
            GameObject tmpObj = Instantiate(projectile, transform.position, Quaternion.identity);
            tmpObj.GetComponent<Projectile>().direction = projectileMoveDirection;
            Debug.Log("ScatterShot");
            angle += angleStep;
        }
    }

    public override void OnDeath()
    {
        if(exploding) return;

        StartCoroutine(DeathBlast());
        ManagerDeathEvent.Invoke();
        exploding = true;

    }

    IEnumerator DeathBlast()
    {
        blastParticles.Play();
        deathParticles.Play();
        yield return new WaitForSeconds(2f);
        col.enabled = false;
        ScatterShot();
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, blastRadius);
        foreach (Collider2D target in targets)
        {
            var health = target.GetComponent<Health>();

            if (health != null)
                health.TakeDamage(20);

        }
        var aIndicate = transform.GetChild(0).GetComponent<AttackIndicator>();
        sRend.enabled = false;
        blastParticles.Stop();
        aIndicate.AttackFlash();
        yield return new WaitForSeconds(.5f);
        this.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
