﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevHQ.Scripts;
using UnityEditor.Rendering;
using UnityEngine;

public class WaveManager : MonoSingleton<WaveManager>
{
    
    [SerializeField] 
    [Header("Wave Settings")]
    private List<Wave> _waves;
    [SerializeField] 
    private float _baseTimeToWaitBetweenEnemySpawns = 10f;

    private int enemiesSpawnedInWave;

    private Queue<Wave> _wavesToSpawn;

    private Enemy _lastEnemySpawnedInWave;

    // When a wave has started. Will later likely be used to provide info to UI about wave.
    public static event Action<Wave> onWaveStart; 
    // When a wave requested has finished.
    public static event Action onWaveFinish;
    // When all waves have completed spawning. Will later likely be used to provide info to UI.
    public static event Action onWavesComplete;
    
    private void OnEnable()
    {
        EnemyNavEnd.onEnemyCollision += DespawnEnemy;
    }

    private void OnDisable()
    {
        EnemyNavEnd.onEnemyCollision -= DespawnEnemy;
    }
    
    protected override void Awake()
    {
        base.Awake();
        _wavesToSpawn = new Queue<Wave>(_waves);

        if (_wavesToSpawn.Count < 1)
        {
            Debug.LogError("There were no waves to spawn in Wave Manager.");
        }
    }

    private IEnumerator SpawnRandom(Wave wave)
    {
        for (int i = 0; i < wave.amountToSpawn; i++)
        {
            List<GameObject> enemyTypes = wave.enemyTypesToSpawn;
            GameObject randomEnemyType = enemyTypes.ElementAt(
                UnityEngine.Random.Range(0, enemyTypes.Count));
            yield return new WaitForSeconds(TimeToWaitBetweenEnemySpawnInWave(randomEnemyType));
            SpawnEnemy(randomEnemyType);
        }

        _lastEnemySpawnedInWave = null;
    }

    // TODO(improvement): Rather than do this with static time values, try dynamically spacing
    // enemies even if they have different speeds.
    private float TimeToWaitBetweenEnemySpawnInWave(GameObject enemyTypeToSpawn)
    {
        if (_lastEnemySpawnedInWave == null)
        {
            // Don't wait if this is the first enemy in the wave.
            return 0f;
        }
        
        float enemyTypeToSpawnSpeed = enemyTypeToSpawn.GetComponent<Enemy>().GetSpeed();
        if (enemyTypeToSpawnSpeed > _lastEnemySpawnedInWave.GetSpeed())
        {
            return _baseTimeToWaitBetweenEnemySpawns * 1.5f;
        }

        return _baseTimeToWaitBetweenEnemySpawns;
    }
    
    private IEnumerator SpawnFixed(Wave wave)
    {
        List<GameObject> spawnSequence = wave.fixedSpawnSequence;
        for (int i = 0; i < spawnSequence.Count; i++)
        {
            GameObject enemyType = spawnSequence.ElementAt(i);
            yield return new WaitForSeconds(TimeToWaitBetweenEnemySpawnInWave(enemyType));
            SpawnEnemy(enemyType);
        }

        _lastEnemySpawnedInWave = null;
    }
    
    private void SpawnEnemy(GameObject enemyType)
    {
        GameObject enemy = PoolManager.Instance.RequestEnemyType(enemyType);
        enemy.SetActive(true);
        _lastEnemySpawnedInWave = enemy.GetComponent<Enemy>();
        enemiesSpawnedInWave++;
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log($"Spawning next wave: {wave.ToString()}");
        yield return new WaitForSeconds(wave.timeBeforeStart);
        if (wave.randomSpawnOn)
        {
            StartCoroutine(SpawnRandom(wave));
        }
        else
        {
            StartCoroutine(SpawnFixed(wave));
        }
    }

    public void SpawnNextWave()
    {

        if (_wavesToSpawn.Count < 1)
        {
            onWavesComplete.Invoke();
            AllWavesCompleted();
        }
        
        if (_wavesToSpawn.Count > 0)
        {
            StartCoroutine(SpawnWave(_wavesToSpawn.Dequeue()));
        }
    }
    
    private void DespawnEnemy(GameObject enemy)
    {
        PoolManager.Instance.RecycleEnemy(enemy);
        enemiesSpawnedInWave--;

        if (enemiesSpawnedInWave == 0)
        {
            onWaveFinish.Invoke();
        }
    }

    private void AllWavesCompleted()
    {
        Debug.Log("All waves have been spawned. No more will spawn.");
    }
}