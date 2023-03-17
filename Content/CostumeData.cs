using Object = UnityEngine.Object;

namespace WECCL.Content;

internal class CostumeData
{
    internal string FilePrefix { get; set; }
    internal string InternalPrefix { get; set; }
    internal Type Type { get; set; }
    internal List<Tuple<string, Object>> CustomObjects { get; set; } = new();
    
    internal int Count => CustomObjects.Count;
    
    internal CostumeData(string filePrefix, string internalPrefix, Type type)
    {
        FilePrefix = filePrefix;
        InternalPrefix = internalPrefix;
        Type = type;
    }
    
    internal CostumeData(string prefix, Type type)
    {
        FilePrefix = prefix;
        InternalPrefix = prefix;
        Type = type;
    }
    
    internal void AddCustomObject(string name, Object obj)
    {
        if (obj.GetType() != Type)
        {
            throw new ArgumentException($"Object type {obj.GetType()} of {obj.name} does not match expected type {Type}.");
        }
        CustomObjects.Add(new Tuple<string, Object>(name, obj));
    }
}