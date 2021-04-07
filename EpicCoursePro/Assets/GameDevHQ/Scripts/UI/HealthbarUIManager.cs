using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevHQ.Scripts.UI;
using UnityEngine;

// TODO: Merge with PlayerUIManager to be a single UI Manager class?
public class HealthbarUIManager : MonoSingleton<HealthbarUIManager>
{
    [SerializeField] 
    private List<CanvasHealthBar> _healthbarContainer;
    [SerializeField] 
    private GameObject _healthbarPrefab;
    [SerializeField] 
    private int _numberOfHealthbarsToPool = 30;

    // protected override void Awake()
    // {
    //     base.Awake();
    //     GeneratePooledHealthbar(_numberOfHealthbarsToPool);
    // }
    
    private void GeneratePooledHealthbar(int numBars)
    {
        for (int i = 0; i < numBars; i++)
        {
            GameObject spawnedBar = Instantiate(_healthbarPrefab, transform, false);
            spawnedBar.SetActive(false);
            _healthbarContainer.Add(spawnedBar.GetComponent<CanvasHealthBar>());
        }
    }
    
    public CanvasHealthBar RequestHealthbar(GameObject parent)
    {
        CanvasHealthBar healthBar = _healthbarContainer.FirstOrDefault(
            hb => hb.gameObject.activeSelf == false);

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.SetFollowing(parent);
            return healthBar;
        }
        GeneratePooledHealthbar(1);
        return RequestHealthbar(parent);
    }
    
}
