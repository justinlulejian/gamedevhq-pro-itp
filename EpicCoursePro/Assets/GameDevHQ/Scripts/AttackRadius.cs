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
    
    private OrderedDictionary<GameObject, Enemy> _enemiesInAttackRadius =
        new OrderedDictionary<GameObject, Enemy>();

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

    private Enemy GetEnemyComponentFromCollider(Collider enemyCollider)
    {
        Enemy enemy = enemyCollider.gameObject.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError($"Enemy {enemyCollider.name} that entered attack radius of " +
                           $"tower {name} does not have a behavior script attached.");
            return null;
        }

        return enemy;
    }
    
    // Enqueue the enemy and it's behavior script that enters radius.
    private void OnTriggerEnter(Collider other)
    {
        if (!_tower.IsPlaced) return;
        if (!other.CompareTag("Enemy")) return;
        _enemiesInAttackRadius[other.gameObject] = GetEnemyComponentFromCollider(other);
    }

    // Attack an enemy and remove it from attack radius if it dies.
    private void OnTriggerStay(Collider other)
    {
        if (!_tower.IsPlaced || _enemiesInAttackRadius.Count == 0) return;
        // Handle when we place a tower after an enemy has already collided with radius.
        if (!_enemiesInAttackRadius.ContainsKey(other.gameObject))
        {
            _enemiesInAttackRadius[other.gameObject] = GetEnemyComponentFromCollider(other);
        }
        
        Enemy enemyToTarget = _enemiesInAttackRadius.First().Value;
        _tower.EnemyInAttackRadius(enemyToTarget);

        if (enemyToTarget.IsDead)
        {
            _enemiesInAttackRadius.Remove(other.gameObject);
        }
    }

    // Remove enemy from attack radius tracking if it exits.
    private void OnTriggerExit(Collider other)
    {
        if (!_tower.IsPlaced) return;
        _enemiesInAttackRadius.Remove(other.gameObject);

        if (_enemiesInAttackRadius.Count == 0)
        {
            _tower.NoEnemiesInAttackRadius();
        }
    }
}
