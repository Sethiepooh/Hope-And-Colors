using System.Collections;
using UnityEngine;

public class ScatterBomb : MonoBehaviour
{
    [SerializeField] float timeToExplosion;
    [SerializeField] float blastRadius;
    [SerializeField] int blastDamage;


    [SerializeField] ProjectilePool projectilePool;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] LayerMask explosionTargetLayer;


    void Start()
    {
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(timeToExplosion);
        ScatterShot();
        BombDamage();
        Destroy(this.gameObject);
    }

    void BombDamage()
    {
        GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        effect.transform.localScale = new Vector3(blastRadius, blastRadius, blastRadius);
        Destroy(effect, 1f);

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, blastRadius, explosionTargetLayer);
        if (hitPlayers.Length > 0)
        {
            foreach (Collider2D player in hitPlayers)
            {
                if (player.CompareTag("Player"))
                {
                    player.GetComponent<Health>().TakeDamage(blastDamage);
                }
                else if (player.CompareTag("Obstacle"))
                {
                    player.GetComponent<Health>().TakeDamage(blastDamage);
                }
                else if (player.CompareTag("Bomb"))
                {
                    player.GetComponent<Health>().TakeDamage(blastDamage);
                }
            }
        }
    }

    void ScatterShot()
    {
        float angleStep = 360f / 8;
        float angle = 0f;
        for (int i = 0; i < 8; i++)
        {
            float projectileDirXPosition = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float projectileDirYPosition = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);
            Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
            Vector3 projectileMoveDirection = (projectileVector - transform.position).normalized;
            Projectile projectileInstance = projectilePool.GetProjectile(
               transform.position,
               Quaternion.LookRotation(Vector3.forward, projectileVector)
           );
            projectileInstance.Initialize(projectilePool, false, projectileMoveDirection);
            angle += angleStep;
        }
    }
}
