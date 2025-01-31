namespace SliLib.Events;


public class SliActionEvent<T>
{
    List<Action<T>> subscribers = [];

    public void Subscribe(Action<T> call)
    {
        subscribers.Add(call);
    }
    public void Unsubscribe(Action<T> call)
    {
        subscribers.Remove(call);
    }
    public void Trigger(T data)
    {
        foreach (var sub in subscribers)
        {
            sub.Invoke(data);
        }
    }
}

public class SliFuncEvent<T>
{
    List<Func<T>> subscribers = [];

    public void Subscribe(Func<T> call)
    {
        subscribers.Add(call);
    }
    public void Unsubscribe(Func<T> call)
    {
        subscribers.Remove(call);
    }
    public void Trigger()
    {
        foreach (var sub in subscribers)
        {
            sub.Invoke();
        }
    }
}
