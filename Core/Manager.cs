namespace KrallikYonetimi.Core;

public class Manager<T> where T : class
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }

    public bool Remove(T item) => _items.Remove(item);

    public IReadOnlyList<T> GetAll() => _items.AsReadOnly();

    public T? Find(Predicate<T> match) => _items.Find(match);

    public int Count => _items.Count;
}
