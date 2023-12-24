namespace WECCL.Animation;

internal interface IAnimationArgument
{
    public string Name { get; set; }
    
    public string DefaultValue { get; set; }
    
    public bool Required { get; set; }
    
    public object Parse(string value);
    
    public bool Validate(string value);
    
    public bool TryParse(string value, out object result);
    
}

internal class AnimationArgument<T> : IAnimationArgument
{
    public string Name { get; set; }
    
    public bool Required { get; set; } 
    
    public string DefaultValue { get; set; }
    
    public Func<string, (T Value, bool Result)> Parser { get; set; }
    
    public AnimationArgument(string name, Func<string, (T Value, bool Result)> parser, string defaultValue = null)
    {
        Name = name;
        Parser = parser;
        DefaultValue = defaultValue;
        Required = defaultValue == null;
    }
    
    public object Parse(string value)
    {
        return Parser(value);
    }
    
    public bool Validate(string value)
    {
        return Parser(value).Result;
    }
    
    public bool TryParse(string value, out object result)
    {
        var parsed = Parser(value);
        result = parsed.Value;
        return parsed.Result;
    }
}