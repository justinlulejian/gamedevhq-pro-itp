using GameDevHQ.Scripts;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [Header("Enemy Spawn Settings")]
    [SerializeField]
    private GameObject _enemySpawnStartPoint;
    [SerializeField]
    private GameObject _enemySpawnEndPoint;

    public bool SpawningEnabled { get; set; } = true;

    private GameObject GetEnemySpawnStartPoint()
    {
        return _enemySpawnStartPoint;
    }

    private GameObject GetEnemySpawnEndPoint()
    {
        return _enemySpawnEndPoint;
    }
    
    private void OnEnable()
    {
        WaveManager.onWaveFinish += RequestNextWave;
        Enemy.onSpawnStart += ResetEnemyNavOnSpawn;
    }

    private void OnDisable()
    {
        WaveManager.onWaveFinish -= RequestNextWave;
        Enemy.onSpawnStart -= ResetEnemyNavOnSpawn;
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

    public void RequestNextWave()
    {
        if (SpawningEnabled)
        {
            WaveManager.Instance.SpawnNextWave();
        }
    }

    private void ResetEnemyNavOnSpawn(Transform enemyTransform, NavMeshAgent enemyNavMeshAgent)
    {
        GameObject enemyStartPoint = GetEnemySpawnStartPoint();
        if (enemyStartPoint == null)
        {
            Debug.LogError("Enemy spawn start position was null from Spawn " +
                           "Manager.");
        }
        enemyTransform.rotation = enemyStartPoint.transform.rotation;
        if (!enemyNavMeshAgent.Warp(enemyStartPoint.transform.position))
        {
            Debug.LogError($"Attempt to warp enemy {enemyTransform.gameObject.name} to " +
                           $"start position {enemyStartPoint.transform.position.ToString()}" +
                           $" failed.");
        }
        enemyNavMeshAgent.SetDestination(GetEnemySpawnEndPoint().transform.position);
    }


}
