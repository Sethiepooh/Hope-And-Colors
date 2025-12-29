using System.Collections;
using UnityEngine;

public class SonicEruption : MonoBehaviour
{
    public AttackIndicator indicator;

    [Header("Size Control")]
    [SerializeField] float sizeMultiplier = .2f;
    [SerializeField] float maxSize = 5f;

    [Header("Damage Control")]
    [SerializeField] int damage = 15;
    [SerializeField] int maxDamage = 25;
    int currentDamage;

    Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartExpansion()
    {
        StartCoroutine(ExpandSonicEruption());
    }

    IEnumerator ExpandSonicEruption()
    {
        transform.localScale = originalScale;
        currentDamage = damage;

        while (transform.localScale.x < maxSize)
        {
            transform.localScale += originalScale * sizeMultiplier * Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(maxSize, maxSize, maxSize);
        currentDamage = maxDamage;
    }

    public void ReleaseAttack()
    {
        StopCoroutine(ExpandSonicEruption());
        indicator.AttackFlash();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy") || enemy.CompareTag("Boss"))
            {
                enemy.GetComponent<Health>().TakeDamage(currentDamage);
            }
        }
    }
}
