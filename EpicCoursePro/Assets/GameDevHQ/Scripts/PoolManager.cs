using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;


namespace GameDevHQ.Scripts
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        [SerializeField]
        private Vector3 _defaultPositionForPooledObjects = new Vector3(0, 0, 0);
        
        [Header("Enemy pool settings")]
        [SerializeField] 
        private GameObject _enemyContainer;
        [SerializeField] 
        private List<GameObject> _allEnemyPrefabs = new List<GameObject>();
        [SerializeField]
        private int _numberOfEnemiesToPool = 30;
        private List<GameObject> _enemyPool = new List<GameObject>();
        
        [Header("Tower pool settings")]
        [SerializeField] 
        private GameObject _towerContainer;
        [SerializeField] 
        private List<GameObject> _allTowerPrefabs = new List<GameObject>();
        [SerializeField]
        private int _numberOfTowersToPool = 5;
        private List<GameObject> _towerPool = new List<GameObject>();
        
        [Header("Missile turret missile pool settings")]
        [SerializeField] 
        private GameObject _missileContainer;
        [SerializeField] 
        private GameObject _missilePrefab;
        [SerializeField]
        private int _numberOfMissilesToPool = 6;
        private List<GameObject> _missilePool = new List<GameObject>();

        
        
        protected override void Awake()
        {
            base.Awake();
            if (_allEnemyPrefabs.Count == 0) 
            {
                Debug.LogError("Enemy prefabs have not been specified in Pool Manager.");
            }
            if (_allTowerPrefabs.Count == 0) 
            {
                Debug.LogError("Tower prefabs have not been specified in Pool Manager.");
            }
            if (_missilePrefab == null) 
            {
                Debug.LogError("Missile prefab has not been specified in Pool Manager.");
            }

            foreach (GameObject enemyPrefab in _allEnemyPrefabs)
            {
                GeneratePooledObjects(_numberOfEnemiesToPool, enemyPrefab, _enemyPool,
                    _enemyContainer.transform);
            }
            foreach (GameObject towerPrefab in _allTowerPrefabs)
            {
                GeneratePooledObjects(
                    _numberOfTowersToPool, towerPrefab, _towerPool, _towerContainer.transform);
            }
            GeneratePooledObjects(
                _numberOfMissilesToPool, _missilePrefab, _missilePool, _missileContainer.transform);
        }
        
        private void GeneratePooledObjects(int numObjects, GameObject objectType,
            List<GameObject> pool, Transform parentTransform)
        {
            for (int i = 0; i < numObjects; i++)
            {
                GameObject spawnedObj = Instantiate(objectType, parentTransform);
                spawnedObj.SetActive(false);
                pool.Add(spawnedObj);
            }
        }

        public GameObject RequestObjOfType(GameObject objType)
        {
            if (_allEnemyPrefabs.Contains(objType))
            {
               return RequestEnemyType(objType);
            }
            if (_allTowerPrefabs.Contains(objType))
            {
                return RequestTowerType(objType);
            }
            if (_missilePrefab.CompareTag(objType.tag))
            {
                return RequestMissile(objType);
            }

            Debug.LogError($"Pool manager couldn't find object of type {objType.name} in a" +
                           $"pool.");
            return null;
        }

        // TODO: Collapse all the Request types into generic methods that can find any type.
        // Provides one inactivated enemy from the pool of the type that was specified.
        private GameObject RequestEnemyType(GameObject enemyType)
        {
            Enemy requestedEnemy = enemyType.GetComponent<Enemy>();
            if (requestedEnemy == null)
            {
                Debug.LogError($"Enemy {name} does not have a Enemy script attached.");
            }
            var inactiveEnemyOfType = _enemyPool.FirstOrDefault(
                e => 
                    e.GetComponent<Enemy>().EnemyType == requestedEnemy.EnemyType &&
                    !e.activeInHierarchy);
            if (inactiveEnemyOfType != null)
            {
                return inactiveEnemyOfType;
            }
        
            GeneratePooledObjects(1, enemyType, _enemyPool, _enemyContainer.transform);
            return RequestEnemyType(enemyType);
        }

        private GameObject RequestTowerType(GameObject towerType)
        {
            var inactiveTowerOfType = _towerPool.FirstOrDefault(
                t => 
                    // TODO(optimization): Is there a faster way than this double GetComponent call?
                    t.GetComponent<AbstractTower>().TowerType ==
                    towerType.GetComponent<AbstractTower>().TowerType &&
                    !t.activeInHierarchy);
            
            if (inactiveTowerOfType != null)
            {
                return inactiveTowerOfType;
            }
            
            GeneratePooledObjects(1, towerType, _towerPool, _towerContainer.transform);
            return RequestTowerType(towerType);
        }
        
        private GameObject RequestMissile(GameObject missilePrefab)
        {
            var inactiveMissile = _missilePool.FirstOrDefault(
                m => !m.activeInHierarchy);
            
            if (inactiveMissile != null)
            {
                return inactiveMissile;
            }
            
            GeneratePooledObjects(1, missilePrefab, _missilePool, _missileContainer.transform);
            return RequestMissile(missilePrefab);
        }
        
        public void RecyclePooledObj(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.position = _defaultPositionForPooledObjects;
        }
    }
}
