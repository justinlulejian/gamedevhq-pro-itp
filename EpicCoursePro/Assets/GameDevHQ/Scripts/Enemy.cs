using System;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevHQ.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [Header("Generic Enemy Settings")]
        [Tooltip("The unique ID of the enemy, must something other than 0 (zero).")]
        public int EnemyType;

        [field: Header("Navigation and Speed Components")]
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
        public bool IsDead => health == 0;
        

        [Header("Currency Settings")]
        [SerializeField] 
        [Tooltip("The currency value provided when the enemy is killed by player.")]
        public int WarFundValue = 100;

        [Header("Targeting Settings")]
        [SerializeField]
        [Tooltip(
            "The object player weapons will target when firing. Must have transform attached.")]
        private GameObject _weaponTarget;
        private Transform _weaponTargetTransform;

        // When an enemy is enabled it will invoke this event.
        public static event Action<Transform, NavMeshAgent> onSpawnStart;
        public static event Action<Enemy> onEnemyKilledByPlayer;

        private void Awake()
        {
            if (EnemyType == 0)
            {
                Debug.LogError($"Enemy {name} does not have a type, assign a type for" +
                               $" proper usage in-game.");
            }
            
            WeaponTargetTransform = _weaponTarget.transform;

            if (WeaponTargetTransform == null)
            {
                Debug.LogError($"Enemy {name} does not have a valid weapon target. Make " +
                               $"sure target object has transform attached.");
            }
        }

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
            onSpawnStart?.Invoke(this.transform, _navMeshAgent);
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

        public Transform WeaponTargetTransform
        {
            get => _weaponTargetTransform;
            private set => _weaponTargetTransform = value;
        }

        public void PlayerDamageEnemy(int damageValue)
        {
            Mathf.Min(0, health -= damageValue);

            if (health == 0)
            {
                onEnemyKilledByPlayer?.Invoke(this);
            }
        }
    }
}
