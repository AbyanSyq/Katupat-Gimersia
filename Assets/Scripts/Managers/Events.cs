using System;

public static partial class Events
{
    public static readonly GameEvent<float,float> OnPlayerHealthChanged = new();
    public static readonly GameEvent<float> OnPlayerAttack = new();//when the player attack(throw the spear) (parameter is damage value)
    public static readonly GameEvent OnPlayerAttackHitted = new();//when the spear hit the enemy(parameter is a )
    public static readonly GameEvent OnPlayerAttackMissed = new();//when the spear missed the enemy
    public static readonly GameEvent<float> OnPlayerChargeForceChanged = new();
    public static readonly GameEvent OnPlayerDeath = new();
}
