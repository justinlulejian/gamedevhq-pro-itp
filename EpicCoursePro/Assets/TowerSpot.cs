using UnityEngine;

public class TowerSpot : MonoBehaviour
{
    public bool IsAvailableForPlacement { get; set; }

    [SerializeField] 
    private ParticleSystem _availableParticleSystem;

    private void OnEnable()
    {
        TowerManager.onActivateTowers += ActivateTower;
        TowerManager.onDeactivateTowers += DeactivateTower;
    }

    private void OnDisable()
    {
        TowerManager.onActivateTowers -= ActivateTower;
        TowerManager.onDeactivateTowers += DeactivateTower;
    }
    
    private void Awake()
    {
        _availableParticleSystem = GetComponent<ParticleSystem>();

        if (_availableParticleSystem == null)
        {
            Debug.Log("Particle system is null on tower spot.");
        }
        // _availableParticleSystem.Play();
    }
    

    public void ActivateTower()
    {
        Debug.Log($"Activating tower");
        IsAvailableForPlacement = false;
        _availableParticleSystem.Play();
    }
    
    public void DeactivateTower()
    {
        Debug.Log($"Deactivating tower");
        IsAvailableForPlacement = true;
        _availableParticleSystem.Stop();
    }
}
 
