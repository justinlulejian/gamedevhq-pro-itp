using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;


namespace GameDevHQ.Scripts
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        private static List<GameObject> _enemyPool;
        [SerializeField] 
        private GameObject _enemyContainer;
        [SerializeField] 
        private List<GameObject> _enemyPrefabs = new List<GameObject>();

        private Random _random;
        private bool _setupComplete;

        protected override void Awake()
        {
            base.Awake();
            if (_enemyPrefabs.Count == 0) 
            {
                Debug.LogError("Enemy prefabs have not been specified in Pool Manager.");
            } 
            
            _random = new Random();
            _enemyPool = GenerateEnemies(30);
            _setupComplete = true;
        }
        
        private List<GameObject> GenerateEnemies(int numEnemies)
        {
        
            List<GameObject> spawnedEnemies = new List<GameObject>();
            for (int i = 0; i < numEnemies; i++)
            {
                GameObject enemyPrefab = _enemyPrefabs.ElementAt(
                    _random.Next(_enemyPrefabs.Count));
                GameObject enemyStartPoint = SpawnManager.Instance.GetEnemySpawnStartPoint();
                if (enemyStartPoint == null)
                {
                    Debug.LogError("Enemy spawn start position was null from Spawn Manager.");
                }
                GameObject spawnedEnemy = Instantiate(
                    enemyPrefab, enemyStartPoint.transform.position, enemyStartPoint.transform.rotation,
                    _enemyContainer.transform);
                spawnedEnemy.SetActive(false);
                spawnedEnemies.Add(spawnedEnemy);
            }
            return spawnedEnemies;
        }

        // Provides a random enemy to the caller. When pre-spawned enemies are exhausted it generates
        // on additional one recursively.
        public GameObject RequestEnemy()
        {
            var inactiveEnemy = _enemyPool.FirstOrDefault(
                b => !b.activeInHierarchy);
            if (inactiveEnemy != null)
            {
                inactiveEnemy.SetActive(true);
                return inactiveEnemy;
            }
        
            _enemyPool = GenerateEnemies(1);
            return RequestEnemy();
        }
        
        public void RecycleEnemy(GameObject enemy)
        {
            enemy.SetActive(false);
        }
    }
}
