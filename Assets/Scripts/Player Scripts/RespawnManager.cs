using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class RespawnManager : MonoBehaviour
{
    EnemyManager enemyManager;
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] float resetSpeed;
    GameObject player;
    public int spawnIndex;
    public int lives = 3;
    public Image[] lifeCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 spawnPos = spawnPoints[0].transform.position;
        Vector2 playerPos = player.transform.position;
    }

    public void SetSpawnIndex(GameObject spawnPoint)
    {
        if (enemyManager.CheckGroupDefeated(spawnIndex))
        {
            spawnPoint.GetComponent<RespawnPoint>().active = true;
            spawnPoint.GetComponent<RespawnPoint>().SwitchColor();  
            spawnIndex = spawnPoints.IndexOf(spawnPoint);
        }        
            enemyManager.ActivateGroup(spawnPoints.IndexOf(spawnPoint));
    }

    public void ResetPlayer()
    {
        if (lives <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }
        lives--;
        if(lives > 0)
        {
            lifeCount[lives].enabled = false;
        }
       
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {        
        player.GetComponent<PlayerMovement>().controlable = false;
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        playerRb.linearVelocity = Vector2.zero;
        Vector2 spawnPos = spawnPoints[spawnIndex].transform.position;
        player.transform.position = spawnPos;
        player.GetComponent<TrailRenderer>().Clear();

        yield return new WaitForSeconds(1f);

        player.GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<Collider2D>().enabled = true;
        player.GetComponent<PlayerAttack>().currentInspiration = 0;
        player.GetComponent<Health>().Heal(100);
        player.GetComponent<PlayerMovement>().controlable = true;

        enemyManager.RespawnEnemies(spawnIndex);
    }
}
