using System;
using System.Collections;
using UnityEngine;

namespace GameDevHQ.Scripts
{
    // TODO: Still needs to be abstract or can be regular class?
    public abstract class Tower : AbstractTower
    {
        [Header("Target Settings")]
        private AttackRadius _attackRadius;
        private bool _targetingEnemy;
        public bool IsPlaced { get; set; }
        [Header("Rotation Settings")]
        [SerializeField] 
        private GameObject _rotationObject;
        [SerializeField]
        private float _rotationSpeed = 5f;
        private Transform _rotationTransform;
        private Quaternion _originalRotation;
        

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

        private void RotateTowardsTarget(Enemy enemy)
        {
            // TODO: Clamp the rotation so it doesn't rotate too oddly depending on where enemy is.
            Vector3 direction = enemy.transform.position - _rotationTransform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            _rotationTransform.rotation =
                Quaternion.Slerp(_rotationTransform.rotation, lookRotation,
                    _rotationSpeed * Time.deltaTime);
        }
        
        // Resets the rotation while, but will stop if a new enemy is being targeted.
        private IEnumerator ResetRotation()
        {
            while (!_targetingEnemy)
            {
                _rotationTransform.rotation =
                    Quaternion.Slerp(_rotationTransform.rotation, _originalRotation,
                        _rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private void FireAtEnemy(Enemy enemy)
        {
            // TODO
            return;
        }

        // How the tower will react to enemies within it's attack radius.
        public void EnemyInAttackRadius(Enemy enemy)
        {
            if (!IsPlaced) return;
            _targetingEnemy = true;
            RotateTowardsTarget(enemy);
            FireAtEnemy(enemy);
        }
        
        public void NoEnemiesInAttackRadius()
        {
            if (!IsPlaced) return;
            _targetingEnemy = false;
            StartCoroutine(ResetRotation());
        }

        public void EnableAttackRadiusCollider()
        {
            _attackRadius.EnableCollider();
        }
        
    }
}