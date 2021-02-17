using System;
using System.Collections;
using System.Collections.Generic;
using GameDevHQ.Scripts;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField] 
    private List<GameObject> _enemiesToSpawn = new List<GameObject>();
    [SerializeField] 
    private GameObject _enemyContainer;
    [SerializeField] 
    private GameObject _enemySpawnStartPoint;
    [SerializeField] 
    private GameObject _enemySpawnEndPoint;

    private void OnEnable()
    {
        EnemyNavEnd.enemyCollision += DespawnEnemy;
    }

    private void OnDisable()
    {
        EnemyNavEnd.enemyCollision -= DespawnEnemy;
    }

    private void Awake()
    {
        if (_enemiesToSpawn.Count <= 0)
        {
            Debug.LogError("No enemies were specified to spawn in spawn manager.");
        }
        if (_enemyContainer == null)
        {
            Debug.LogError("No enemies container was specified in spawn manager.");
        }
        if (_enemySpawnStartPoint == null)
        {
            Debug.LogError("Enemy spawn start point was null in spawn manager.");
        }
        if (_enemySpawnEndPoint == null)
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
        foreach (GameObject enemy in _enemiesToSpawn)
        {
            GameObject spawnedEnemy = Instantiate(
                enemy, _enemySpawnStartPoint.transform.position,
                _enemySpawnStartPoint.transform.rotation);
            spawnedEnemy.transform.parent = _enemyContainer.transform;
            Enemy spawnedEnemyObj = spawnedEnemy.GetComponent<Enemy>();
            if (spawnedEnemyObj == null)
            {
                Debug.LogError($"Spawned enemy {spawnedEnemy.name} has no script attached.");
            }

            spawnedEnemyObj.NavTarget = _enemySpawnEndPoint;
            yield return new WaitForSeconds(3.0f);
        }
    }

    private void DespawnEnemy(GameObject enemy)
    {
        Destroy(enemy);
    }

}
