using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class ElectrodeDropper : MonoBehaviour
{
    [SerializeField] private InputActionManager m_InputActionManager;
    [SerializeField] private SnapZone m_SnapZone;

    public UnityEvent onElectrodeDropped;

    private void OnValidate()
    {
        m_InputActionManager = FindAnyObjectByType<InputActionManager>();
        m_SnapZone = GetComponent<SnapZone>();
    }

    private void Start()
    {
        var buttonPressedRight = m_InputActionManager.actionAssets[0].FindActionMap("XRI RightHand Interaction").FindAction("UI Press");

        buttonPressedRight.Enable();
        buttonPressedRight.started += ButtonAPressed;
    }

    private void ButtonAPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        var grabbable = m_SnapZone.getCurrentGrabable;

        if (!grabbable) return;

        var weldingCompt = grabbable.gameObject.GetComponent<WeldingMachine>();

        if (!weldingCompt) return;

        //if (!weldingCompt.getLimitReached) return;

        m_SnapZone.ForceDeSnap();
        onElectrodeDropped.Invoke();
    }

}
