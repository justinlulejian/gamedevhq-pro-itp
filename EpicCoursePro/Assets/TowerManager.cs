using System;
using UnityEngine;

public class TowerManager : MonoSingleton<TowerManager>
{
    [SerializeField] 
    private GameObject _towerSpotContainer;

    public static event Action onActivateTowers;
    public static event Action onDeactivateTowers;

    private void AddTowerToContainer(TowerSpot spot)
    {
        spot.transform.parent = _towerSpotContainer.transform;
    }

    private void Update()
    {
        // TODO: Test code to test tower particle system activation.
        if (Input.GetKeyDown(KeyCode.O))
        {
            onActivateTowers.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            onDeactivateTowers.Invoke();
        }
    }
}
