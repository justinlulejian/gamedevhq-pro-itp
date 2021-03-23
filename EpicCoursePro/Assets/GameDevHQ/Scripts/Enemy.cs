using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rock.Collections;
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

        [Header("Health Settings")]
        [SerializeField] 
        private int _maxHealth = 100;
        [SerializeField]
        private int _currentHealth; 
        public bool IsDead => _currentHealth == 0;
        private CanvasHealthBar _healthBar;
        [SerializeField]
        private GameObject _healthBarGameObject;
        
        [Header("Currency Settings")]
        [SerializeField] 
        [Tooltip("The currency value provided when the enemy is killed by player.")]
        public int WarFundValue = 100;

        [Header("Targeting Settings")]
        [SerializeField]
        [Tooltip(
            "The object player weapons will target when firing. Must have transform attached.")]
        private GameObject _playerWeaponTarget;
        [SerializeField] 
        private  GameObject _currentTarget;
        private OrderedHashSet<GameObject> _targetList = new OrderedHashSet<GameObject>();
        [SerializeField] 
        private GameObject _waistToTurnTowardsTarget;
        private Quaternion _waistOriginalRotation;
        private Transform _weaponTargetTransform;

        [Header("Animation Settings")]
        [Tooltip("Time to wait for enemy to play it's death animation before recycling to pool.")]
        public float DeathAnimWaitTime = 5f;
        private Animator _animator;
        public GameObject _deathExplosionPrefab;
        [SerializeField]
        protected List<Renderer> _dissolveMeshRenderers = new List<Renderer>();
        
        // When an enemy is enabled it will invoke this event.
        public static event Action<Transform, NavMeshAgent> onSpawnStart;
        public static event Action<Enemy> onEnemyKilledByPlayer;
        public static event Action<Enemy> onEnemyCompletedDeathAnimation;
        
        private void OnEnable()
        {
            AttackRadius.OnEnemyEnterTowerRadius += AddTarget;
            AttackRadius.OnEnemyExitTowerRadius += RemoveTarget;
            
            if (_navMeshAgent == null)
            {
                _navMeshAgent = GetComponent<NavMeshAgent>();
            }

            _navMeshAgent.enabled = true;
            _healthBar?.gameObject?.SetActive(true);
            onSpawnStart?.Invoke(this.transform, _navMeshAgent);
            _navDestinationPosition = _navMeshAgent.destination;
        }
        
        private void OnDisable()
        {
            AttackRadius.OnEnemyEnterTowerRadius -= AddTarget;
            AttackRadius.OnEnemyExitTowerRadius -= RemoveTarget;
            
            // Reset health so if recycled they'll start with
            PrepareEnemyForRecycling();
        }
        
        protected virtual void Awake()
        {
            if (EnemyType == 0)
            {
                Debug.LogError($"Enemy {name} does not have a type, assign a type for" +
                               $" proper usage in-game.");
            }
            
            _waistOriginalRotation = _waistToTurnTowardsTarget.transform.rotation;
            if (_waistToTurnTowardsTarget == null)
            {
                Debug.LogError($"Enemy {name} does not have a waist to rotate while " +
                               $"firing. Attach one to allow firing to work.");
            }
            
            
            WeaponTargetTransform = _playerWeaponTarget.transform;
            if (WeaponTargetTransform == null)
            {
                Debug.LogError($"Enemy {name} does not have a valid weapon target. Make " +
                               $"sure target object has transform attached.");
            }

            _animator = GetComponent<Animator>();
            // TODO: Had to move animation to child of mech2 due to https://forum.unity.com/threads/animation-stops-object-movement.184084/ 
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }
            
            _dissolveMeshRenderers = GetComponentsInChildren<Renderer>().ToList();
            _dissolveMeshRenderers.RemoveAll(
                r => r is SpriteRenderer);

            if (_animator == null)
            {
                Debug.LogError($"Animator is null on enemy {name}");
            }
            if (_dissolveMeshRenderers.Count < 0)
            {
                Debug.LogError($"Mesh renderer is non-existent on enemy {name}");
            }
            if (_deathExplosionPrefab == null)
            {
                Debug.LogError($"Death explosion prefab is null on enemy {name}");
            }
        }

        public float GetSpeed()
        {
            return _navigationSpeed;
        }

        private void PrepareEnemyForRecycling()
        {
            // Reset health so if recycled they'll start again with full health.
            _currentHealth = _maxHealth;
            // TODO: Why do I need both null propogations here?
            _healthBar?.gameObject?.SetActive(false);
            _animator.ResetTrigger("IsEnemyFiring");
            _animator.SetTrigger("OnEnemyDeath");
            _animator.WriteDefaultValues(); // Reset position of mech to upright.
            _navMeshAgent.enabled = false;
            
            // Reset dissolve shader value to opaque.
            foreach (var meshRenderer in _dissolveMeshRenderers)        
            {
                meshRenderer.material.SetFloat(
                    "_fillAmount", 0.0f);
            }
        }

        protected virtual void Start()
        {
            if (_navMeshAgent == null)
            {
                Debug.LogError($"Nav mesh agent is null on enemy: {this.gameObject.name}");
            }
            _navMeshAgent.speed = _navigationSpeed;
            
            _healthBar = HealthbarUIManager.Instance.RequestHealthbar(gameObject);
            _healthBarGameObject = _healthBar.gameObject;
            if (_healthBar == null)
            {
                Debug.LogError($"Health bar is null on enemy {name}");
            }
            
        }

        private void Update()
        {
            // TODO: Move from Update to a coroutine?
            if (_currentTarget != null)
            {
                // TODO: Change to slerp to make the tracking look smoother.
                _waistToTurnTowardsTarget.transform.LookAt(_currentTarget.transform.position);
            }
        }

        private void AddTarget(GameObject enemy, GameObject target)
        {
            if (!(enemy == gameObject)) return;
            _animator.SetTrigger("IsEnemyFiring");
            if (!_targetList.Contains(target))
            {
                _targetList.Add(target);
                _currentTarget = _targetList.First();
;            }
        }

        private void RemoveTarget(GameObject enemy, GameObject target)
        {
            if (!(enemy == gameObject)) return;
            _targetList.Remove(target);
            if (_targetList.Count == 0)
            {
                _currentTarget = null;
                ResetRotation();
                return;
            }

            _currentTarget = _targetList.First();
        }

        private void ResetRotation()
        {
            _animator.ResetTrigger("IsEnemyFiring");
            _waistToTurnTowardsTarget.transform.rotation = transform.rotation;
        }

        public Transform WeaponTargetTransform
        {
            get => _weaponTargetTransform;
            private set => _weaponTargetTransform = value;
        }
        
        private IEnumerator DissolveRoutine(float dissolveTime)
        {
            // TODO: static value to wait for explosion to finish. Change to check for explosion
            // finish.
            yield return new WaitForSeconds(1.5f);
            // TODO: Change this to do in steps across the death/dissolveTime vs statically like
            // this.
            float dissolveValue = 0;
            while (dissolveValue < 1)
            {
                dissolveValue += 0.01f;
                foreach (var meshRenderer in _dissolveMeshRenderers)        
                {
                    meshRenderer.material.SetFloat("_fillAmount", dissolveValue);
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void PlayDeathExplosion()
        {
            GameObject deathExplosion = Instantiate(
                _deathExplosionPrefab, transform.position, Quaternion.identity);
            // 1.5 is static value I've set the particle to run for on prefab.
            Destroy(deathExplosion, 1.5f);
        }
        
        private IEnumerator WaitBeforeDespawn(Enemy enemy)
        {
            StartCoroutine(DissolveRoutine(enemy.DeathAnimWaitTime));
            yield return new WaitForSeconds(enemy.DeathAnimWaitTime);
            onEnemyCompletedDeathAnimation?.Invoke(this);
        }
        
        private void AnimateDeath()
        {
            _animator.SetTrigger("OnEnemyDeath");
            _navMeshAgent.enabled = false;
            PlayDeathExplosion();
        }

        public void PlayerDamageEnemy(int damageValue)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - damageValue, 0, _maxHealth);
            // TODO: Make is to that healthbar only shows up when enemy has been damaged.
            _healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
            
            if (_currentHealth == 0)
            {
                onEnemyKilledByPlayer.Invoke(this);
                AnimateDeath();
                StartCoroutine(WaitBeforeDespawn(this));
            }
        }
    }
}
