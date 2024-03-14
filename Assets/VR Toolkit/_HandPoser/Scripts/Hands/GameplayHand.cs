using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using System.Collections.Generic;

public class GameplayHand : BaseHand
{

    [Header("Hand Pose")]
    [SerializeField] private Pose gripPose = null;
    [SerializeField] private Pose triggerPose = null;
    [SerializeField] private Pose triggerGripPose = null;

    public bool hasGrabbed = false;
    // The interactor we react to
    [SerializeField] private ActionBasedController baseInteractor = null;
    [SerializeField] private HandInteractor playerHand = null;

    private Vector3 attachPosition = Vector3.zero;
    private Quaternion attachRotation;

    private void Update()
    {
        if (!baseInteractor || !playerHand) return;
        UpdateHandAnimation();
    }

    private void UpdateHandAnimation()
    {
        if (!playerHand.HandIsGrabbing)
        {
            var triggerValue = baseInteractor.activateActionValue.action.ReadValue<float>();
            var gripValue = baseInteractor.selectActionValue.action.ReadValue<float>();

            if (gripValue > 0.05f && triggerValue <= 0.01f)
            {
                ApplyLerpingPose(DefaultHandPose(), gripPose, gripValue);
            }
            else if (gripValue <= 0.01f && triggerValue > 0.05f)
            {
                ApplyLerpingPose(DefaultHandPose(), triggerPose, triggerValue);
            }
            else if (gripValue > 0.05f && triggerValue > 0.05f)
            {
                ApplyLerpingPose(DefaultHandPose(), triggerGripPose, gripValue);
            }
        }        
    }

    public void TryApplyObjectPose(PoseContainer poseContainer, Transform parent)
    {
        this.transform.SetParent(parent);

        ApplyPoseWithPos(poseContainer.pose, out attachPosition, out attachRotation);

        SetLocalPosition();
    }
    public void SetLocalPosition()
    {
        transform.localPosition = attachPosition;
        transform.localRotation = attachRotation;
    }

    public override void ApplyOffset(Vector3 position, Quaternion rotation)
    {
        

    }

    private void OnValidate()
    {
        // Let's have this done automatically, but not hide the requirement
        if (!baseInteractor)
        {
            baseInteractor = GetComponentInParent<ActionBasedController>();
        }

        if (!playerHand)
        {
            playerHand = GetComponentInParent<HandInteractor>();
        }
    }

    
}