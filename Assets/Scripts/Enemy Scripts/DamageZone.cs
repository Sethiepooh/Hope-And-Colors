using System.Collections;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] LayerMask damageLayer;

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
