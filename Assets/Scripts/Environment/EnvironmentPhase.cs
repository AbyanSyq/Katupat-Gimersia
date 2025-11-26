using UnityEngine;

public class EnvironmentPhase : MonoBehaviour
{
    [SerializeField] int currPhase;

    void Start()
    {
        currPhase = 1;
    }

    void OnEnable()
    {
        // Events.OnPhaseChange.Add();
    }
    void OnDisable()
    {
        // Events.OnPhaseChange.Remove();
    }
    
    void OnPhaseChange(int phase)
    {
        
    }

    [ContextMenu("Trigger Next Environment Phase")]
    void GoToNextEnvironmentPhase()
    {
        OnPhaseChange(currPhase++);
    }
}
