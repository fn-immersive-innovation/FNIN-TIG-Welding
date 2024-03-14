using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRDirectInteractor))]
public class HandController : MonoBehaviour
{
    [SerializeField] private GameObject controllerObject = null;
    [SerializeField] private GameObject controllerObjectDisplayHand = null;
    private bool handFollowTransform = false;

    private XRDirectInteractor interactor = null;
    private Vector3 handPosition;
    private Quaternion handRotation;
    private GameplayHand displayHand;
    private GameplayHandStick displayHand2;

    private SkinnedMeshRenderer displayHandMesh;
    private SkinnedMeshRenderer displayHand2Mesh;

    private void Awake()
    {
        if (controllerObject.TryGetComponent(out GameplayHand gp))
        {
            displayHand = gp;
            displayHandMesh = controllerObject.GetComponentInChildren<SkinnedMeshRenderer>();
        }
        else Debug.LogError("Display Hand doesnt have GameplayHand.CS");

        if (controllerObjectDisplayHand.TryGetComponent(out GameplayHandStick gp2))
        {
            displayHand2 = gp2;
            displayHand2Mesh = controllerObjectDisplayHand.GetComponentInChildren<SkinnedMeshRenderer>();
        }
        else Debug.LogError("Display Hand doesnt have GameplayHandStick.CS");

        handPosition = controllerObject.transform.localPosition;
        handRotation = controllerObject.transform.localRotation;
        interactor = GetComponent<XRDirectInteractor>();

        displayHand2Mesh.enabled = false;
    }

    private void OnEnable()
    {
        interactor.onSelectEntered.AddListener(Hide);
        interactor.onSelectExited.AddListener(Show);
    }

    private void OnDisable()
    {
        interactor.onSelectEntered.RemoveListener(Hide);
        interactor.onSelectExited.RemoveListener(Show);
    }

    private void Hide(XRBaseInteractable interactable)
    {
        if (interactable.TryGetComponent(out ObjectInteractable interactor))
        {
            handFollowTransform = interactor.GetHandFollowState();
        }

        TrySetCollider(controllerObject, true);

        if (handFollowTransform)
        {
            SetHandStateToObject(true);
        }
    }

    public void SetHandStateToObjectType2(bool state,Transform _transform)
    {
        if (state)
        {
            controllerObjectDisplayHand.transform.SetParent(_transform);

            displayHandMesh.enabled = false;
            displayHand2Mesh.enabled = true;

            _transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

        }
        else
        {            
            controllerObjectDisplayHand.transform.SetParent(this.transform);
            controllerObjectDisplayHand.transform.localPosition = handPosition;
            controllerObjectDisplayHand.transform.localRotation = handRotation;

            displayHandMesh.enabled = true;
            displayHand2Mesh.enabled = false;
        }
    }
    private void SetHandStateToObject(bool state)
    {
        if (state)
        {
            /*interactor.attachTransform.SetParent(this.transform);
            controllerObject.transform.SetParent(interactor.selectTarget.transform);
            displayHand.SetLocalPosition();*/

            controllerObjectDisplayHand.transform.SetParent(interactor.selectTarget.transform);
            //displayHand2.SetLocalPosition();

            displayHandMesh.enabled = false;
            displayHand2Mesh.enabled = true;

            interactor.selectTarget.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else
        {
            /*controllerObject.transform.SetParent(this.transform);
            controllerObject.transform.localPosition = handPosition;
            controllerObject.transform.localRotation = handRotation;
            interactor.attachTransform.SetParent(controllerObject.transform);*/

            controllerObjectDisplayHand.transform.SetParent(this.transform);
            controllerObjectDisplayHand.transform.localPosition = handPosition;
            controllerObjectDisplayHand.transform.localRotation = handRotation;

            displayHandMesh.enabled = true;
            displayHand2Mesh.enabled = false;
            //interactor.attachTransform.SetParent(controllerObject.transform);
        }
    }
    private void TrySetCollider(GameObject gameObject, bool State)
    {
        if (gameObject.GetComponentsInChildren<Collider>().Length > 0 && gameObject != null)
        {
            foreach (Collider col in gameObject.GetComponentsInChildren<Collider>())
            {
                col.isTrigger = State;
            }
        }
        else
        {
            Debug.LogWarning("Controller Doesnt have any collider");
        }
    }
    private void Show(XRBaseInteractable interactable)
    {
        if (handFollowTransform)
        {
            SetHandStateToObject(false);
        }
        //if (handFollowTransform) SetHandStateToObject(false);

        TrySetCollider(controllerObject, false);
    }

    private void SetState()
    {
        if (handFollowTransform) SetHandStateToObject(false);

        TrySetCollider(controllerObject, false);
    }
}
