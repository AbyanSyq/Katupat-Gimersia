using UnityEngine;
using UnityEngine.InputSystem;


public static partial class Events
{
    public static readonly GameEvent OnGameplayStarted = new GameEvent();
    public static readonly GameEvent<int> OnPlayerAtkHitCounted = new GameEvent<int>();
    public static readonly GameEvent<int> OnPlayerAtkHitComboCounted = new GameEvent<int>();
}
public class GameplayManager : SingletonMonoBehaviour<GameplayManager>
{
    [SerializeField, ReadOnly] private PlayerController3D playerController;
    [SerializeField, ReadOnly] private PlayerInputHandler playerInputHandler;
    public PlayerController3D PlayerController { get => playerController; }
    [Header("Player Log")]
    [SerializeField] private int playerAtkHitCount;//when the spear hit the enemy
    [SerializeField] private int playerAtkHitComboCount;//when the spear hit the enemy
    [SerializeField] private int playerAtkMissedCount;//when the spear missed the enemy

    [Header("Game State")]
    [SerializeField] private bool isGamePaused = false;
    protected override void Awake()
    {
        base.Awake();
        playerController = FindFirstObjectByType<PlayerController3D>();
        playerInputHandler = FindFirstObjectByType<PlayerInputHandler>();
    }

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
        playerAtkHitCount++;
        playerAtkHitComboCount++;
        
        Events.OnPlayerAtkHitCounted.Publish(playerAtkHitCount);
        Events.OnPlayerAtkHitComboCounted.Publish(playerAtkHitComboCount);
    }
    public void OnPlayerAttackMissed()
    {
        playerAtkMissedCount++;
        playerAtkHitComboCount = 0;
    }
    public void SetInput(bool enable)
    {
        playerInputHandler.SetInput(enable);
    }

    
    #region Get Counters Functions
    public int GetCurrentHitCount(){ return playerAtkHitCount; }
    public int GetCurrentHitComboCount(){ return playerAtkHitComboCount; }
    public int GetCurrentMissCount() { return playerAtkMissedCount; }
    #endregion
}
