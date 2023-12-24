using Newtonsoft.Json;
// ReSharper disable InconsistentNaming
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace WECCL.Saves;

internal class BetterCharacterDataFile
{
    internal string _guid;
    public BetterCharacterData characterData;
    public string findMode;
    public string findName;
    public string overrideMode;

    [JsonIgnore]
    public BetterCharacterData CharacterData
    {
        get
        {
            if (this.characterData == null)
            {
                return new BetterCharacterData();
            }

            return this.characterData;
        }
    }

    [JsonIgnore]
    public string OverrideMode
    {
        get
        {
            if (this.overrideMode == null)
            {
                return "append";
            }

            return this.overrideMode;
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

    [JsonIgnore] public string FindName => this.findName;
}