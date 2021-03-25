using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevHQ.Scripts.UI
{
    public class WarFundsUIManager : SpriteColorableUIManager
    {
        [SerializeField] 
        private Text _warFundsText;
        
        // Cached yields
        private readonly WaitForSeconds _textFlickerWait = new(.25f);

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
            yield return _textFlickerWait;
            text.enabled = true;
            yield return _textFlickerWait;
            text.enabled = false;
            yield return _textFlickerWait;
            text.enabled = true;
            yield return _textFlickerWait;
        }
    }
}
