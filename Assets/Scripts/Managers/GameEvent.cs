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
public class GameEvent<T1, T2>
{
    private event Action<T1, T2> action;

    public void Publish(T1 param1, T2 param2)
    {
        action?.Invoke(param1, param2);
    }

    public void Add(Action<T1, T2> subscriber)
    {
        action += subscriber;
    }

    public void Remove(Action<T1, T2> subscriber)
    {
        action -= subscriber;
    }
}
public class GameEvent
{
    private event Action action;

    public void Publish()
    {
        action?.Invoke();
    }

    public void Add(Action subscriber)
    {
        action += subscriber;
    }

    public void Remove(Action subscriber)
    {
        action -= subscriber;
    }
}