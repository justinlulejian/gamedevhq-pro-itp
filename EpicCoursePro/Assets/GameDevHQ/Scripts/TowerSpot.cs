using System;
using System.Security.Cryptography;
using GameDevHQ.Scripts;
using UnityEngine;

public class TowerSpot : MonoBehaviour
{
    public bool IsAvailableForPlacement { get; private set; }

    [SerializeField] 
    private ParticleSystem _availableParticleSystem;

    [SerializeField] 
    private GameObject _towerPlaced;

   

    // TODO: wrap all my .Invokes() in null checks to make sure we aren't invoking with no listeners.
    public static event Action<TowerSpot> onUserMouseEnterTowerSpot; 
    public static event Action onUserMouseExitTowerSpot; 
    public static event Action<TowerSpot> onUserMouseDownTowerSpot;
    
    private void OnEnable()
    {
        TowerManager.onTowerPlacementModeStatusChange += OnTowerPlacementModeChange;
    }

    private void OnDisable()
    {
        TowerManager.onTowerPlacementModeStatusChange -= OnTowerPlacementModeChange;
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
        IsAvailableForPlacement = modeEnabled;
        if (modeEnabled)
        {
            _availableParticleSystem.Play();
        }
        else
        {
            _availableParticleSystem.Stop();
        }
    }
    
    // Swap decoy tower for original tower prefab.
    private void OnMouseEnter()
    {
        if (IsAvailableForPlacement)
        {
            onUserMouseEnterTowerSpot.Invoke(this);
            _availableParticleSystem.Stop();
        }
    }
    
    // Reactivate decoy tower and deactivate un-placed tower.
    private void OnMouseExit()
    {
        if (IsAvailableForPlacement)
        {
            onUserMouseExitTowerSpot.Invoke();
            _availableParticleSystem.Play();
        }
    }
    
    // Place tower in spot, don't allow it to be removed by mouse exit.
    private void OnMouseDown()
    {
        if (IsAvailableForPlacement)
        {
            onUserMouseDownTowerSpot.Invoke(this);
        }
    }
    
    public void PlaceTower(GameObject towerToPlace)
    {
        _towerPlaced = towerToPlace;
        IsAvailableForPlacement = false;
    }
    
}
 
