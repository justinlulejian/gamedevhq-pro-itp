using System;
using UnityEngine;

namespace GameDevHQ.Scripts
{
    public abstract class Tower : AbstractTower
    {
        private AttackRadius _attackRadius;
        public bool IsPlaced { get; set; }

        protected override void Awake()
        {
            base.Awake();
            
            _attackRadius = AttackRadiusObj.GetComponent<AttackRadius>();
            
            if (_attackRadius == null)
            {
                Debug.LogError($"Attack radius was null on tower {this.name}.");
            }
        }

        // How the tower will react to enemies within it's attack radius.
        public abstract void EnemyInAttackRadius(Enemy enemy);

        public void EnableAttackRadiusCollider()
        {
            _attackRadius.EnableCollider();
        }
        
    }
}