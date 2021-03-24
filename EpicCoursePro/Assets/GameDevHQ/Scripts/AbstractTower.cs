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

        protected Color _attackRadiusEnabledColor = new Color(
            71 / 255.0f, 255 / 255.0f, 134 / 255.0f, 134/ 255.0f);
        protected Color _attackRadiusDisabledColor = new Color(
            255 / 255.0f, 17 / 255.0f, 0 / 255.0f, 134/ 255.0f);

        protected virtual void OnEnable()
        {
            TowerManager.onTowerSpotPreview += TurnOnAttackRadius;
            TowerManager.onTowerPlaced += TurnOffAttackRadius;
            TowerManager.onTowerPlacementModeStatusChange += ChangeAttachRadiusVisible;
        }

        protected virtual void OnDisable()
        {
            TowerManager.onTowerSpotPreview -= TurnOnAttackRadius;
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