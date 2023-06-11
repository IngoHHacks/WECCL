namespace WECCL.Utils;

public class LimitedDictionary<TKey, TValue> : SortedDictionary<TKey, TValue>
{
    private readonly int _limit;

    public LimitedDictionary(int limit)
    {
        _limit = limit;
    }

    public new void Add(TKey key, TValue value)
    {
        if (Count >= _limit)
        {
            Remove(this.First().Key);
        }

        base.Add(key, value);
    }
}