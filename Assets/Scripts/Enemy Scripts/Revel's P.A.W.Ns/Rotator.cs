using UnityEngine;

/// <summary>
/// Rotator makes an object spin for a specified number of 360-degree rotations at a given speed.
/// Attach this script to any GameObject you want to rotate.
/// </summary>
public class Rotator : MonoBehaviour
{
    [Tooltip("Degrees per second for rotation.")]
    public float rotationSpeed = 180f;

    [Tooltip("Number of full 360-degree rotations to perform. Set to 0 for infinite.")]
    public int numberOfRotations = 1;

    private float rotatedDegrees = 0f;
    private bool isRotating = true;

    void Update()
    {
        if (!isRotating) return;

        float deltaRotation = rotationSpeed * Time.deltaTime;
        float targetDegrees = numberOfRotations * 360f;

        // If numberOfRotations is 0, spin infinitely
        if (numberOfRotations == 0)
        {
            transform.Rotate(Vector3.forward, deltaRotation);
            return;
        }

        // Clamp rotation to not exceed the target
        if (rotatedDegrees + deltaRotation >= targetDegrees)
        {
            deltaRotation = targetDegrees - rotatedDegrees;
            isRotating = false;
        }

        transform.Rotate(Vector3.forward, deltaRotation);
        rotatedDegrees += deltaRotation;
    }

    /// <summary>
    /// Call this to reset and start the rotation again.
    /// </summary>
    public void RestartRotation()
    {
        rotatedDegrees = 0f;
        isRotating = true;
    }

    public void ResetRotation()
    {
        this.transform.rotation = Quaternion.identity;
    }
}
