using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class PointerInformation : MonoBehaviour
{
    [SerializeField] private XRBaseInteractable xRBaseInteractable;

    [SerializeField] private InputActionManager m_InputActionManager;

    private bool canSelect = false;
    private bool invoked = false;

    public UnityEvent OnClick;

    private void OnValidate()
    {
        xRBaseInteractable = GetComponent<XRBaseInteractable>();

        m_InputActionManager = FindAnyObjectByType<InputActionManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        xRBaseInteractable.hoverEntered.AddListener(HoverEntered);
        xRBaseInteractable.hoverExited.AddListener(HoverExited);

        var buttonPressedLeft = m_InputActionManager.actionAssets[0].FindActionMap("XRI LeftHand Interaction").FindAction("UI Press");
        var buttonPressedRight= m_InputActionManager.actionAssets[0].FindActionMap("XRI RightHand Interaction").FindAction("UI Press");

        buttonPressedLeft.Enable();
        buttonPressedLeft.started += UIButtonPressed;

        buttonPressedRight.Enable();
        buttonPressedRight.started += UIButtonPressed;
    }

    private void UIButtonPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!canSelect) return;

        if (invoked) return;

        if (canSelect)
        {
            OnClick.Invoke();
            invoked = true;
        }
    }

    private void HoverEntered(HoverEnterEventArgs arg0)
    {
        canSelect = true;
    }

    private void HoverExited(HoverExitEventArgs arg0)
    {
        canSelect = false;
    }

    public void ReduceMeshScale(Transform t)
    {
        t.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
}
