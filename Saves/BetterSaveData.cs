namespace WECCL.Saves;

public class BetterSaveData
{
    public static BetterSaveData Instance { get; set; } = new();
    
    public List<BetterCharacterData> CharacterData { get; set; } = new();
    
    public void ToRegularSaveData(ref SaveData baseData, Character[] allCharacters)
    {
        baseData.savedChars = new Character[CharacterData.Count];
        for (var i = 0; i < CharacterData.Count; i++)
        {
            baseData.savedChars[i] = CharacterData[i].ToRegularCharacter(allCharacters);
        }
    }
    
    public void FromRegularSaveData(SaveData baseData, Character[] allCharacters)
    {
        CharacterData = new List<BetterCharacterData>(baseData.savedChars.Length);
        for (var i = 0; i < baseData.savedChars.Length; i++)
        {
            CharacterData.Add(BetterCharacterData.FromRegularCharacter(baseData.savedChars[i], allCharacters));
        }
    }
}