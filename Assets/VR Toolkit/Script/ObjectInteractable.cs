using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System;

[RequireComponent(typeof (PoseContainer))]
[RequireComponent(typeof (AudioSource))]
public class ObjectInteractable : MonoBehaviour
{
    XRBaseInteractor m_XRInteractor;

    bool state = false;

    [SerializeField] private XRBaseInteractable m_XRGrabInteractable;
    [SerializeField] private Transform m_FollowTransform;

    [Header("Define Object Custom Functionalities")]

    [SerializeField] private bool allowOffHandGrab = true;
    [SerializeField] private bool stickToHand = false;
    [SerializeField] private bool handFollowTransform = false;
    [SerializeField] private bool useDefaultTrackingType = false;


    private void Start()
    {
        m_XRGrabInteractable.selectEntered.AddListener(OnSelectEntered);
        m_XRGrabInteractable.selectExited.AddListener(OnSelectExited);
    }


    private void OnValidate()
    {
        m_XRGrabInteractable = GetComponent<XRBaseInteractable>();
    }
    public bool GetHandFollowState()
    {
        return handFollowTransform;
    }

    public bool GetStickToHandState()
    {
        return stickToHand;
    }

    public void SetAllowOffHandGrab(bool value)
    {
        allowOffHandGrab = value;
    }
    public void SetStickToHandState(bool value)
    {
        stickToHand = value;
    }

    //How to use call; DropObject(m_XRInteractor);
    private void OnSelectEntered(SelectEnterEventArgs arg0)
    {
        //hasExited = false;

        if (handFollowTransform)
        {
            var displayHand = this.GetComponentInChildren<GameplayHandStick>();
            //var displayHand = arg0.interactorObject.transform.GetComponentInParent<HandInteractor>().getGrabbingHand;
            var poseContainer = GetComponent<PoseContainer>();

            if (displayHand && poseContainer)
            {
                if (poseContainer.pose)
                {
                    displayHand.TryApplyObjectPose(poseContainer, m_FollowTransform);
                }
            }
            

        }             
    }
 
    private void OnSelectExited(SelectExitEventArgs arg0)
    {
        if (!stickToHand)
        {
            //hasExited = true;
            
        }
    }

    public void SetParentToWorldV2()
    {

        transform.SetParent(null);
    }

    
}
