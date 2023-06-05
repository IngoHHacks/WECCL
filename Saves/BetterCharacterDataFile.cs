using Newtonsoft.Json;

namespace WECCL.Saves;

public class BetterCharacterDataFile
{
    public BetterCharacterData characterData;
    public string overrideMode;
    public string findMode;
    public string findName;

    internal string _guid;
    
    [JsonIgnore]
    public BetterCharacterData CharacterData
    {
        get
        {
            if (characterData == null)
            {
                return new BetterCharacterData();
            }
            return characterData;
        }
    }
    
    [JsonIgnore]
    public string OverrideMode
    {
        get
        {
            if (overrideMode == null)
            {
                return "append";
            }
            return overrideMode;
        }
    }
    
    [JsonIgnore]
    public string FindMode
    {
        get
        {
            if (this.findMode == null)
            {
                return this.overrideMode == "append" ? "" : "name_then_id";
            }
            return this.findMode;
        }
    }
    
    [JsonIgnore]
    public string FindName => this.findName;
}