using UnityEngine;

public class FallingHazard : MonoBehaviour
{
    [SerializeField] float fallDelay;
    float fallTimer;
    [SerializeField] int damage;
    [SerializeField] float size;
    [SerializeField] GameObject telegraph;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject hazard;

    public void Initialize()
    {
        GameObject telegraphInstance = Instantiate(telegraph, transform.position, Quaternion.identity);
        telegraphInstance.transform.localScale = Vector3.one * size;
    }

    // Update is called once per frame
    void Update()
    {
        fallTimer += Time.deltaTime;
        if (fallTimer >= fallDelay)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, size, playerLayer);
            if (hit != null)
            {
                hit.GetComponent<Health>().TakeDamage(damage);
            }
            Instantiate(hazard, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
