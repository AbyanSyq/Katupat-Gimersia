using System;

public class GameEvent<T>
{
    private event Action<T> action;

    public void Publish(T param)
    {
        action?.Invoke(param);
    }

    public void Add(Action<T> subscriber)
    {
        action += subscriber;
    }

    public void Remove(Action<T> subscriber)
    {
        action -= subscriber;
    }
}
