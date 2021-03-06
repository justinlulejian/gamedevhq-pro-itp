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
        if (_enemiesInAttackRadius.Count > 0)
        {
            _tower.UpdateAttackTarget(_enemiesInAttackRadius.First().Value);
        }
        else
        {
            _tower.UpdateAttackTarget(null);
        }
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
        TrackTarget(other.gameObject);
        UpdateTarget();
    }

    // Add target for tower to attack and remove it from attack radius if it dies.
    // private void OnTriggerStay(Collider other)
    // {
    //     // TODO: Do we really care if it's staying in? We just fire from enter to exit so do we need
    //     // to do that in stay? Or do we need to if radius spawns over enemy?
    //     // if (!_tower.IsPlaced || _enemiesInAttackRadius.Count == 0) return;
    //     // UpdateTarget();
    // }

    // Remove enemy from attack radius tracking if it exits.
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        RemoveTargetFromTrackingAndMaybeUpdate(other.gameObject);
    }
}
