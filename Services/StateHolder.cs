namespace OrleansEmailApp.Services;

public interface IStateHolderGrain<T> : IGrainWithStringKey
{
    Task<T> GetItem();
    Task<T> SetItem(T obj);
}

public class StateHolder<T>
{
    public StateHolder() : this(default(T))
    {
    }

    public StateHolder(T value)
    {
        Value = value;
    }

    public T Value { get; set; }
}

public abstract class StateHolderGrain<T> : Grain<StateHolder<T>>, IStateHolderGrain<T>
{
    private bool _stateHasChanged = false;
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("TIMER REGISTERED");
        RegisterTimer(WriteState, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        return base.OnActivateAsync(cancellationToken);
    }

    private async Task WriteState(object _)
    {
        if (_stateHasChanged)
        {
            Console.WriteLine("WRITING TO STORAGE");
            await WriteStateAsync();
            _stateHasChanged = false;
        }
    }
    
    public Task<T> GetItem()
    {
        return Task.FromResult(State.Value);
    }
    
    public async Task<T> SetItem(T item)
    {
        if (item is not null && !item.Equals(State.Value))
        {
            Console.WriteLine("NORMAL WRITE");
            State.Value = item;
            _stateHasChanged = true;
        }
        return State.Value;
    }
}