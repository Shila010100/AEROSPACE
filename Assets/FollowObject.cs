using UnityEngine;

// This script makes the attached GameObject follow the position and rotation of the specified target object.
public class FollowObject : MonoBehaviour
{
    [Tooltip("The target transform that this object should follow.")]
    public Transform target; // Assign this in the inspector to the part of the hand or any other object you want the cube to follow.

    void LateUpdate() // Using LateUpdate to ensure all positional updates have been processed.
    {
        if (target != null)
        {
            // Match the position and rotation of the target
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}
