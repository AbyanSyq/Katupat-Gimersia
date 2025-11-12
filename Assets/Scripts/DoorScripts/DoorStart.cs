using DG.Tweening;
using UnityEngine;

public class DoorStart : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private LayerMask playerLayer;

    private bool isOpened = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            GameManager.Instance.StartGame();

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            GameManager.Instance.StartGame();

        }
    }
}
