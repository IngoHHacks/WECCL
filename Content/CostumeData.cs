using Object = UnityEngine.Object;

namespace WECCL.Content;

internal class CostumeData
{
    internal CostumeData(string filePrefix, string internalPrefix, Type type)
    {
        this.FilePrefix = filePrefix;
        this.InternalPrefix = internalPrefix;
        this.Type = type;
    }

    internal CostumeData(string prefix, Type type)
    {
        this.FilePrefix = prefix;
        this.InternalPrefix = prefix;
        this.Type = type;
    }

    internal string FilePrefix { get; set; }
    internal string InternalPrefix { get; set; }
    internal Type Type { get; set; }
    internal List<Tuple<string, Object, List<Tuple<string, string>>>> CustomObjects { get; set; } = new();

    internal int Count => this.CustomObjects.Count;

    internal void AddCustomObject(string name, Object obj, List<Tuple<string, string>> meta)
    {
        if (obj.GetType() != this.Type)
        {
            throw new ArgumentException(
                $"Object type {obj.GetType()} of {obj.name} does not match expected type {this.Type}.");
        }

        this.CustomObjects.Add(new Tuple<string, Object, List<Tuple<string, string>>>(name, obj, meta));
    }
}