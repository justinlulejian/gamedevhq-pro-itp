using System.Linq;
using GameDevHQ.Scripts;
using Rock.Collections;
using UnityEditor.Rendering;
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
    
    private void OnEnable()
    {
        Enemy.onEnemyKilledByPlayer += RemoveTargetFromTrackingAndMaybeUpdate;
    }

    private void OnDisable()
    {
        Enemy.onEnemyKilledByPlayer -= RemoveTargetFromTrackingAndMaybeUpdate;
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
    // tower.
    private void UpdateTarget()
    {
        if (!_tower.IsPlaced) return;
        _tower.UpdateAttackTarget(_enemiesInAttackRadius.Count > 0
            ? _enemiesInAttackRadius.First().Value
            : null);
    }

    private void RemoveTargetFromRadiusTracking(GameObject enemyObj)
    {
        _enemiesInAttackRadius.Remove(enemyObj);
    }
    
    private void RemoveTargetFromTrackingAndMaybeUpdate(GameObject enemyObj)
    {
        if (!_tower.IsPlaced) return;
        RemoveTargetFromRadiusTracking(enemyObj);
        UpdateTarget();
    }
    
    private void RemoveTargetFromTrackingAndMaybeUpdate(Enemy enemy)
    {
        RemoveTargetFromTrackingAndMaybeUpdate(enemy.gameObject);
    }
    
    // Enqueue the enemy and it's behavior script that enters radius.
    private void OnTriggerEnter(Collider other)
    {
        if (!_tower.IsPlaced) return;
        if (!other.CompareTag("Enemy")) return;
        Debug.Log($"Tower {_tower.GetInstanceID().ToString()} should now start targeting enemy {other.GetInstanceID().ToString()}");
        TrackTarget(other.gameObject);
        UpdateTarget();
        
        if (_enemiesInAttackRadius.Count > 0)
        {
            Debug.DrawRay(transform.position, transform.position - other.gameObject.transform.position, Color.magenta);
        }
    }

    // Add target for tower to attack and remove it from attack radius if it dies.
    private void OnTriggerStay(Collider other)
    {
        if (!_tower.IsPlaced) return;
        if (!other.CompareTag("Enemy")) return;
        if (!_enemiesInAttackRadius.ContainsKey(other.gameObject))
        {
            TrackTarget(other.gameObject);
            UpdateTarget();
        }
        
        if (_enemiesInAttackRadius.Count > 0) {
            Debug.DrawRay(
                transform.position, _enemiesInAttackRadius.First().Value.transform.position - transform.position,
                Color.red);
            if (_enemiesInAttackRadius.Count > 1)
            {
                for (int i = 1; i < _enemiesInAttackRadius.Count; i++)
                {
                    Debug.DrawRay(
                        transform.position,
                        _enemiesInAttackRadius.ElementAt(i).Value.transform.position -
                        transform.position,
                        Color.blue);
                }
            }
        }
    }

    // Remove enemy from attack radius tracking if it exits.
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        RemoveTargetFromTrackingAndMaybeUpdate(other.gameObject);
        
        if (_enemiesInAttackRadius.Count > 0)
        {
            Debug.DrawRay(transform.position, transform.position - other.gameObject.transform.position, Color.green);
            Debug.Log($"Mech {other.gameObject.GetInstanceID().ToString()} exited tower {this.GetInstanceID().ToString()}");
        }
    }
}
