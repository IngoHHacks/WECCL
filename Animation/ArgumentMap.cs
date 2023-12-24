namespace WECCL.Animation;

internal class ArgumentMap
{
    public Dictionary<string, string> Arguments { get; set; } = new();
    
    public ArgumentMap()
    {
        
    }
    
    public void Add(string name, string value)
    {
        Arguments.Add(name, value);
    }
    
    public string this[string name]
    {
        get => Arguments[name];
    }
}