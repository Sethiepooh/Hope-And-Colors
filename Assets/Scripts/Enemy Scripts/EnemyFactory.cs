using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    // Dictionary to map enemy type names to their prefabs
    private Dictionary<string, GameObject> enemyPrefabs = new();

    // Register an enemy prefab with a type name
    public void RegisterEnemy(string enemyType, GameObject prefab)
    {
        if (!enemyPrefabs.ContainsKey(enemyType))
        {
            enemyPrefabs.Add(enemyType, prefab);
        }
    }

    // Spawn an enemy of the given type at the specified position and rotation
    public EnemyBase SpawnEnemy(string enemyType, Vector3 position, Quaternion rotation)
    {
        if (enemyPrefabs.TryGetValue(enemyType, out var prefab))
        {
            GameObject enemyObj = Instantiate(prefab, position, rotation);
            EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.active = true;
                return enemy;
            }
            else
            {
                Destroy(enemyObj);
                throw new InvalidOperationException($"Prefab for {enemyType} does not have an EnemyBase component.");
            }
        }
        else
        {
            throw new ArgumentException($"Enemy type '{enemyType}' is not registered.");
        }
    }
}
