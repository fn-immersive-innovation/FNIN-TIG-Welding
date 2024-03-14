using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandInteractor : MonoBehaviour
{
    [Header("Define Display and Interactable Hand Meshes")]
    [SerializeField] private GameplayHand grabbingHand = null;
    [SerializeField] private GameplayHand grabbingHandPhysics = null;
    [SerializeField] private GameplayHandStick displayHandAfterGrab = null;

    public enum InteractionType {
        Instantaneous,
        Physics
    }

    [Header("Interaction Type")]
    [SerializeField] private InteractionType m_InteractionType = InteractionType.Instantaneous;


    public GameplayHand getGrabbingHand
    {
        get {

            if (m_InteractionType == InteractionType.Instantaneous)
            {
                return grabbingHand;
            }
            else
            {
                return grabbingHandPhysics;
            }
            
        }
    }


    [SerializeField] private XRDirectInteractor interactor = null;
    private Vector3 handPosition;
    private Quaternion handRotation;

    [SerializeField] private SkinnedMeshRenderer displayHandMesh;
    [SerializeField] private SkinnedMeshRenderer displayHand2Mesh;

    private XRBaseInteractable m_XRBaseInteractable = null;

    private bool hasGrabbed = false;
    public bool HandIsGrabbing { get { return hasGrabbed; } }

    private void InitGrabbingHand(InteractionType interaction)
    {
        GameplayHand hand;

        if (interaction == InteractionType.Instantaneous)
        {
            hand = grabbingHand;
            grabbingHandPhysics.gameObject.SetActive(false);
        }
        else
        {
            hand = grabbingHandPhysics;
            grabbingHand.gameObject.SetActive(false);
        }

        displayHandMesh = hand.GetComponentInChildren<SkinnedMeshRenderer>();
        handPosition = hand.transform.localPosition;
        handRotation = hand.transform.localRotation;
    }

    private void Start()
    {
        InitGrabbingHand(m_InteractionType);

        displayHand2Mesh = displayHandAfterGrab.GetComponentInChildren<SkinnedMeshRenderer>();
        displayHand2Mesh.enabled = false;

        interactor.selectEntered.AddListener(OnSelectEntered);
        interactor.selectExited.AddListener(OnSelectExited);
    }


    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        m_XRBaseInteractable = args.interactableObject.transform.GetComponent<XRBaseInteractable>();

        if (args.interactableObject.transform.TryGetComponent(out ObjectInteractable interactable))
        {
            Hide();
        }
        
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        m_XRBaseInteractable = null;
        Show();
    } 


    private void Hide()
    {
        SetHandStateToObject(true);
        hasGrabbed = true;
    }
    public void Show()
    {
        SetHandStateToObject(false);
        hasGrabbed = false;

    }

    IEnumerator DelayInvoke(float duration, UnityAction callback)
    {
        yield return new WaitForSeconds(duration);

        callback.Invoke();

    }
    private void SetHandStateToObject(bool state)
    {
        if (state)
        {
            displayHandAfterGrab.transform.SetParent(m_XRBaseInteractable.transform);

            displayHandMesh.enabled = false;
            displayHand2Mesh.enabled = true;
        }
        else
        {
            displayHandAfterGrab.transform.SetParent(this.transform);
            displayHandAfterGrab.transform.localPosition = handPosition;
            displayHandAfterGrab.transform.localRotation = handRotation;

            displayHandMesh.enabled = true;
            displayHand2Mesh.enabled = false;

            var rb = grabbingHandPhysics.GetComponent<Rigidbody>();
            var poser = grabbingHandPhysics.GetComponent<PhysicsPoser>();

            rb.isKinematic = true;
            poser.enabled = false;

            StartCoroutine(DelayInvoke(0.5f, () =>
            {
                grabbingHandPhysics.transform.position = this.transform.position;
                rb.isKinematic = false;
                poser.enabled = true;
            }));
            
            //var pos = grabbingHandPhysics.transform.localPosition;
            //pos.y += 0.2f;
            //grabbingHandPhysics.transform.localPosition = pos;
        }
    }

}
