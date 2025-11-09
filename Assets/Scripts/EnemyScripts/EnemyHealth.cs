using UnityEngine;

public class EnemyHealth : Health
{
    [Header("Enemy Health Settings")]
    [SerializeField] private int hitCount;
    [SerializeField] private int hitCombo;
    [SerializeField] private 


    void OnEnable()
    {

    }

    void OnDisable()
    {
        
    }
    public override void ReduceHealth(float amount)
    {
        base.ReduceHealth(amount + (amount * hitCombo));
    }
}
