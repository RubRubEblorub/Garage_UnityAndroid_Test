using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchJoystick : MonoBehaviour
{
    [SerializeField] public RectTransform knob;
    
    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}
