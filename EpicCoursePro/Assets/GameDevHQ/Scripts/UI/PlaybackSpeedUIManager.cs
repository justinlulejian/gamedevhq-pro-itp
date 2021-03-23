using System;
using System.Collections;
using System.Collections.Generic;
using GameDevHQ.Scripts.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlaybackSpeedUIManager : SpriteColorableUIManager
{
    [SerializeField] 
    private GameObject _pauseGameObject;
    private Button _pauseButton;
    private Image _pauseClickedImage;
    [SerializeField] 
    private GameObject _playGameObject;
    private Button _playButton;
    private Image _playClickedImage;
    [SerializeField] 
    private GameObject _fastForwardGameObject;
    private Button _fastForwardButton;
    private Image _fastForwardImage;


    protected override void Awake()
    {
        base.Awake();
        _pauseButton = _pauseGameObject.GetComponentInChildren<Button>();
        _pauseClickedImage = _pauseGameObject.GetComponent<Image>();
        _playButton = _playGameObject.GetComponentInChildren<Button>();
        _playClickedImage = _playGameObject.GetComponent<Image>();
        _fastForwardButton = _fastForwardGameObject.GetComponentInChildren<Button>();
        _fastForwardImage = _fastForwardGameObject.GetComponent<Image>();
        List<MonoBehaviour> uiScripts = new List<MonoBehaviour>()
        {
            _pauseButton, _pauseClickedImage, _playButton, _playClickedImage, _fastForwardButton,
            _fastForwardImage
        };

        foreach (var uiScript in uiScripts)
        {
            if (uiScript == null)
            {
                Debug.LogError($"Playback speed UI manager does not have access to all " +
                               $"it's behavior scripts.");
            }
        }
    }
    
    public void PauseClicked()
    {
        _fastForwardImage.enabled = false;
        _playClickedImage.enabled = false;
        _pauseClickedImage.enabled = true;
        PlayerUIManager.Instance.PauseClicked();
    }

    public void PlayClicked()
    {
        _pauseClickedImage.enabled = false;
        _fastForwardImage.enabled = false;
        _playClickedImage.enabled = true;
        PlayerUIManager.Instance.PlayClicked();
    }

    public void FastForwardClick()
    {
        _pauseClickedImage.enabled = false;
        _playClickedImage.enabled = false;
        _fastForwardImage.enabled = true;
        PlayerUIManager.Instance.FastForwardClicked();
    }

    public void ResetClicked()
    {
        _pauseClickedImage.enabled = false;
        _playClickedImage.enabled = false;
        _fastForwardImage.enabled = false;
    }

}
