﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStatusUIManager : MonoBehaviour
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

    private void Awake()
    {
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

    private void PresentIntroInstructions()
    {
        _statusText.text = "Welcome to tower defense! Hit the start button to start the game. A " +
                           "countdown will appear to give you time to place the towers and then " +
                           "the waves of enemies will start.";
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