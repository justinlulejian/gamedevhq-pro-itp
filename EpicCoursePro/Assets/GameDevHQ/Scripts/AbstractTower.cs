using UnityEngine;

namespace GameDevHQ.Scripts
{
    public abstract class AbstractTower : MonoBehaviour
    {
        [Header("Tower Info")]
        [SerializeField]
        public TowerInfo TowerInfo;
        public int TowerType;
        
        // TODO: Fix the visibility of these to only what's necessary.
        public GameObject AttackRadiusObj;
        protected MeshRenderer _attackRadiusMeshRenderer;

        protected virtual void OnEnable()
        {
            TowerManager.onTowerPreview += TurnOnAttackRadius;
            TowerManager.onTowerPlaced += TurnOffAttackRadius;
            TowerManager.onTowerPlacementModeStatusChange += ChangeAttachRadiusVisible;
        }

        protected virtual void OnDisable()
        {
            TowerManager.onTowerPreview -= TurnOnAttackRadius;
            TowerManager.onTowerPlaced -= TurnOffAttackRadius;
            TowerManager.onTowerPlacementModeStatusChange -= ChangeAttachRadiusVisible;
        }
        
        protected virtual void Awake()
        {
            if (AttackRadiusObj == null)
            {
                Debug.LogError($"Attack radius object was null on tower {this.name}.");
            }

            _attackRadiusMeshRenderer = AttackRadiusObj.GetComponent<MeshRenderer>();
            
            if (_attackRadiusMeshRenderer == null)
            {
                Debug.LogError($"Attack radius mesh renderer was null on tower {this.name}.");
            }

            VerifyTowerInfo();
        }

        protected virtual void VerifyTowerInfo()
        {
            if (TowerInfo.Equals(default(TowerInfo)))
            {
                Debug.LogError($"Tower {name} id: {this.GetInstanceID().ToString()} is " +
                               $"missing it's TowerInfo.");
            }
        }
        
        protected void TurnOnAttackRadius()
        {
            _attackRadiusMeshRenderer.enabled = true;
        }
        
        protected void TurnOffAttackRadius()
        {
            _attackRadiusMeshRenderer.enabled = false;
        }

        private void ChangeAttachRadiusVisible(bool visible)
        {
            _attackRadiusMeshRenderer.enabled = visible;
        }
    }
}