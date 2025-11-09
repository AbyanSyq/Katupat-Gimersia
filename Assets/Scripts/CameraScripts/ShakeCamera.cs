using Unity.Cinemachine;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;
    public float force = 1f;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    [ContextMenu("Shake")]
    public void Shake()
    {
        impulseSource.GenerateImpulse(force);
    }
}
