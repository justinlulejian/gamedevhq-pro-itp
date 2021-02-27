using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private int _playerWarFunds = 500;

    private void SubtractWarFunds(int funds)
    {
        _playerWarFunds = Mathf.Clamp(_playerWarFunds - funds, 0, _playerWarFunds * 9999);
    }

    public void AddWarFunds(int funds)
    {
        _playerWarFunds += funds;
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
