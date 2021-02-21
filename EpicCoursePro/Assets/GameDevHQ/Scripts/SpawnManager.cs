using System;
using System.Collections;
using System.Collections.Generic;
using GameDevHQ.Scripts;
using UnityEditor.Rendering;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [Header("Enemy Spawn Settings")]
    [SerializeField]
    private GameObject _enemySpawnStartPoint;
    [SerializeField]
    private GameObject _enemySpawnEndPoint;

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
        WaveManager.onWaveFinish += RequestNextWave;
    }

    private void OnDisable()
    {
        WaveManager.onWaveFinish -= RequestNextWave;
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
        WaveManager.Instance.SpawnNextWave();
    }

    private void RequestNextWave()
    {
        WaveManager.Instance.SpawnNextWave();
    }




}
