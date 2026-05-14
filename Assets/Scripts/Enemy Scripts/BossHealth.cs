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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health.onDamageEvent += UpdateUI;
        health.onDamageEvent += ActivateHitStun;

        health.onHealEvent += UpdateUI;

        health.onDeathEvent += HandleBossDeath;
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

    public void HandleBossDeath()
    {
        StartCoroutine(BossDeath());
    }
    IEnumerator BossDeath()
    {
        sRend.enabled = false;
        var col = gameObject.GetComponent<Collider2D>();
        col.enabled = false;
        var rb = gameObject.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector3.zero;
        health.PlayDeathParticles();
        NextLevel nextLevel = GameObject.FindFirstObjectByType<NextLevel>();
        nextLevel.LoadNextLevel();

        yield return new WaitForSeconds(.5f);
        this.gameObject.SetActive(false);
    }
}
