using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour
{
    public event EventHandler <KeyCode> OnKeyPressed;

    [SerializeField] public KeyCode keyCode;

    public bool stateSettable;
    private TextMeshProUGUI text;
    private Image fill;
    private Outline outline;

    private void Awake()
    {
        stateSettable = true;
        text = GetComponentInChildren<TextMeshProUGUI>();
        fill = GetComponent<Image>();
        outline = GetComponent<Outline>();
    }
    
    public void OnButtonClick()
    {
        OnKeyPressed?.Invoke(this, keyCode);
    }


    public void SetState(Tile.State state)
    {
        if (stateSettable) 
        {
            fill.color = state.fillColor;
            outline.effectColor = state.outlineColor;
        }
        stateSettable = false;

    }
}
