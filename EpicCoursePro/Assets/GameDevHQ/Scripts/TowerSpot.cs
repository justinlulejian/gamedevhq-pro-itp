using System;
using GameDevHQ.Scripts;
using GameDevHQ.Scripts.UI;
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
    // public static event Action<PlayerUIManager.UIStates, TowerSpot> onUserTowerInteractIntent;

    private void OnEnable()
    {
        TowerManager.onTowerPlacementModeStatusChange += OnTowerPlacementModeChange;
        // DismantleTowerInteractableChoiceUIManager.onDismantleTower += DismantleTower;
    }

    private void OnDisable()
    {
        TowerManager.onTowerPlacementModeStatusChange -= OnTowerPlacementModeChange;
        // DismantleTowerInteractableChoiceUIManager.onDismantleTower -= DismantleTower;
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
    
    // Left click places tower if none is placed. Otherwise it will attempt upgrade. Right-click
    // open dismantle tower UI if tower is present
    private void OnMouseOver()
    {
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
               PlayerUIManager.Instance.EnableTowerUpgradeUI(this, _towerPlaced.TowerInfo);
               return;
            }
        }
        
        if (Input.GetMouseButtonDown((int) MouseButton.RightMouse) &&
            !TowerManager.Instance.IsTowerPlacementModeActivated() && _towerPlaced)
        {
            PlayerUIManager.Instance.EnableTowerDismantleUI(this, _towerPlaced.TowerInfo);
        }
    }

    public Tower GetTowerPlacedOnSpot()
    {
        if (_towerPlaced)
        {
            return _towerPlaced;
        }

        Debug.LogError($"Requested placed tower from Tower spot ID" +
                       $" {this.GetInstanceID().ToString()} but it is not set.");
        return null;
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

    public void DismantleTower()
    {
        IsUpgraded = false;
        OnTowerPlacementModeChange(true);
        IsAvailableForPlacement = true;
        _towerPlaced.IsPlaced = false;
        PoolManager.Instance.RecyclePooledObj(_towerPlaced.gameObject);
        _towerPlaced = null;
    }
}
 
