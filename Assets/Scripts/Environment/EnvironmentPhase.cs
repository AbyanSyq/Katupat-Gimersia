using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class EnvironmentPhase : MonoBehaviour
{
    [System.Serializable]
    public class ParticlesList // stores what particles will be triggered when each phase starts
    { 
        public List<ParticleSystem> particles; 
    }

    [Header("References")]
    [SerializeField] Animator anim;
    [SerializeField] CinemachineImpulseSource cinemachineImpulseSource;

    [Header("Phase Effects List")]
    [Tooltip("Each index element below is for each phase. In 1-based.")]
    [SerializeField] List<ParticlesList> phaseTransitionParticlesList;

    [Header("Stats/Debug")]
    [SerializeField] int currPhase;

    void Start()
    {
        currPhase = 1;
    }

    void OnEnable()
    {
        Events.OnPhaseChange.Add(OnPhaseChange);
    }
    void OnDisable()
    {
        Events.OnPhaseChange.Remove(OnPhaseChange);
    }
    
    void OnPhaseChange(int phase)
    {
        anim.SetInteger("phase", phase);
        TriggerPhaseTransitionEffects(--phase);
        cinemachineImpulseSource.GenerateImpulseWithForce(0.2f);
    }

    void TriggerPhaseTransitionEffects(int phase)
    {
        if(phaseTransitionParticlesList[phase] == null) return;

        foreach(ParticleSystem particle in phaseTransitionParticlesList[phase].particles)
        {
            particle.Play();
        }
    }

    [ContextMenu("Trigger Next Environment Phase")] // for testing only
    void GoToNextEnvironmentPhase()
    {
        currPhase++;
        OnPhaseChange(currPhase);
    }
}
