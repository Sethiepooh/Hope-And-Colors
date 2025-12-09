using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public SpawnPoint[] spawnPoints;
    public GameObject[] enemies;
    public int enemiesPerWave;
    public int enemiesSpawnedInCurrentWave = 0;
    public NextLevel nextLevel;

    int wavesCompleted = 0;

    bool spawning = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnGroup());
    }

    // Update is called once per frame
    void Update()
    {
        if(wavesCompleted >= 3)
        {
            nextLevel.LoadNextLevel();
        }

        if (spawning == false && enemiesSpawnedInCurrentWave == 0)
        {
            if(enemiesPerWave < spawnPoints.Length)
            {
                enemiesPerWave += 3;
            }
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
                int randEnemyIndex = Random.Range(0, enemies.Length);
                spawnPoint.PlayEffect();
                StartCoroutine(spawnPoint.SpawnEnemy(enemies[randEnemyIndex]));
                if(randEnemyIndex != enemies.Length - 1)
                {
                    enemiesSpawnedInCurrentWave++;
                }
                else
                {
                    i--; // Do not count this enemy towards the wave total
                }
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
