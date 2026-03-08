using System.Collections;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] LayerMask damageLayer;
    [SerializeField] bool decaying;

    [SerializeField] float decayTime;
    float decayTimer;

    private void Update()
    {
        if (decaying)
        {
            decayTimer += Time.deltaTime;
            if (decayTimer >= decayTime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & damageLayer) != 0)
        {
            StartCoroutine(DamageCoroutine(collision.gameObject));
        }
    }

    IEnumerator DamageCoroutine(GameObject target)
    {
        target.GetComponent<Health>().TakeDamage(damage);
        yield return new WaitForSeconds(1f);
    }
}
