namespace WECCL.Utils;

public class LimitedDictionary<TKey, TValue> : SortedDictionary<TKey, TValue>
{
    private readonly int _limit;

    public LimitedDictionary(int limit)
    {
        this._limit = limit;
    }

    public new void Add(TKey key, TValue value)
    {
        if (this.Count >= this._limit)
        {
            Remove(this.First().Key);
        }

        base.Add(key, value);
    }
}