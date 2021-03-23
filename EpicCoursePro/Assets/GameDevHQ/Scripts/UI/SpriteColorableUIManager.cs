using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevHQ.Scripts.UI
{
    public class SpriteColorableUIManager : MonoBehaviour
    {
        [SerializeField]
        private Image _spriteToColor;

        protected virtual void Awake()
        {
            if (_spriteToColor == null)
            {
                Debug.LogError($"UI Manager {name} does not have access to colorable " +
                               $"sprite.");
            }
        }

        public void SetSpriteColor(Color color)
        {
            _spriteToColor.color = color;
        }
    }
}