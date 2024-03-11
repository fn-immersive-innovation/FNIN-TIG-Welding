using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WeldSticher : MonoBehaviour
{
    [SerializeField] private PowerManager powerManager;

    [SerializeField] private string m_WeldPieceTag = "Weld";

    private GameObject objectToJoinA;
    private GameObject objectToJoinB;

    private bool stiched = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!powerManager.canWeld || stiched)
            return;

        if (other.CompareTag(m_WeldPieceTag))
        {
            if (objectToJoinA == null)
            {
                objectToJoinA = other.gameObject;
            }
            else if (objectToJoinA != other.gameObject && objectToJoinB == null)
            {
                objectToJoinB = other.gameObject;

                //Here make object grabbable
                JoinObjects(objectToJoinA, objectToJoinB);
                //SetGrabbableState(objectToJoinA);
                //objectToJoinA.transform.SetParent(objectToJoinB.transform);

                stiched = true;
            }
      
        }
    }

    private void JoinObjects(GameObject obj1, GameObject obj2)
    {
        var grab1 = obj1.GetComponentInParent<XRGrabInteractable>();
        var grab2 = obj2.GetComponentInParent<XRGrabInteractable>();

        if (!grab1 || !grab2) return;

        var fixedJoint = grab2.gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = grab1.gameObject.GetComponent<Rigidbody>();

        grab2.enabled = false;

        grab2.transform.SetParent(grab1.transform);
    }

    private void SetGrabbableState(GameObject obj, bool state = false)
    {
        if (state)
        {
            
            obj.AddComponent<Rigidbody>();
            obj.AddComponent<XRGrabInteractable>();

            return;
        }

        var grabbable = obj.GetComponent<XRGrabInteractable>();

        if (!grabbable) return;

        var rb = grabbable.GetComponent<Rigidbody>();

        grabbable.enabled = false;

        Destroy(grabbable);
        Destroy(rb);
    }

    private void OnTriggerExit(Collider other)
    {
        if (stiched)
            return;

        if (other.CompareTag(m_WeldPieceTag))
        {
            if (objectToJoinA == other.gameObject)
            {
                objectToJoinA = null;
            }
            else if (objectToJoinB == other.gameObject)
            {
                objectToJoinB = null;
            }
        }
    }
}
