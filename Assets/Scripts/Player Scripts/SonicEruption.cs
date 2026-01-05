using System.Collections;
using UnityEngine;

public class SonicEruption : MonoBehaviour
{
    public AttackIndicator indicator;

    [Header("Damage Control")]
    [SerializeField] int damage = 15;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReleaseAttack()
    {
        indicator.AttackFlash();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy") || enemy.CompareTag("Boss") || enemy.CompareTag("Obstacle"))
            {
                enemy.GetComponent<Health>().TakeDamage(damage);
            }
        }
    }
}
