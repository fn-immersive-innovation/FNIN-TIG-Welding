using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class HeadWearer : MonoBehaviour
{
    [SerializeField] private string m_HeadTiggerTag = "";
    [SerializeField] private Material m_ObjectMaterial;

    public UnityEvent onHelmetWear;

    private void WearObject(Collider col)
    {
        var meshes = col.gameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (var mesh in meshes)
        {
            mesh.material = m_ObjectMaterial;
        }

        var grabbable = col.GetComponentInParent<XRGrabInteractable>();

        grabbable.transform.SetParent(this.transform);
        grabbable.transform.localPosition = Vector3.zero;
        grabbable.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(m_HeadTiggerTag))
        {
            return;
        }

        WearObject(other);

        onHelmetWear.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag(m_HeadTiggerTag))
        {
            return;
        }

        
    }
}
