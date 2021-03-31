using System;
using System.Linq;
using GameDevHQ.Scripts;
using Rock.Collections;
using UnityEngine;

public class AttackRadius : MonoBehaviour
{
    [SerializeField]
    private GameObject _towerObj;
    private Tower _tower;
    private SphereCollider _sphereCollider;
    
    // TODO: Consider since Enemy has the GO, can't that be the single key in an ordered HashSet?
    private OrderedDictionary<GameObject, Enemy> _enemiesInAttackRadius =
        new OrderedDictionary<GameObject, Enemy>();
    
    public static event Action<GameObject, GameObject> OnEnemyEnterTowerRadius;
    public static event Action<GameObject, GameObject> OnEnemyExitTowerRadius;
    
    private void OnEnable()
    {
        Enemy.onEnemyKilledByPlayer += RemoveTargetFromTrackingAndMaybeUpdate;
        EnemyNavEnd.onEnemyCollision += RemoveTargetFromTrackingAndMaybeUpdate;
    }

    private void OnDisable()
    {
        Enemy.onEnemyKilledByPlayer -= RemoveTargetFromTrackingAndMaybeUpdate;
        EnemyNavEnd.onEnemyCollision -= RemoveTargetFromTrackingAndMaybeUpdate;
    }

    private void Awake()
    {
        _tower = _towerObj.GetComponent<Tower>();

        if (_tower == null)
        {
            Debug.LogError($"Tower behavior script is not available from attack radius" +
                           $" {name}");
        }

        _sphereCollider = GetComponent<SphereCollider>();
        if (_sphereCollider == null)
        {
            Debug.LogError($"Mesh collider is not available from attack radius" +
                           $" {name}");
        }
        _sphereCollider.enabled = false;

    }

    public void EnableCollider()
    {
        _sphereCollider.enabled = true;
    }

    private Enemy GetEnemyComponentFromCollider(GameObject enemyObj)
    {
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError($"Enemy {enemyObj.name} that entered attack radius of " +
                           $"tower {name} does not have a behavior script attached.");
            return null;
        }

        return enemy;
    }
    
    private void TrackTarget(GameObject enemyObj)
    {
        if (!_enemiesInAttackRadius.ContainsKey(enemyObj))
        {
            _enemiesInAttackRadius[enemyObj] = GetEnemyComponentFromCollider(enemyObj);
        }
    }

    // TODO: Implement priority queuing with _enemiesInAttackRadius so that an enemy further along
    // in the path towards the navigation end has higher priority when selecting choosing target for
    // tower vs just whichever entered the radius first.
    private void UpdateTarget()
    {
        _tower.UpdateAttackTarget(_enemiesInAttackRadius.Count > 0
            ? _enemiesInAttackRadius.First().Value
            : null);
    }

    private void RemoveTargetFromTrackingAndMaybeUpdate(GameObject enemyObj)
    {
        if (_enemiesInAttackRadius.Remove(enemyObj))
        {
            UpdateTarget();
        }
    }
    
    private void RemoveTargetFromTrackingAndMaybeUpdate(Enemy enemy)
    {
        RemoveTargetFromTrackingAndMaybeUpdate(enemy.gameObject);
    }
    
    // Enqueue the enemy and it's behavior script that enters radius.
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        GameObject enemy = other.gameObject;
        OnEnemyEnterTowerRadius?.Invoke(enemy, _tower.gameObject);
        TrackTarget(enemy);
        UpdateTarget();
    }

    // Remove enemy from attack radius tracking if it exits.
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        GameObject enemy = other.gameObject;
        OnEnemyExitTowerRadius?.Invoke(enemy, _tower.gameObject);
        RemoveTargetFromTrackingAndMaybeUpdate(enemy);
    }
}
