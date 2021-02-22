using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;


namespace GameDevHQ.Scripts
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        private static List<GameObject> _enemyPool = new List<GameObject>();
        [SerializeField] 
        private GameObject _enemyContainer;
        [SerializeField] 
        private List<GameObject> _allEnemyPrefabs = new List<GameObject>();
        [SerializeField]
        private int _numberOfEnemiesToPool = 30;
        
        protected override void Awake()
        {
            base.Awake();
            if (_allEnemyPrefabs.Count == 0) 
            {
                Debug.LogError("Enemy prefabs have not been specified in Pool Manager.");
            }

            foreach (GameObject enemyPrefab in _allEnemyPrefabs)
            {
                GenerateEnemies(_numberOfEnemiesToPool, enemyPrefab);
            }
        }
        
        private void GenerateEnemies(int numEnemies, GameObject enemyType)
        {
            for (int i = 0; i < numEnemies; i++)
            {
                GameObject spawnedEnemy = Instantiate(enemyType, _enemyContainer.transform);
                spawnedEnemy.SetActive(false);
                _enemyPool.Add(spawnedEnemy);
            }
        }

        // Provides one inactivated enemy from the pool of the type that was specified.
        public GameObject RequestEnemyType(GameObject enemyType)
        {
            var inactiveEnemyOfType = _enemyPool.FirstOrDefault(
                e => e.CompareTag(enemyType.tag) && !e.activeInHierarchy);
            if (inactiveEnemyOfType != null)
            {
                return inactiveEnemyOfType;
            }
        
            GenerateEnemies(1, enemyType);
            return RequestEnemyType(enemyType);
        }
        
        public void RecycleEnemy(GameObject enemy)
        {
            enemy.SetActive(false);
        }
    }
}
