using System;
using System.Collections.Generic;
using System.Linq;
using GameDevHQ.Scripts;
using UnityEngine;

public class TowerManager : MonoSingleton<TowerManager>
{
    [SerializeField] 
    private GameObject _decoyTowerContainer;

    [SerializeField] 
    private List<GameObject> _decoyTowerPrefabs;
    private HashSet<DecoyTower> _instantiatedDecoyTowers = new HashSet<DecoyTower>();
    // The decoy tower following the user's cursor, if requested.
    private DecoyTower _currentDecoyTower;

    public static event Action<bool> onTowerPlacementModeStatusChange;
    private bool _towerPlacementModeActivated;

    private void Awake()
    {
        if (_decoyTowerContainer == null)
        {
            Debug.Log("Decoy tower container is null in Tower Manager.");
        }
        
        foreach (GameObject decoyTowerPrefab in _decoyTowerPrefabs)
        {
            GameObject spawnedDecoyTowerType =
                Instantiate(decoyTowerPrefab, _decoyTowerContainer.transform);
            spawnedDecoyTowerType.SetActive(false);
            DecoyTower spawnedDecoyTower = spawnedDecoyTowerType.GetComponent<DecoyTower>();
            _instantiatedDecoyTowers.Add(spawnedDecoyTower);
        }
    }

    private void FixedUpdate()
    {
        // TODO: Switch this to be done by decoy tower itself or will that not work?
        if (_currentDecoyTower)
        {
            Ray rayOrigin = PlayerCamera.Instance.GetPlayerCamera()
                .ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHitInfo;
            if (Physics.Raycast(rayOrigin, out rayHitInfo))
            {
                _currentDecoyTower.transform.position = rayHitInfo.point;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ResetCurrentDecoyTower();
            _towerPlacementModeActivated = false;
            onTowerPlacementModeStatusChange.Invoke(false);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            _towerPlacementModeActivated = true;
            onTowerPlacementModeStatusChange.Invoke(true);
            SetCurrentDecoyTower(1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _towerPlacementModeActivated = true;
            onTowerPlacementModeStatusChange.Invoke(true);
            SetCurrentDecoyTower(2);
        }
    }

    private void SetCurrentDecoyTower(int towerType)
    {
        if (_currentDecoyTower && _currentDecoyTower.TowerType != towerType)
        {
            ResetCurrentDecoyTower();
        }
        DecoyTower decoyTower = _instantiatedDecoyTowers.First(
            dt => dt.TowerType == towerType);
        _currentDecoyTower = decoyTower;
        decoyTower.gameObject.SetActive(true);
    }

    private void ResetCurrentDecoyTower()
    {
        if (_currentDecoyTower)
        {
            _currentDecoyTower.gameObject.SetActive(false);
            _currentDecoyTower = null;
        }
    }
    
}
