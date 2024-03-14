using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using System.Collections.Generic;

public class GameplayHandStick : BaseHand
{
    // The interactor we react to
    [SerializeField] private XRBaseInteractor baseInteractor = null;

    private Vector3 attachPosition = Vector3.zero;
    private Quaternion attachRotation;
  

    public void TryApplyObjectPose(PoseContainer poseContainer, Transform parent)
    {
        this.transform.SetParent(parent);

        ApplyPoseWithPos(poseContainer.pose, out attachPosition, out attachRotation);

        SetLocalPosition();
    }

    private void SetLocalPosition()
    {
        transform.localPosition = attachPosition;
        transform.localRotation = attachRotation;
    }
    public void TryApplyDefaultPose()
    {
        ApplyDefaultPose();
    }

    public override void ApplyOffset(Vector3 position, Quaternion rotation)
    {
        // Invert since the we're moving the attach point instead of the hand
        /*Vector3 finalPosition = position * -1.0f;
        Quaternion finalRotation = Quaternion.Inverse(rotation);

        // Since it's a local position, we can just rotate around zero
        finalPosition = finalPosition.RotatePointAroundPivot(Vector3.zero, finalRotation.eulerAngles);

        // Set the position and rotach of attach
        baseInteractor.attachTransform.localPosition = finalPosition;
        baseInteractor.attachTransform.localRotation = finalRotation;*/

    }

    private void OnValidate()
    {
        // Let's have this done automatically, but not hide the requirement
        if (!baseInteractor)
        {
            baseInteractor = GetComponentInParent<XRBaseInteractor>();
        }
    }
}