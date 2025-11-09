using UnityEngine;


public static partial class Events
{
    
}
public class GameplayManager : SingletonMonoBehaviour<GameplayManager>
{
    [Header("Player Log")]
    [SerializeField] private int playerAttackHittedCount;//when the spear hit the enemy
    [SerializeField] private int playerAttackHittedCombo;//when the spear hit the enemy
    [SerializeField] private int playerAttackMissedCount;//when the spear missed the enemy



    void OnEnable()
    {
        Events.OnPlayerAttackHitted.Add(OnPlayerAttackHitted);
        Events.OnPlayerAttackMissed.Add(OnPlayerAttackMissed);
    }
    
    void OnDisable()
    {
        Events.OnPlayerAttackHitted.Remove(OnPlayerAttackHitted);
        Events.OnPlayerAttackMissed.Remove(OnPlayerAttackMissed);
    }
    private void OnPlayerAttackHitted()
    {
        playerAttackHittedCount++;
        playerAttackHittedCombo++;
    }
    public void OnPlayerAttackMissed()
    {
        playerAttackMissedCount++;
        playerAttackHittedCombo = 0;
    }

}
