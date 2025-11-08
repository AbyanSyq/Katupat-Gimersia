using System;

public static partial class Events
{
    public static readonly GameEvent<float> OnPlayerHealthChanged = new GameEvent<float>();
}
