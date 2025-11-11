using UnityEngine;
using UnityEngine.InputSystem;


public static partial class Events
{
    public static readonly GameEvent<int> OnPlayerAttackHittedCount = new GameEvent<int>();
    public static readonly GameEvent<int> OnPlayerAttackHittedCombo = new GameEvent<int>();
}
public class GameplayManager : SingletonMonoBehaviour<GameplayManager>
{
    [SerializeField, ReadOnly] private PlayerController3D playerController;
    [SerializeField, ReadOnly] private PlayerInputHandler playerInputHandler;
    public PlayerController3D PlayerController { get => playerController; }
    [Header("Player Log")]
    [SerializeField] private int playerAttackHittedCount;//when the spear hit the enemy
    [SerializeField] private int playerAttackHittedCombo;//when the spear hit the enemy
    [SerializeField] private int playerAttackMissedCount;//when the spear missed the enemy

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
        playerAttackHittedCount++;
        playerAttackHittedCombo++;
    }
    public void OnPlayerAttackMissed()
    {
        playerAttackMissedCount++;
        playerAttackHittedCombo = 0;
    }
    public void SetInput(bool enable)
    {
        playerInputHandler.SetInput(enable);
    }
}
