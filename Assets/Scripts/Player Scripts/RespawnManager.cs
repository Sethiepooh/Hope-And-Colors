using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class RespawnManager : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>() ;
    [SerializeField] float resetSpeed;
    GameObject player;
    int spawnIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpawnIndex(GameObject spawnPoint)
    {
        spawnIndex = spawnPoints.IndexOf(spawnPoint);
    }

    public void ResetPlayer()
    {
        player.GetComponent<PlayerAttack>().currentInspiration = 0;
        player.GetComponent<Health>().Heal(100);
        StartCoroutine(MovePlayerToSpawnPoint(spawnIndex));
    }

    IEnumerator MovePlayerToSpawnPoint(int i)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        Vector2 spawnPos = spawnPoints[i].transform.position;
        Vector2 playerPos = player.transform.position;
        Vector2 direction = spawnPos - playerPos;

        if(Vector2.Distance(spawnPos, playerPos) > 1)
        {
            playerRb.linearVelocity = direction * resetSpeed;
        }
        player.GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<Collider2D>().enabled = true;

        yield return null;
    }
}
