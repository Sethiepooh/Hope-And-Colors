using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField] Health health;

    SpriteRenderer sRend;

    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        health.onDamageEvent += UpdateUI;
        health.onDamageEvent += ActivateHitStun;

        health.onHealEvent += UpdateUI;
    }

    public void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = health.GetHealthPercent();
        }
    }

    public void ActivateHitStun()
    {
        StartCoroutine(SetHitStun());
    }

    IEnumerator SetHitStun()
    {
        health.damagable = false;
        yield return new WaitForSeconds(.2f);
        Debug.Log("Player can be damaged again");
        health.damagable = true;
    }
}
