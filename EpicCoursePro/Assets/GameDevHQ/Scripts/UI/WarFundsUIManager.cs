using System.Collections;
using GameDevHQ.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class WarFundsUIManager : MonoSingleton<WarFundsUIManager>
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

    public void WarFundsOutAnim()
    {
        StartCoroutine(FlashTextCoroutine(_warFundsText));

    }

    private IEnumerator FlashTextCoroutine(Text text)
    {
        text.enabled = false;
        yield return new WaitForSeconds(.25f);
        text.enabled = true;
        yield return new WaitForSeconds(.25f);
        text.enabled = false;
        yield return new WaitForSeconds(.25f);
        text.enabled = true;
        yield return new WaitForSeconds(.25f);
    }
}
