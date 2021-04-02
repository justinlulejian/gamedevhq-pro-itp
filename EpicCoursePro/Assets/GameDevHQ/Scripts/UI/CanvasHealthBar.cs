using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevHQ.Scripts.UI
{
   public class CanvasHealthBar : MonoBehaviour
   {
      [SerializeField] private GameObject _gameObjectFollowing;
      [SerializeField] private Camera _playerCamera;
      [SerializeField] private Image barImage;
      // TODO: This doesn't seem to actually raise them off the head of the enemy...
      [SerializeField] private Vector3 overheadOffset = new Vector3(0, 1f, 0);

      private void OnDisable()
      {
         barImage.fillAmount = 1.0f;
      }

      private void Awake()
      {
         barImage = GetComponent<Image>();
      }

      private void Start()
      {
         _playerCamera = PlayerCamera.Instance.GetPlayerCamera();
      }

      private void Update()
      {
         transform.position = _playerCamera.WorldToScreenPoint(
            _gameObjectFollowing.transform.position + overheadOffset);
      }

      public void SetFollowing(GameObject followee)
      {
         _gameObjectFollowing = followee;
         transform.position = _gameObjectFollowing.transform.position;
      }

      public void UpdateHealthBar(int health, int maxHealth)
      {
         barImage.fillAmount = health / (float)maxHealth;
      }
   }
}
