using GameDevHQ.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class WarFundsUIManager : MonoBehaviour
{
    [SerializeField] 
    private Text _warFundsText;

    private void Start()
    {
        SetWarFundsText(GameManager.Instance.GetWarFunds());
    }

    private void OnEnable()
    {
        GameManager.onWarFundsChange += SetWarFundsText;
    }

    private void OnDisable()
    {
        GameManager.onWarFundsChange -= SetWarFundsText;
    }

    private void SetWarFundsText(int currentWarFunds)
    {
        _warFundsText.text = $"{currentWarFunds.ToString()}";
    }
}
