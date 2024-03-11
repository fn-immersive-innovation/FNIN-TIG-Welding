using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class TeleportController : MonoBehaviour
{
    [SerializeField] private InputActionAsset m_DefaultInputAsset = null;
    [SerializeField] private ActionBasedController m_Controller = null;

    [SerializeField] private TeleportationProvider m_TeleportationProvider = null;

    

    [SerializeField] private XRRayInteractor m_RayInteractor = null;

    public enum HandType
    {
        Left,
        Right
    }

    [SerializeField] private HandType m_TeleportHand = HandType.Left;

    private InputAction activate;
    private void OnValidate()
    {
        if (!m_RayInteractor) m_RayInteractor = GetComponent<XRRayInteractor>();
        if (!m_Controller) m_Controller = GetComponent<ActionBasedController>();

        if (!m_DefaultInputAsset)
        {
            m_DefaultInputAsset = FindObjectOfType<InputActionManager>().actionAssets[0];
        }

        if (!m_TeleportationProvider)
        {
            m_TeleportationProvider = FindObjectOfType<TeleportationProvider>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
        if (m_TeleportHand == HandType.Left) activate = m_DefaultInputAsset.FindActionMap("XRI LeftHand Locomotion").FindAction("Teleport Mode Activate");
        else activate = m_DefaultInputAsset.FindActionMap("XRI RightHand Locomotion").FindAction("Teleport Mode Activate");

        activate.Enable();
        activate.started += TeleportPressed;
        activate.canceled += TeleportReleased;

        m_RayInteractor.enabled = false;
    }

    private bool m_TeleportTriggered = false;
    private void TeleportPressed(InputAction.CallbackContext obj)
    {
        m_RayInteractor.enabled = true;

        m_Controller.SendHapticImpulse(0.25f, 0.1f);

    }

    private void TeleportReleased(InputAction.CallbackContext obj)
    {
        if (m_RayInteractor.isActiveAndEnabled)
        {
            m_RayInteractor.enabled = false;
            TryTeleport(m_RayInteractor);
            m_Controller.SendHapticImpulse(0.5f, 0.1f);
        }
        
    }

    private void TryTeleport(XRRayInteractor rayInteractor)
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            TeleportRequest request;

            if (hit.transform.TryGetComponent<TeleportationAnchor>(out TeleportationAnchor teleportationAnchor))
            {
                request = new TeleportRequest()
                {
                    destinationPosition = teleportationAnchor.teleportAnchorTransform.position,
                    destinationRotation = teleportationAnchor.teleportAnchorTransform.rotation,
                    matchOrientation = MatchOrientation.TargetUpAndForward
                };
            }
            else
            {
                request = new TeleportRequest()
                {
                    destinationPosition = hit.point
                };
            }

            m_TeleportationProvider.QueueTeleportRequest(request);

        }

    }

}
