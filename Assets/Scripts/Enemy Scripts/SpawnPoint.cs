using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using static EnemyManager;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] ParticleSystem spawnEffect;
    [SerializeField] RoomEncounterManager roomEncounterManager;
    EnemyBase currentEnemy;
    public bool hasEnemy;

    public IEnumerator SpawnEnemy(RoomEncounterManager.EnemySpawnConfig enemy)
    {
        List<RoomEncounterManager.EnemySpawnConfig> enemies = new List<RoomEncounterManager.EnemySpawnConfig>();

        enemies.Add(enemy);

        hasEnemy = true;

        yield return new WaitForSeconds(4);
        currentEnemy = roomEncounterManager.SpawnEnemyGroup(enemies);
    }

    public IEnumerator SpawnAlixShaman(RoomEncounterManager.EnemySpawnConfig enemy)
    {
        List<RoomEncounterManager.EnemySpawnConfig> enemies = new List<RoomEncounterManager.EnemySpawnConfig>();
        enemies.Add(enemy);

        hasEnemy = true;

        yield return new WaitForSeconds(4);
        currentEnemy = roomEncounterManager.SpawnEnemyGroup(enemies, true, 0);

        currentEnemy.GetComponent<GlitchShaman>().SetProtectionTarget(GameObject.FindWithTag("Boss").GetComponent<EnemyBase>());
    }

    public void DestroyEnemy()
    {
        if (currentEnemy != null)
        {
            hasEnemy = false;
            if (currentEnemy != null)
                currentEnemy.ObliterateEnemy();
            currentEnemy = null;
        }    
    }

    public bool HasEnemy()
    {
        return currentEnemy != null;
    }

    public void PlayEffect()
    {
        spawnEffect.Play();
    }
}
