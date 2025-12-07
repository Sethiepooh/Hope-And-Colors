using Unity.VisualScripting;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    RespawnManager r_Man;
    public bool active;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        r_Man = GameObject.FindWithTag("RespawnManager").GetComponent<RespawnManager>();
        SwitchColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchColor()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (active)
        {
            sr.color = Color.green;
        }
        else
        {
            sr.color = Color.grey;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !active)
        {
            r_Man.SetSpawnIndex(this.gameObject);
        }
    }
}
