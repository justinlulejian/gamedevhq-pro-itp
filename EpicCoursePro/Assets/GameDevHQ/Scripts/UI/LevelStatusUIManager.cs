using System;
using System.Collections;
using System.Collections.Generic;
using GameDevHQ.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class LevelStatusUIManager : SpriteColorableUIManager
{
    [SerializeField]
    private GameObject _statusGameObject;
    private Text _statusText;
    [SerializeField]
    private GameObject _countdownGameObject;
    private Text _countdownText;
    [SerializeField] 
    private GameObject _startAcknowledgeGameObject;
    private Button _startAcknowledgeButton;
    [SerializeField] 
    private int _startCountdownTime = 5;

    public static event Action onCountdownFinished;

    private void OnDisable()
    {
        _statusGameObject.SetActive(false);
        _countdownGameObject.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        _statusText = _statusGameObject.GetComponent<Text>();
        _countdownText = _countdownGameObject.GetComponent<Text>();
        _startAcknowledgeButton = _startAcknowledgeGameObject.GetComponent<Button>();

        if (_statusText == null)
        {
            Debug.LogError("Level status is missing it's status text reference.");
        }
        if (_countdownText == null)
        {
            Debug.LogError("Level status is missing it's countdown text reference.");
        }
        if (_startAcknowledgeButton == null)
        {
            Debug.LogError("Level status is missing it's start ack button reference.");
        }
    }

    public void PresentStartUI()
    {
        _statusGameObject.SetActive(true);
        PresentIntroInstructions();
        _startAcknowledgeGameObject.SetActive(true);
    }
    
    public void PresentGameOverUI()
    {
        _statusGameObject.SetActive(true);
        PresentGameOverInstructions();
    }
    
    public void PresentPlayerWonUI()
    {
        _statusGameObject.SetActive(true);
        PresentPlayerWonInstructions();
    }

    private void PresentIntroInstructions()
    {
        _statusText.text = "Welcome to tower defense! Hit the start button to start the game. A " +
                           "countdown will appear to give you time to place the towers and then " +
                           "the waves of enemies will start.";
    }
    
    private void PresentGameOverInstructions()
    {
        _statusText.text = "GAME OVER.\n" +
                           "Click the restart button to restart the level to try again.";
    }
    
    private void PresentPlayerWonInstructions()
    {
        _statusText.text = "You won! Good job!\n" +
                           "Click the restart button to restart the level to play again.";
    }

    public void StartCountdown()
    {
        _startAcknowledgeGameObject.SetActive(false);
        StartCoroutine(CountdownToStartGameUI());
    }

    private IEnumerator CountdownToStartGameUI()
    {
        _countdownGameObject.SetActive(true);
        for (int i = _startCountdownTime; i >= 0; i--)
        {
            _countdownText.text = $"COUNTDOWN TO WAVE START: {i}";
            yield return new WaitForSeconds(1.0f);
        }
        onCountdownFinished?.Invoke();
    }
}
