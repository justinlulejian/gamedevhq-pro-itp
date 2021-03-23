using UnityEngine;
using UnityEngine.UI;

public class CanvasHealthBar : MonoBehaviour
{
   [SerializeField] private GameObject _gameObjectFollowing;
   [SerializeField] private Camera _mainCamera;
   [SerializeField] private Image barImage;
   [SerializeField] private Vector3 overheadOffset = new Vector3(0, 1f, 0);

   private void OnDisable()
   {
      barImage.fillAmount = 1.0f;
   }

   private void Awake()
   {
      _mainCamera = Camera.main;
      barImage = GetComponent<Image>();
   }

   private void Update()
   {
      transform.position = _mainCamera.WorldToScreenPoint(
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
