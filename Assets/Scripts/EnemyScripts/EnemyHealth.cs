using Unity.Behavior;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private BehaviorGraphAgent enemyBehaviorGraphAgent;
    [SerializeField] private BlackboardVariable<OnEnemyHurt> OnEnemyHurtEventChannel;
    [SerializeField] private BlackboardVariable<bool> IsGetHurt;
    [Header("Enemy Health Settings")]
    [SerializeField] private int hitCount;
    [SerializeField] private int hitCombo;

    protected override void Awake()
    {
        base.Awake();
        // enemyBehaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
        enemyBehaviorGraphAgent.BlackboardReference.GetVariable<OnEnemyHurt>("OnEnemyHurt", out OnEnemyHurtEventChannel);
        enemyBehaviorGraphAgent.BlackboardReference.GetVariable<bool>("IsGetHurt", out IsGetHurt);
    }

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
    [ContextMenu("Test Hurt Event")]
    private void TestHurtEvent()
    {
        Debug.Log("Test Enemy Hurt Event Channel Send Event Message");
        OnEnemyHurtEventChannel.Value.SendEventMessage();
    }
    [ContextMenu("Test Get Hurt Event")]
    private void TestGetHurtEvent()
    {
        Debug.Log("Test Enemy Get Hurt Set IsGetHurt to True");
        IsGetHurt.Value = true;
    }

}
