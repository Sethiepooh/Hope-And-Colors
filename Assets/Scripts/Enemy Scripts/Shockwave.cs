using UnityEngine;

public class Shockwave : MonoBehaviour
{
    int beatCount = 0;
    [SerializeField] AttackIndicator aIndicate;
    [SerializeField]ParticleSystem telegraph;
    public float range = 5f;
    AlixBoss bossRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossRef = GameObject.FindGameObjectWithTag("Boss").GetComponent<AlixBoss>();
        telegraph.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToBeatCount()
    {
        beatCount++;
        if (beatCount == 2)
        {
            aIndicate.AttackFlash();
            foreach (Collider2D hit in Physics2D.OverlapCircleAll(transform.position, range))
            {
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<Health>().TakeDamage(10);
                }
            }
        }

        if(beatCount >= 4)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
