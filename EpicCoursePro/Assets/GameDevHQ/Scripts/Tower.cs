using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameDevHQ.Scripts
{
    // TODO: Still needs to be abstract or can be regular class?
    public abstract class Tower : AbstractTower
    {
        [Header("Target Settings")]
        private AttackRadius _attackRadius;
        [SerializeField]
        protected Enemy _currentTargetedEnemy;
        [SerializeField]
        protected Enemy _previousTargetedEnemy;
        [SerializeField]
        protected bool _firingAtEnemy;
        [SerializeField]
        protected int _damageValue = 20;
        [SerializeField] 
        protected float _damageRate = 1f;
        protected float _canFire;

        public bool IsPlaced { get; set; }
        [Header("Rotation Settings")]
        [SerializeField] 
        private GameObject _rotationObject;
        [SerializeField]
        private float _rotationSpeed = 5f;
        private Transform _rotationTransform;
        private Quaternion _originalRotation;

        protected abstract void StopAttacking();

        protected override void Awake()
        {
            base.Awake();
            
            _attackRadius = AttackRadiusObj.GetComponent<AttackRadius>();
            _rotationTransform = _rotationObject.transform;
            _originalRotation = _rotationObject.transform.rotation;
            
            if (_attackRadius == null)
            {
                Debug.LogError($"Attack radius was null on tower {this.name}.");
            }
            if (_rotationTransform == null)
            {
                Debug.LogError($"Rotation transform was null on tower {this.name}.");
            }
        }
        
        protected virtual void Update()
        {
            // if (_currentTargetedEnemy)
            // {
            //     RotateTowardsTarget(_currentTargetedEnemy);
            // }
        }

        protected void RotateTowardsTarget()
        {
            // TODO: Clamp the rotation so it doesn't rotate too oddly depending on where enemy is.
            Vector3 direction = _currentTargetedEnemy.WeaponTargetTransform.position - _rotationTransform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            _rotationTransform.rotation =
                Quaternion.Slerp(_rotationTransform.rotation, lookRotation,
                    _rotationSpeed * Time.deltaTime);
        }
        
        // Resets the rotation while no targets, but will stop if a new enemy is being targeted.
        private IEnumerator ResetRotation()
        {
            // comparing rotation would prevent this from running forever.
            while (_currentTargetedEnemy == null && _rotationTransform.rotation != _originalRotation)
            {
                _rotationTransform.rotation =
                    Quaternion.Slerp(_rotationTransform.rotation, _originalRotation,
                        _rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // How the tower will react to enemies within it's attack radius.
        public void UpdateAttackTarget(Enemy enemy)
        {
            if (enemy == null)
            {
                _currentTargetedEnemy = null;
                StartCoroutine(ResetRotation());
                StopAttacking();
                return;
            }
           
            // TODO: Necessary check? Avoid updating things if not necessary.
            if (enemy == _currentTargetedEnemy) return;
            _previousTargetedEnemy = _currentTargetedEnemy;
            _currentTargetedEnemy = enemy;
            // RotateTowardsTarget(enemy);  // moved to update temporarily
            // TODO: Figure out a way to start the firing process once rotation is close to being
            // done to make it look more natural.
            // StartFiringAtEnemy(enemy);
        }
        public void EnableAttackRadiusCollider()
        {
            _attackRadius.EnableCollider();
        }
    }
}