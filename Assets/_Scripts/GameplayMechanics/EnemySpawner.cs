using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPool;
    [SerializeField] private Transform enemyLocation;

    public bool CanSpawn => enemyPool != null && enemyPool.Length > 0 && enemyLocation != null;

    public void Spawner(int count, HandView handView, List<Enemy> enemies, int roomCounter, UiManager uiManager)
    {
        count = Mathf.Max(1, count);

        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        enemies.Clear();

        for (int i = 0; i < count; ++i)
        {
            GameObject prefab = enemyPool[Random.Range(0, enemyPool.Length)];
            GameObject random_enemy = Instantiate(prefab, enemyLocation, false);
            
            Enemy enemyComponent = random_enemy.GetComponent<Enemy>();
            if (enemyComponent == null)
            {
                Debug.LogWarning($"Spawned prefab '{prefab.name}' has no Enemy component.");
                Destroy(random_enemy);
                continue;
            }

            enemies.Add(enemyComponent);
            enemyComponent.SetHand(handView, uiManager);
            enemyComponent.SetupForRoom(roomCounter);

            Debug.Log($"Spawned enemy from pool: {prefab.name}");
        }
        Debug.Log($"Spawner complete. Active enemies: {enemies.Count}");
    }
}
