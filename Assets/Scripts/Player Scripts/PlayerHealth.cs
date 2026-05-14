using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField] Health health;

    SpriteRenderer sRend;

    RespawnManager r_Man;


    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
        r_Man = GameObject.FindWithTag("RespawnManager").GetComponent<RespawnManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health.onDamageEvent += UpdateUI;
        health.onDamageEvent += ActivateHitStun;
        
        health.onHealEvent += UpdateUI;

        health.onDeathEvent += HandlePlayerDeath;
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
        StartCoroutine(SetPlayerHitStun());
    }

    IEnumerator SetPlayerHitStun()
    {
        health.damagable = false;
        yield return new WaitForSeconds(.2f);
        Debug.Log("Player can be damaged again");
        health.damagable = true;
    }

    public void HandlePlayerDeath()
    {
        //handle player death
        sRend.enabled = false;
        var col = gameObject.GetComponent<Collider2D>();
        col.enabled = false;
        var rb = gameObject.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector3.zero;
        health.PlayDeathParticles();
        r_Man.ResetPlayer();
    }
}
