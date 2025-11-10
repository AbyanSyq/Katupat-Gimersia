using UnityEngine;
using UnityEngine.InputSystem;


public static partial class Events
{
    
}
public class GameplayManager : SingletonMonoBehaviour<GameplayManager>
{
    [Header("Player Log")]
    [SerializeField] private int playerAttackHittedCount;//when the spear hit the enemy
    [SerializeField] private int playerAttackHittedCombo;//when the spear hit the enemy
    [SerializeField] private int playerAttackMissedCount;//when the spear missed the enemy

    [Header("Game State")]
    [SerializeField] private bool isGamePaused = false;
    public PlayerInputAction playerInputAction { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        playerInputAction = new PlayerInputAction();
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
        if (enable)
        {
            playerInputAction.Enable();
        }
        else
        {
            playerInputAction.Disable();
        }
    }
    public void PauseGame(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
    }
    public void ResumeGame(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    
    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }   

}
