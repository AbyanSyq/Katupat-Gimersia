using UnityEngine;

public class EndAreaBoxTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            UIManager.Instance.ChangeUI(UIType.WIN);
    }
}
