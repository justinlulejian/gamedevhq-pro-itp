using System;
using System.Collections;
using System.Collections.Generic;
using GameDevHQ.Scripts;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField] 
    private List<GameObject> enemiesToSpawn = new List<GameObject>();
    [SerializeField] 
    private GameObject enemyContainer;
    [SerializeField] 
    private GameObject enemySpawnStartPoint;
    [SerializeField] 
    private GameObject enemySpawnEndPoint;

    private void OnEnable()
    {
        EnemyNavEnd.EnemyCollision += DespawnEnemy;
    }

    private void OnDisable()
    {
        EnemyNavEnd.EnemyCollision -= DespawnEnemy;
    }

    private void Awake()
    {
        if (enemiesToSpawn.Count <= 0)
        {
            Debug.LogError("No enemies were specified to spawn in spawn manager.");
        }
        if (enemyContainer == null)
        {
            Debug.LogError("No enemies container was specified in spawn manager.");
        }
        if (enemySpawnStartPoint == null)
        {
            Debug.LogError("Enemy spawn start point was null in spawn manager.");
        }
        if (enemySpawnEndPoint == null)
        {
            Debug.LogError("Enemy spawn end point was null in spawn manager.");
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        foreach (GameObject enemy in enemiesToSpawn)
        {
            GameObject spawnedEnemy = Instantiate(
                enemy, enemySpawnStartPoint.transform.position,
                enemySpawnStartPoint.transform.rotation);
            spawnedEnemy.transform.parent = enemyContainer.transform;
            Enemy spawnedEnemyObj = spawnedEnemy.GetComponent<Enemy>();
            if (spawnedEnemyObj == null)
            {
                Debug.LogError($"Spawned enemy {spawnedEnemy.name} has no script attached.");
            }

            spawnedEnemyObj.NavTarget = enemySpawnEndPoint;
            yield return new WaitForSeconds(3.0f);
        }
    }

    private void DespawnEnemy(GameObject enemy)
    {
        Destroy(enemy);
    }

}
