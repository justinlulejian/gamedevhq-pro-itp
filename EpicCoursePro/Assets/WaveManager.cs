using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevHQ.Scripts;
using UnityEngine;

public class WaveManager : MonoSingleton<WaveManager>
{
    
    [SerializeField] 
    [Header("Wave Settings")]
    private List<Wave> _waves;

    private int enemiesSpawnedInWave;

    private Queue<Wave> _wavesToSpawn;

    // When a wave has started. Will later likely be used to provide info to UI about wave.
    public static event Action<Wave> onWaveStart; 
    // When a wave requested has finished.
    public static event Action onWaveFinish;
    // When all waves have completed spawning. Will later likely be used to provide info to UI.
    public static event Action onWavesComplete;
    
    private void OnEnable()
    {
        EnemyNavEnd.OnEnemyCollision += DespawnEnemy;
    }

    private void OnDisable()
    {
        EnemyNavEnd.OnEnemyCollision -= DespawnEnemy;
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
            SpawnEnemy(randomEnemyType);
            yield return new WaitForSeconds(2f);
        }
    }
    
    private IEnumerator SpawnFixed(Wave wave)
    {
        List<GameObject> spawnSequence = wave.fixedSpawnSequence;
        for (int i = 0; i < spawnSequence.Count; i++)
        {
            GameObject enemyType = spawnSequence.ElementAt(i);
            SpawnEnemy(enemyType);
            yield return new WaitForSeconds(2f);
        }
    }
    
    private void SpawnEnemy(GameObject enemyType)
    {
        GameObject enemy = PoolManager.Instance.RequestEnemyType(enemyType);
        enemy.SetActive(true);
        enemiesSpawnedInWave++;
    }

    private void SpawnWave(Wave wave)
    {
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
            SpawnWave(_wavesToSpawn.Dequeue());
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
        
        // TODO: reset position to start with warp 
    }

    private void AllWavesCompleted()
    {
        Debug.Log("All waves have been spawned. No more will spawn.");
    }
}
