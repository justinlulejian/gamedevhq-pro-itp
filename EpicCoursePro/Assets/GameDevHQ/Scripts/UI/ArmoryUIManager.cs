using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ArmoryUIManager : MonoBehaviour
{
    private List<Button> _armoryButtons = new List<Button>();

    private void Awake()
    {
        _armoryButtons = GetComponentsInChildren<Button>().ToList();

        if (_armoryButtons.Count == 0)
        {
            Debug.LogError($"Armory UI is missing it's buttons components.");
        }
    }

    public void EnableDisableArmoryButtons(bool enable)
    {
        _armoryButtons.ForEach(b => b.enabled = enable);
    }

    
}
