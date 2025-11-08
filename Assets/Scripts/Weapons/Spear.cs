using UnityEngine;

public class Spear : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
         rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Only rotate if moving
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            // Align spear's UP axis (local up) with its velocity direction
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, rb.linearVelocity.normalized);
            rb.MoveRotation(targetRotation);
        }
    }
}
