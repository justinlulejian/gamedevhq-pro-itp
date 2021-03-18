using GameDevHQ.Scripts;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class ArmoryButton : MonoBehaviour
{
    [SerializeField] 
    private GameObject _towerPrefab;
    [SerializeField] 
    private Text _priceText;

    private Button _button;
    private AbstractTower _tower;
    
    private void OnEnable()
    {
        GameManager.onWarFundsChange += ChangeButtonStateIfInsufficientFunds;
    }

    private void OnDisable()
    {
        GameManager.onWarFundsChange -= ChangeButtonStateIfInsufficientFunds;
    }

    private void Awake()
    {
        if (_towerPrefab == null)
        {
            Debug.LogError($"Armory button {this.name} does not have a tower prefab.");
        }
        if (_priceText == null)
        {
            Debug.LogError($"Armory button {this.name} does not have a price text object.");
        }

        _button = GetComponent<Button>();
        if (_button == null)
        {
            Debug.LogError($"Armory button {this.name} does not have a Button component" +
                           $" object.");
        }
        
        _tower = _towerPrefab.GetComponent<AbstractTower>();

        if (_tower == null)
        {
            Debug.LogError($"Armory button {this.name} could not find tower from tower" +
                           $" prefab.");
        }

        _priceText.text = $"${_tower.TowerInfo.WarFundsValue.ToString()}";
    }

    private void ChangeButtonStateIfInsufficientFunds(int currentWarFunds)
    {
        if (_tower.TowerInfo.WarFundsValue > currentWarFunds)
        {
            _button.interactable = false;
        }
        else
        {
            _button.interactable = true;
        }
    }
}
