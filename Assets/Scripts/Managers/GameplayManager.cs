using UnityEngine;
using UnityEngine.InputSystem;


public static partial class Events
{
    public static readonly GameEvent OnGameplayStarted = new GameEvent();
    public static readonly GameEvent<int> OnPlayerAtkTotalHitCounted = new GameEvent<int>();
    public static readonly GameEvent<int> OnPlayerAtkComboCounted = new GameEvent<int>();
    public static readonly GameEvent<float> OnThrowCooldownChanged = new GameEvent<float>();//percentage
}
public class GameplayManager : SingletonMonoBehaviour<GameplayManager>
{
    [SerializeField, ReadOnly] private PlayerController3D playerController;
    [SerializeField, ReadOnly] private PlayerInputHandler playerInputHandler;
    public PlayerController3D PlayerController { get => playerController; }
    [Header("Player Log")]
    [SerializeField] private int playerAtkTotalHitCount;//when the spear hit the enemy
    [SerializeField] private int playerAtkComboCount;//when the spear hit the enemy
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
        playerAtkTotalHitCount++;
        playerAtkComboCount++;
        
        Events.OnPlayerAtkTotalHitCounted.Publish(playerAtkTotalHitCount);
        Events.OnPlayerAtkComboCounted.Publish(playerAtkComboCount);
    }
    public void OnPlayerAttackMissed()
    {
        playerAtkMissedCount++;
        playerAtkComboCount = 0;

        Events.OnPlayerAtkComboCounted.Publish(playerAtkComboCount);
    }
    public void SetInput(bool enable)
    {
        playerInputHandler.SetInput(enable);
    }

    
    #region Get Counters Functions
    public int GetCurrentTotalHitCount(){ return playerAtkTotalHitCount; }
    public int GetCurrentComboCount(){ return playerAtkComboCount; }
    public int GetCurrentMissCount() { return playerAtkMissedCount; }
    #endregion
}
