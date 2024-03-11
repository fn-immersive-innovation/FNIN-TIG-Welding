using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class PhysicsPoser : MonoBehaviour
{
    public float movementSpeed = 0.1f;

    [Range(0, 1)] public float slowDownVelocity = 0.75f;
    [Range(0, 1)] public float slowDownAngularVelocity = 0.75f;

    [Range(0, 1000)] public float maxPositionChange = 75.0f;
    [Range(0, 1000)] public float maxRotationChange = 75.0f;

    // References
    [SerializeField] private Rigidbody rigidBody = null;

    [SerializeField] private ActionBasedController baseInteractor = null;
    [SerializeField] private HandInteractor playerHand = null;

    // Runtime
    private Vector3 targetPosition = Vector3.zero;
    private Quaternion targetRotation = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        // Update our target location
        targetPosition = baseInteractor.transform.position;
        targetRotation = baseInteractor.transform.rotation;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!baseInteractor || !playerHand) return;
        if (playerHand.HandIsGrabbing) return;
        if (!rigidBody) return;

        //rigidBody.position = Vector3.Lerp(rigidBody.position, targetPosition, movementSpeed * Time.fixedDeltaTime);
        //rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, movementSpeed * Time.fixedDeltaTime);

        //MoveUsingPhysicsv2();

        MoveUsingPhysics();
        RotateUsingPhysics();
    }

    private void MoveUsingPhysics()
    {
        // Prevents overshooting
        rigidBody.velocity *= slowDownVelocity;

        // Get target velocity
        Vector3 velocity = FindNewVelocityv2();

        // Check if it's valid
        if (IsValidVelocity(velocity.x))
        {
            // Figure out the max we can move, then move via velocity
            float maxChange = maxPositionChange * Time.deltaTime;
            rigidBody.velocity = Vector3.MoveTowards(rigidBody.velocity, velocity, maxChange);
        }

    }

    private Vector3 FindNewVelocity()
    {
        // Figure out the difference we can move this frame
        Vector3 worldPosition = transform.root.TransformPoint(targetPosition);
        Vector3 difference = worldPosition - rigidBody.position;
        return difference / Time.deltaTime;
    }

    private Vector3 FindNewVelocityv2()
    {

        // Calculate the difference vector between the current position and the target position
        Vector3 currentPosition = transform.position;
        Vector3 difference = targetPosition - currentPosition;

        // Calculate velocity by dividing the difference by a time interval (e.g., Time.deltaTime)
        float timeInterval = Time.deltaTime;
        if (timeInterval > 0)
        {
            return difference / timeInterval;
        }
        else
        {
            // To avoid division by zero, you might want to handle this case differently
            return Vector3.zero;
        }
    }

    private void RotateUsingPhysics()
    {
        // Prevents overshooting
        rigidBody.angularVelocity *= slowDownAngularVelocity;

        // Get target velocity
        Vector3 angularVelocity = FindNewAngularVelocity();

        // Check if it's valid
        if (IsValidVelocity(angularVelocity.x))
        {
            // Figure out the max we can rotate, then move via velocity
            float maxChange = maxRotationChange * Time.deltaTime;
            rigidBody.angularVelocity = Vector3.MoveTowards(rigidBody.angularVelocity, angularVelocity, maxChange);
        }
    }

    private Vector3 FindNewAngularVelocity()
    {
        // Figure out the difference in rotation
        Quaternion worldRotation = transform.root.rotation * targetRotation;
        Quaternion difference = worldRotation * Quaternion.Inverse(rigidBody.rotation);
        difference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);

        // Do the weird thing to account for have a range of -180 to 180
        if (angleInDegrees > 180)
            angleInDegrees -= 360;

        // Figure out the difference we can move this frame
        return (rotationAxis * angleInDegrees * Mathf.Deg2Rad) / Time.deltaTime;
    }

    private Vector3 FindNewAngularVelocityv2()
    {
        // Calculate the difference in rotation
        Quaternion currentRotation = transform.rotation;
        Quaternion relativeRotation = Quaternion.Inverse(currentRotation) * targetRotation;

        // Convert the relative rotation to an angle-axis representation
        relativeRotation.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);

        // Ensure that the angle is in the range -180 to 180 degrees
        if (angleInDegrees > 180)
        {
            angleInDegrees -= 360;
        }

        // Calculate the angular velocity based on the angle and time interval (e.g., Time.deltaTime)
        float angularVelocity = (angleInDegrees * Mathf.Deg2Rad) / Time.deltaTime;

        // Combine the angular velocity with the rotation axis
        Vector3 angularVelocityVector = rotationAxis * angularVelocity;

        return angularVelocityVector;
    }


    private bool IsValidVelocity(float value)
    {
        // Is it an actual number, or is a broken number?
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
