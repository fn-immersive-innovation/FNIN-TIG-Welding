using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SnapZone : MonoBehaviour
{
    [SerializeField] private bool m_UseGripToSnap = false;
    [SerializeField] private string m_SnapTiggerTag = "";

    [SerializeField] private Vector3 m_SnapPosition = Vector3.zero;
    [SerializeField] private Vector3 m_SnapRotation = Vector3.zero;

    [SerializeField] private bool m_DisableKinematicOnGrab = false;
    [SerializeField] private bool m_DestroyRigidbody = false;

    public UnityEvent onSnap;
    public UnityEvent onSnapExit;

    private bool hasInvoked = false;
    private bool isSelected = false;

    private XRGrabInteractable m_CurrentGrabbable;

    public XRGrabInteractable getCurrentGrabable
    {
        get { return m_CurrentGrabbable; }
    }

    [SerializeField]
    private XRDirectInteractor m_DirectInteractor;

    private void Start()
    {
        if (m_DirectInteractor)
        {
            m_DirectInteractor.selectEntered.AddListener(OnSnapSelect);
        }
    }

    private void OnSnapSelect(SelectEnterEventArgs arg0)
    {
        if (!arg0.interactableObject.transform.CompareTag(m_SnapTiggerTag))
        {
            return;
        }


        if (hasInvoked) return;

        //SnapObject(arg0.interactableObject.transform.GetComponentInChildren<Collider>());

        var grabbable = arg0.interactableObject.transform.GetComponent<XRGrabInteractable>();

        if (grabbable)
        {
            //grabbable.gameObject.SetActive(false);
            grabbable.enabled = false;
        }

        StartCoroutine(DelayInvoke());
        
    }

    IEnumerator DelayInvoke()
    {
        yield return new WaitForSeconds(0.2f);
        onSnap.Invoke();
        hasInvoked = true;

    }

    private void SnapObject(Collider col)
    {
        var m_Object = col.gameObject.transform;

        //Get Grabbable
        var grabbable = m_Object.GetComponentInParent<XRGrabInteractable>();

        if (!grabbable)
        {
            print("No Grabbable found in parent");
            return;
        }

        m_CurrentGrabbable = grabbable;
        grabbable.enabled = false;

        if(m_DisableKinematicOnGrab)
        {
            grabbable.GetComponent<Rigidbody>().isKinematic = true;
        }

        grabbable.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        grabbable.transform.SetParent(this.transform);

        grabbable.transform.localPosition = m_SnapPosition;
        grabbable.transform.localRotation = Quaternion.Euler(m_SnapRotation);

        var connectRigidbody = this.transform.GetComponentInParent<Rigidbody>();
        var parentGrabbable = this.transform.GetComponentInParent<XRGrabInteractable>();

        //Enable back
        grabbable.enabled = true;

        grabbable.selectEntered.AddListener(ObjectSelected);
        grabbable.selectExited.AddListener(ObjectDeSelected);


        if (connectRigidbody)
        {
            grabbable.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            var fixedJoint = grabbable.gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = connectRigidbody;

            grabbable.enabled = false;
        }

    }

    private void ObjectSelected(SelectEnterEventArgs arg0)
    {
        arg0.interactableObject.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        isSelected = true;
    }

    private void ObjectDeSelected(SelectExitEventArgs arg0)
    {
        isSelected = false;
    }

    private void DeSnapObject(Collider col)
    {
        var m_Object = col.gameObject.transform;

        //Get Grabbable
        var grabbable = m_Object.GetComponentInParent<XRGrabInteractable>();

        if (!grabbable)
        {
            print("No Grabbable found in parent");
            return;
        }


        grabbable.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        grabbable.transform.parent = null;

        var connectedJoint = grabbable.GetComponent<FixedJoint>();

        if (connectedJoint)
        {
            connectedJoint.connectedBody = null;
            Destroy(connectedJoint);
            //grabbable.enabled = true;
        }

        grabbable.selectEntered.RemoveListener(ObjectSelected);
        grabbable.selectExited.RemoveListener(ObjectDeSelected);

        StopAllCoroutines();

        StartCoroutine(DisableEnableSnapping(() => {
            //if (m_DisableKinematicOnGrab) grabbable.GetComponent<Rigidbody>().isKinematic = false;
        }));

        m_CurrentGrabbable = null;
        grabbable.transform.SetParent(null);
    }

    public void ForceDeSnap()
    {
        if (!m_CurrentGrabbable) return;

        var col = m_CurrentGrabbable.GetComponentInChildren<Collider>();

        if (!col) return;

        DeSnapObject(col);

        hasInvoked = false;
    }
    IEnumerator DisableEnableSnapping(UnityAction callback)
    {
        yield return new WaitForSeconds(1);
        isSelected = false;

        callback.Invoke();

    }

    private void OnTriggerEnter(Collider other)
    {

        if (!other.gameObject.CompareTag(m_SnapTiggerTag))
        {
            return;
        }

        if (m_UseGripToSnap) return;

        if (isSelected) return;
        if (hasInvoked) return;

        SnapObject(other);

        onSnap.Invoke();
        hasInvoked = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag(m_SnapTiggerTag))
        {
            return;
        }

        if (m_UseGripToSnap) return;

        if (!hasInvoked) return;

        if (!isSelected) return;

        DeSnapObject(other);

        onSnapExit.Invoke();

        hasInvoked = false;


    }
}
