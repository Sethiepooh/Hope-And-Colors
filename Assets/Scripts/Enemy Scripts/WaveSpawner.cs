using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public SpawnPoint[] spawnPoints;
    public GameObject[] enemies;
    public int enemiesPerWave = 6;
    public int enemiesSpawnedInCurrentWave = 0;

    int wavesCompleted = 0;

    bool spawning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(wavesCompleted >= 3)
        {
            return;
        }

        if (spawning == false && enemiesSpawnedInCurrentWave == 0)
        {
            enemiesPerWave += 6;
            spawning = true;
            StartCoroutine(SpawnGroup());
        }
    }

    IEnumerator SpawnGroup()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (!spawnPoint.HasEnemy())
            {
                spawnPoint.PlayEffect();
                StartCoroutine(spawnPoint.SpawnEnemy(enemies[Random.Range(0, enemies.Length)]));
                enemiesSpawnedInCurrentWave++;
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                i--; // Retry this iteration if the spawn point is occupied
            }
        }


        spawning = false;

    }
}
