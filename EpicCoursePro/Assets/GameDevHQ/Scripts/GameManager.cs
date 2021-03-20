using System;
using GameDevHQ.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameDevHQ.Scripts
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField]
        private int _playerWarFunds = 500;
        [SerializeField] 
        private int _playerMaximumWarFunds = 100000;
        #if UNITY_EDITOR
            public float CurrentTimeScale;
        #endif
    
        public static event Action<int> onWarFundsChange;
        
        private void OnEnable()
        {
            Enemy.onEnemyKilledByPlayer += AddWarFundsForEnemy;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            Enemy.onEnemyKilledByPlayer -= AddWarFundsForEnemy;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Update()
        {
            // Debug timescale slow down function.
            #if UNITY_EDITOR
                if (Input.GetKey(KeyCode.H))
                {
                    Time.timeScale = Mathf.Clamp(Time.timeScale - .01f, 0f, 1.0f);
                } else if (Input.GetKey(KeyCode.J))
                {
                    Time.timeScale += .1f;
                } else if (Input.GetKey(KeyCode.K))
                {
                    Time.timeScale = 1.0f;
                }
                CurrentTimeScale = Time.timeScale;
            #endif
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals("Start_Level") && scene.isLoaded)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
           PlayerUIManager.Instance.PresentStartUI();
        }

        public void StartCountdownFinished()
        {
            StartWaves();
        }

        private void StartWaves()
        {
            SpawnManager.Instance.StartWaves();
        }

        public int GetWarFunds()
        {
            return _playerWarFunds;
        }

        private void SubtractWarFunds(int funds)
        {
            _playerWarFunds = Mathf.Clamp(_playerWarFunds - funds, 0, _playerMaximumWarFunds);
            onWarFundsChange?.Invoke(_playerWarFunds);
        }

        public void AddWarFunds(int funds)
        {
            _playerWarFunds += funds;
            onWarFundsChange?.Invoke(_playerWarFunds);
        }

        public bool PlayerCanPurchaseItem(int purchaseCost)
        {
            return _playerWarFunds >= purchaseCost;
        }

        public void PurchaseItem(int purchaseCost)
        {
            SubtractWarFunds(purchaseCost); 
        }

        private void AddWarFundsForEnemy(Enemy enemy)
        {
            AddWarFunds(enemy.WarFundValue);
        }
    }
}
