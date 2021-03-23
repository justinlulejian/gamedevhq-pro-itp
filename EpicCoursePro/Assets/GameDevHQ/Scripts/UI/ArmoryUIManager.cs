using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevHQ.Scripts.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ArmoryUIManager : SpriteColorableUIManager
{
    private List<Button> _armoryButtons = new List<Button>();

    protected override void Awake()
    {
        base.Awake();
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
