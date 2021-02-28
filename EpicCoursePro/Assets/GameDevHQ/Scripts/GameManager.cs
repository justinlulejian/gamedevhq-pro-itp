using System;
using UnityEngine;

namespace GameDevHQ.Scripts
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField]
        private int _playerWarFunds = 500;
        [SerializeField] 
        private int _playerMaximumWarFunds = 100000;
    
        public static event Action<int> onWarFundsChange;
    
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
    }
}
