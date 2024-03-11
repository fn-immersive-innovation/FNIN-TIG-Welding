using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private ActionBasedController m_handController = null;

    public Animator GetAnimator
    {
        get { return animator; }
    }

    private void OnValidate()
    {
        if (!animator) animator = GetComponent<Animator>();

        if (!m_handController)
        {
            m_handController = GetComponentInParent<ActionBasedController>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_handController)
        {
            ReceiveGripValue(m_handController.activateActionValue.action.ReadValue<float>());
            ReceiveTriggerValue(m_handController.selectActionValue.action.ReadValue<float>());
        }
        
    }

    private void ReceiveGripValue(float v)
    {
        animator.SetFloat("Grip", v);
        //throw new NotImplementedException();
    }

    private void ReceiveTriggerValue(float v)
    {
        animator.SetFloat("Trigger", v);
        //throw new NotImplementedException();
    }
}
