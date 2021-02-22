using System;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevHQ.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [field: Header("Navigation and speed components")]
        [field: SerializeField]
        [field: Tooltip("The end destination Vector3 the enemy will attempt to navigate to.")]
        private Vector3 _navDestinationPosition;

        [field: SerializeField]
        [field: Tooltip("The speed at which the enemy will approach the navigation target.")]
        protected float _navigationSpeed = 1.5f;

        private NavMeshAgent _navMeshAgent;

        [SerializeField] 
        [Header("Health Settings")]
        private int health = 100;

        [Header("Currency Settings")]
        [SerializeField] 
        [Tooltip("The currency value provided when the enemy is killed by player.")]
        private int warFundValue = 100;

        // When an enemy is enabled it will invoke this event.
        public static event Action<Transform, NavMeshAgent> OnSpawnStart;

        public float GetSpeed()
        {
            return _navigationSpeed;
        }
        
        private void OnEnable()
        {
            if (_navMeshAgent == null)
            {
                _navMeshAgent = GetComponent<NavMeshAgent>();
            }
            OnSpawnStart.Invoke(this.transform, _navMeshAgent);
            _navDestinationPosition = _navMeshAgent.destination;
        }

        private void Start()
        {
            if (_navMeshAgent == null)
            {
                Debug.LogError($"Nav mesh agent is null on enemy: {this.gameObject.name}");
            }
            _navMeshAgent.speed = _navigationSpeed;
        }
        
        public void Damage(int damageValue)
        {
            Mathf.Min(0, health -= damageValue);

            if (health <= 0)
            {
                Kill();
            }
        }
        
        private void Kill()
        {
            Destroy(this.gameObject);
        }
    }
}
