using System;
using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameDevHQ.Scripts
{
    // TODO: Still needs to be abstract or can be regular class?
    public abstract class Tower : AbstractTower
    {
        public bool IsPlaced { get; set; }
        public bool IsUpgradeAble => TowerInfo.UpgradeSprite != null;
        
        [Header("Target Settings")]
        private AttackRadius _attackRadius;
        [SerializeField]
        protected Enemy _targetedEnemy;
        [SerializeField]
        protected bool _firingAtEnemy;
        [SerializeField]
        protected int _damageValue = 20;
        [SerializeField] 
        protected float _damageRate = 1f;
        protected float _canFire;
        

        [Header("Rotation Settings")]
        [SerializeField] 
        private GameObject _rotationObject;
        [SerializeField]
        private float _rotationSpeed = 5f;
        private Transform _rotationTransform;
        private Quaternion _originalRotation;

        protected abstract void StopAttacking();

        protected abstract void ResetFiringState();
        
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
            
            if (_attackRadius == null)
            {
                Debug.LogError($"Attack radius was null on tower {this.name}.");
            }
        }

        protected virtual void Update()
        {
            if (_targetedEnemy)
            {
                RotateTowardsTarget(_targetedEnemy);
            }
        }

        private void RotateTowardsTarget(Enemy enemy)
        {
            // TODO: Clamp the rotation so it doesn't rotate too oddly depending on where enemy is.
            Vector3 direction = enemy.WeaponTargetTransform.position - _rotationTransform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            _rotationTransform.rotation =
                Quaternion.Slerp(_rotationTransform.rotation, lookRotation,
                    _rotationSpeed * Time.deltaTime);
        }
        
        // Resets the rotation while no targets, but will stop if a new enemy is being targeted.
        protected IEnumerator ResetRotation()
        {
            // comparing rotation would prevent this from running forever.
            Quaternion defaultRot = TowerSpot.TowerFacingEnemiesRotation;
            while (_targetedEnemy == null && _rotationTransform.rotation != defaultRot)
            {
                _rotationTransform.rotation =
                    Quaternion.Slerp(_rotationTransform.rotation, defaultRot,
                        _rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // How the tower will react to enemies within it's attack radius.
        public void UpdateAttackTarget(Enemy enemy)
        {
            if (enemy == null)
            {
                _targetedEnemy = null;
                StopAttacking();
                StartCoroutine(ResetRotation());
                return;
            }
           
            // TODO: Necessary check? Avoid updating things if not necessary.
            if (enemy == _targetedEnemy) return;
            _targetedEnemy = enemy;
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