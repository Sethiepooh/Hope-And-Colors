using Unity.VisualScripting;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    RespawnManager r_Man;
    bool active;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        r_Man = GameObject.FindWithTag("RespawnManager").GetComponent<RespawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !active)
        {
            r_Man.SetSpawnIndex(this.gameObject);
            active = true;
        }
    }
}
