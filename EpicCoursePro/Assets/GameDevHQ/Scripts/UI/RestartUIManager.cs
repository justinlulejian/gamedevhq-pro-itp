using GameDevHQ.Scripts.UI;
using UnityEngine;

public class RestartUIManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject _restartPressedGameObject;

    private void Awake()
    {
        if (_restartPressedGameObject == null)
        {
            Debug.LogError("Restart UI does not have access to clicked object.");
        }
    }

    public void OnClickRestart()
    {
        _restartPressedGameObject.SetActive(true);
        PlayerUIManager.Instance.RestartClicked();
    }

    public void ResetClickedRestart()
    {
        _restartPressedGameObject.SetActive(false);
    }

}
