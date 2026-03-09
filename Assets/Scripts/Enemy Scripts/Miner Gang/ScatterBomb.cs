using System.Collections;
using UnityEngine;

public class ScatterBomb : ScatterShot
{
    [Header("Bomb Settings")]
    [SerializeField] float timeToExplosion;
    [SerializeField] float blastRadius;
    [SerializeField] int blastDamage;

    [Header("Explosion Effect")]
    [SerializeField] GameObject explosionEffect;
    [SerializeField] LayerMask explosionTargetLayer;


    void Start()
    {
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(timeToExplosion);
        FireScatterShot();
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
}
