using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    //public List<GameObject> enemies = new List<GameObject>();
    public EnemyGroup[] enemyGroups;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetEnemyStartingPos();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void AddEnemy(GameObject enemy)
    //{
    //    enemies.Add(enemy);
    //}

    //public void RemoveEnemy(GameObject enemy)
    //{
    //    enemies.Remove(enemy);
    //}

    void SetEnemyStartingPos()
    {
        foreach (EnemyGroup enemy in enemyGroups)
        {
            foreach(Enemy e in enemy.enemies)
            {
                e.SetStartingPos(e.enemyObject.transform.position);
            }
        }
    }

    public void RespawnEnemies(int i)
    {
        for (int j = i; j < enemyGroups.Length; j++)
        {
            foreach (Enemy e in enemyGroups[j].enemies)
            {
                Respawn(e.enemyObject);
                e.enemyObject.transform.position = e.startingPos;
            }
        }
    }

    void Respawn(GameObject enemy)
    {
        enemy.GetComponent<Health>().Heal(1000);
        enemy.SetActive(true);
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
    }

    [System.Serializable]
    public struct Enemy
    {
        public GameObject enemyObject;
        public Vector2 startingPos;

        public void SetStartingPos( Vector2 pos)
        {
            startingPos = pos;
        }
    }
}
