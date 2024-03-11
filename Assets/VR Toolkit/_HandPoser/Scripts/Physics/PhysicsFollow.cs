using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsFollow : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    [SerializeField] private Transform m_TransformToFollow;

    [Range(0, 1)] public float slowDownVelocity = 0.75f;
    [Range(0, 1)] public float slowDownAngularVelocity = 0.75f;

    [Range(0, 100)] public float maxPositionChange = 75.0f;
    [Range(0, 100)] public float maxRotationChange = 75.0f;

    private Vector3 targetPosition = Vector3.zero;
    private Quaternion targetRotation = Quaternion.identity;
    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        //m_HandInteractor = GetComponentInParent<HandInteractor>();
    }

    private void Update()
    {
        targetPosition = m_TransformToFollow.position;
        targetRotation = m_TransformToFollow.rotation;
    }
    private void FixedUpdate()
    {
        MoveUsingPhysics();
        RotateUsingPhysics();
    }

    private void MoveUsingPhysics()
    {
        // Prevents overshooting
        m_Rigidbody.velocity *= slowDownVelocity;

        // Get target velocity
        Vector3 velocity = FindNewVelocity();

        // Check if it's valid
        if (IsValidVelocity(velocity.x))
        {
            // Figure out the max we can move, then move via velocity
            float maxChange = maxPositionChange * Time.fixedDeltaTime;
            m_Rigidbody.velocity = Vector3.MoveTowards(m_Rigidbody.velocity, velocity, maxChange);
        }


    }

    private Vector3 FindNewVelocity()
    {
        // Figure out the difference we can move this frame
        Vector3 worldPosition = transform.root.TransformPoint(targetPosition);
        Vector3 difference = worldPosition - m_Rigidbody.position;
        return difference / Time.deltaTime;
    }

    private void RotateUsingPhysics()
    {
        // Prevents overshooting
        m_Rigidbody.angularVelocity *= slowDownAngularVelocity;

        // Get target velocity
        Vector3 angularVelocity = FindNewAngularVelocity();

        // Check if it's valid
        if (IsValidVelocity(angularVelocity.x))
        {
            // Figure out the max we can rotate, then move via velocity
            float maxChange = maxRotationChange * Time.fixedDeltaTime;
            m_Rigidbody.angularVelocity = Vector3.MoveTowards(m_Rigidbody.angularVelocity, angularVelocity, maxChange);
        }
    }

    private Vector3 FindNewAngularVelocity()
    {
        // Figure out the difference in rotation
        Quaternion worldRotation = transform.root.rotation * targetRotation;
        Quaternion difference = worldRotation * Quaternion.Inverse(m_Rigidbody.rotation);
        difference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);

        // Do the weird thing to account for have a range of -180 to 180
        if (angleInDegrees > 180)
            angleInDegrees -= 360;

        // Figure out the difference we can move this frame
        return (rotationAxis * angleInDegrees * Mathf.Deg2Rad) / Time.deltaTime;
    }

    private bool IsValidVelocity(float value)
    {
        // Is it an actual number, or is a broken number?
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
