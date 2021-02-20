using System;
using System.Collections;
using System.Collections.Generic;
using GameDevHQ.Scripts;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [Header("Enemy Spawn Settings")]
    [SerializeField]
    private GameObject _enemySpawnStartPoint;
    [SerializeField]
    private GameObject _enemySpawnEndPoint;

    [SerializeField] 
    [Header("Wave Settings")]
    [Tooltip("Enemies per wave = <this number> * the current wave number")]
    private int _baseEnemiesToSpawnPerWave = 10;
    [SerializeField]
    private int _numberOfWaves = 1;
    [SerializeField]
    private int _currentWave;
    // TODO: create a better system for timing to keep spawn tidy.
    private float _timeBetweenEnemySpawnInWave = 3.0f;
    
    public GameObject GetEnemySpawnStartPoint()
    {
        return _enemySpawnStartPoint;
    }
    
    public GameObject GetEnemySpawnEndPoint()
    {
        return _enemySpawnEndPoint;
    }
    
    private void OnEnable()
    {
        EnemyNavEnd.OnEnemyCollision += DespawnOnEnemy;
    }

    private void OnDisable()
    {
        EnemyNavEnd.OnEnemyCollision -= DespawnOnEnemy;
    }

    protected override void Awake()
    {
        base.Awake();
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
        // TODO: add multi-wave system.
        List<GameObject> spawnedEnemies = new List<GameObject>();
        for (int j = 0; j < _baseEnemiesToSpawnPerWave; j++)
        {
            PoolManager.Instance.RequestEnemy();
            yield return new WaitForSeconds(_timeBetweenEnemySpawnInWave);
        }
        
    }

    private void DespawnOnEnemy(GameObject enemy)
    {
        PoolManager.Instance.RecycleEnemy(enemy);
    }

}
