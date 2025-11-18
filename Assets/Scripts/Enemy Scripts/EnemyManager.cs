using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    //public List<GameObject> enemies = new List<GameObject>();
    public EnemyGroup[] enemyGroups;
    RespawnManager respawnManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetEnemies();
        respawnManager = GameObject.FindWithTag("RespawnManager").GetComponent<RespawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(CheckGroupDefeated(respawnManager.spawnIndex))
        {
            if (enemyGroups[respawnManager.spawnIndex].door != null)
            {
                enemyGroups[respawnManager.spawnIndex].door.SetActive(false);
            } 
        }
    }

    //public void AddEnemy(GameObject enemy)
    //{
    //    enemies.Add(enemy);
    //}

    //public void RemoveEnemy(GameObject enemy)
    //{
    //    enemies.Remove(enemy);
    //}

    void SetEnemies()
    {
        foreach (EnemyGroup enemyGroup in enemyGroups)
        {
           for (int i = 0; i < enemyGroup.enemies.Length; i++)
            {
                enemyGroup.enemies[i] = NewEnenemy(enemyGroup.enemies[i]);
            }
        }
    }

    Enemy NewEnenemy(Enemy e)
    {
        return new Enemy(e.enemyObject, e.enemyObject.transform.position);
    }

    public void RespawnEnemies(int i)
    {
        for (int j = i; j < enemyGroups.Length; j++)
        {
            for(int x = 0; x < enemyGroups[j].enemies.Length; x++)
            {
                enemyGroups[j].enemies[x].enemyObject.GetComponent<Health>().Heal(1000);
                enemyGroups[j].enemies[x].enemyObject.GetComponent<SpriteRenderer>().enabled = true;
                enemyGroups[j].enemies[x].enemyObject.GetComponent<Collider2D>().enabled = true;

                if(j == i)
                    enemyGroups[j].enemies[x].enemyObject.GetComponent<EnemyBase>().active = true;

                enemyGroups[j].enemies[x].enemyObject.SetActive(true);
                enemyGroups[j].enemies[x].enemyObject.transform.position = enemyGroups[j].enemies[x].startingPos;
            }
        }
    }

    public void ActivateGroup(int i)
    {
        for (int x = 0; x < enemyGroups[i].enemies.Length; x++)
        {
            enemyGroups[i].enemies[x].enemyObject.GetComponent<EnemyBase>().active = true;
        }
    }

    bool CheckGroupDefeated(int i)
    {
        foreach (Enemy e in enemyGroups[i].enemies)
        {
            if (e.enemyObject.activeInHierarchy)
                return false;
        }
        return true;
    }


    public void AddBeatToAll()
    {
        foreach (EnemyGroup enemy in enemyGroups)
        {
            foreach (Enemy e in enemy.enemies)
            {
                if (e.enemyObject != null)
                {
                    if(e.enemyObject.activeInHierarchy)
                        e.enemyObject.GetComponent<EnemyBase>().AddToBeatCount();
                }                  
            }              
        }
    }

    [System.Serializable]
    public struct EnemyGroup
    {
        public Enemy[] enemies;
        public GameObject door;
    }

    [System.Serializable]
    public struct Enemy
    {
        public GameObject enemyObject;
        public Vector2 startingPos;

        public Enemy( GameObject obj, Vector2 pos)
        {
            enemyObject = obj;
            startingPos = pos;
        }
    }
}
