using System;
using GameDevHQ.Scripts;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerSpot : MonoBehaviour
{
    public bool IsAvailableForPlacement { get; private set; }

    [SerializeField] 
    private ParticleSystem _availableParticleSystem;

    [SerializeField] 
    private Tower _towerPlaced;

    [SerializeField] 
    public bool IsUpgraded;
    
    // This rotation faces towers towards enemy start position.
    public static Quaternion TowerFacingEnemiesRotation = Quaternion.Euler(0, -90, 0);

    public static event Action<TowerSpot> onUserPreviewTowerOnSpotIntent; 
    public static event Action onUserNoLongerPlaceTowerOnSpotPreviewIntent; 
    public static event Action<TowerSpot> onUserPlaceTowerIntent;
    public static event Action<PlayerUIManager.UIStates, TowerSpot> onUserTowerInteractIntent;
    
    private void OnEnable()
    {
        TowerManager.onTowerPlacementModeStatusChange += OnTowerPlacementModeChange;
        DismantleTowerUIManager.onDismantleTower += DismantleTower;
    }

    private void OnDisable()
    {
        TowerManager.onTowerPlacementModeStatusChange -= OnTowerPlacementModeChange;
        DismantleTowerUIManager.onDismantleTower -= DismantleTower;
    }

    private void Awake()
    {
        _availableParticleSystem = GetComponent<ParticleSystem>();

        if (_towerPlaced == null)
        {
            IsAvailableForPlacement = true;
        }

        if (_availableParticleSystem == null)
        {
            Debug.LogError("Particle system is null on tower spot.");
        }
    }

    public void OnTowerPlacementModeChange(bool modeEnabled)
    {
        if (modeEnabled && IsAvailableForPlacement)
        {
            _availableParticleSystem.Play();
        }
        else
        {
            _availableParticleSystem.Stop();
        }
    }
    
    // Swap decoy tower for original tower prefab on spot.
    private void OnMouseEnter()
    {
        if (IsAvailableForPlacement && TowerManager.Instance.IsTowerPlacementModeActivated())
        {
            onUserPreviewTowerOnSpotIntent.Invoke(this);
            _availableParticleSystem.Stop();
        }
    }
    
    // Reactivate decoy tower and deactivate un-placed tower.
    private void OnMouseExit()
    {
        if (IsAvailableForPlacement && TowerManager.Instance.IsTowerPlacementModeActivated())
        {
            onUserNoLongerPlaceTowerOnSpotPreviewIntent?.Invoke();
            _availableParticleSystem.Play();
        }
    }
    
    // Place tower in spot, don't allow it to be removed by mouse exit.
    private void OnMouseOver()
    {
        // TODO(checkpoint): switch this to onmouseover and then check for mouse button down to 
        // either call placement/upgrade or dismantle. Then swithc upgradegunui to not call dismantle UI
        // then make sure dismantle UI is started from here as monosingleton, or from an event.
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
        {
            if (TowerManager.Instance.IsTowerPlacementModeActivated() && !_towerPlaced)
            {
                if (IsAvailableForPlacement && TowerManager.Instance.IsTowerPlacementModeActivated())
                {
                    onUserPlaceTowerIntent?.Invoke(this);
                    return;
                } 
            }

            if (_towerPlaced)
            {
                // TODO: Change this to be dynamic check for different tower types.
                if (_towerPlaced.TowerType == 1)
                {
                    onUserTowerInteractIntent?.Invoke(
                        PlayerUIManager.UIStates.UpgradeGatlingTowerUIState, this);
                } else if (_towerPlaced.TowerType == 2)
                {
                    onUserTowerInteractIntent?.Invoke(
                        PlayerUIManager.UIStates.UpgradeMissileTowerUIState, this);
                }
                return;
            }
        }

        if (Input.GetMouseButtonDown((int) MouseButton.RightMouse) &&
            !TowerManager.Instance.IsTowerPlacementModeActivated())
        {
            onUserTowerInteractIntent?.Invoke(
                PlayerUIManager.UIStates.DismantleTowerUIState, this);
        }
    }

    public Tower GetTowerPlacedOnSpot()
    {
        if (_towerPlaced)
        {
            return _towerPlaced;
        }
        else
        {
            Debug.LogError($"Requested placed tower from Tower spot ID" +
                           $" {this.GetInstanceID().ToString()} but it is not set.");
            return null;
        }
    }
    
    public void PlaceTower(GameObject towerToPlace, bool upgrade = false)
    {
        Tower tower = towerToPlace.GetComponent<Tower>();
        if (tower == null)
        {
            Debug.LogError($"Tower {towerToPlace.name} has been placed in tower spot" +
                           $" {name}, but the tower does not have a behavior script. Tower will" +
                           $"not be placed.");
            return;
        }
        if (upgrade)
        {
            PoolManager.Instance.RecyclePooledObj(_towerPlaced.gameObject);
        }
        
        _towerPlaced = tower;
        _towerPlaced.transform.SetPositionAndRotation(
            transform.position, TowerFacingEnemiesRotation);
        tower.IsPlaced = true;
        IsUpgraded = true;
        tower.EnableAttackRadiusCollider();
        IsAvailableForPlacement = false;
    }

    private void DismantleTower(TowerSpot towerSpot)
    {
        if (gameObject == towerSpot.gameObject)
        {
            IsUpgraded = false;
            OnTowerPlacementModeChange(true);
            IsAvailableForPlacement = true;
            
            // TODO: make first condition the default once upgraded towers are pooled.
            if (_towerPlaced.TowerType == 1 || _towerPlaced.TowerType == 2)
            {
                PoolManager.Instance.RecyclePooledObj(_towerPlaced.gameObject);
                _towerPlaced.IsPlaced = false;
                
            } else if (_towerPlaced.TowerType == 3 || _towerPlaced.TowerType == 4)
            {
                Destroy(_towerPlaced.gameObject);
            }
            _towerPlaced = null;
        }
    }
}
 
