namespace WECCL.Saves;

internal class BetterSaveData
{
    public static BetterSaveData Instance { get; set; } = new();

    public List<BetterCharacterData> CharacterData { get; set; } = new();

    public void ToRegularSaveData(ref SaveData baseData, Character[] allCharacters)
    {
        baseData.savedChars = new Character[this.CharacterData.Count];
        for (int i = 0; i < this.CharacterData.Count; i++)
        {
            baseData.savedChars[i] = this.CharacterData[i].ToRegularCharacter(allCharacters);
        }
    }

    public void FromRegularSaveData(SaveData baseData, Character[] allCharacters)
    {
        this.CharacterData = new List<BetterCharacterData>(baseData.savedChars.Length);
        for (int i = 0; i < baseData.savedChars.Length; i++)
        {
            this.CharacterData.Add(BetterCharacterData.FromRegularCharacter(baseData.savedChars[i], allCharacters));
        }
    }
}