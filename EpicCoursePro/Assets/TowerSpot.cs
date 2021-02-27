using System;
using UnityEngine;

public class TowerSpot : MonoBehaviour
{
    public bool IsAvailableForPlacement { get; set; }

    [SerializeField] 
    private ParticleSystem _availableParticleSystem;
    
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

        if (_availableParticleSystem == null)
        {
            Debug.Log("Particle system is null on tower spot.");
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
}
 
