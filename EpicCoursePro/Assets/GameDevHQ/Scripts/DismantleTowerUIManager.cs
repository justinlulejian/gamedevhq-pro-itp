using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevHQ.Scripts
{
    // TODO: refactor this to have a shared AbstractUIManager class with UpgradeGunUIManager.
    public class DismantleTowerUIManager : MonoSingleton<DismantleTowerUIManager>
    {
        [SerializeField] 
        private Button _confirmButton;
        [SerializeField] 
        private Button _cancelButton;
        [SerializeField]
        private Text _dismantleValueText;
        
        private List<Text> _texts = new List<Text>();
        private List<Button> _buttons = new List<Button>();
        private List<Image> _images = new List<Image>();
        private TowerSpot _towerSpotUpgrade;
        private Image _image;

        public static event Action<GameObject> onDismantleTower;
        
        private void Awake()
            {
                _image = GetComponent<Image>();
                _texts = GetComponentsInChildren<Text>().ToList();
                _buttons.Add(_confirmButton);
                _buttons.Add(_cancelButton);
                _images.Add(_image);
                _images.AddRange(GetComponentsInChildren<Image>());
        
                if (_image == null)
                {
                    Debug.LogError($"Dismantle UI doesn't have access to Image component.");
                }
                if (_texts.Count < 1)
                {
                    Debug.LogError($"Dismantle UI doesn't have access to text child components.");
                }
                if (_buttons.Count < 1)
                {
                    Debug.LogError($"Dismantle UI doesn't have access to button child components.");
                }
                if (_images.Count < 2)
                {
                    Debug.LogError($"Dismantle UI doesn't have access to all image/button child" +
                                   $" components.");
                }
                if (_confirmButton == null)
                {
                    Debug.LogError($"Dismantle UI doesn't have access to confirm button.");
                }
                if (_cancelButton == null)
                {
                    Debug.LogError($"Dismantle UI doesn't have access to cancel button.");
                }
            }
    
        private void EnableDisableUI(bool onoff)
        {
            foreach (var text in _texts)
            {
                text.enabled = onoff;
            }
            foreach (var button in _buttons)
            {
                button.enabled = onoff;
            }
            foreach (var image in _images)
            {
                image.enabled = onoff;
            }
        }

        private void TurnOnUI()
        {
            EnableDisableUI(true);
        }

        public void TurnOffUI()
        {
            EnableDisableUI(false);
            _towerSpotUpgrade = null;
        }
        
        public void PresentDismantleUI(TowerSpot towerSpot)
        {
            Debug.Log("Presenting dismantling UI.");
            _dismantleValueText.text = towerSpot.GetTowerPlacedOnSpot().WarFundValue.ToString();
            _towerSpotUpgrade = towerSpot;
            TurnOnUI();
        }

        public void DismantleTower()
        {
            Debug.Log("Dismantling tower.");
            Tower tower = _towerSpotUpgrade.GetTowerPlacedOnSpot();
            // TODO: create a new property that stores the dismantle value.
            GameManager.Instance.AddWarFunds(tower.WarFundValue);
            
            // TODO: once upgrade towers are pooled switch to pooling.
            Destroy(tower.gameObject);
            // PoolManager.Instance.RecyclePooledObj(tower.gameObject);
        }
    }
}
