namespace WECCL.Saves;

public class BetterSaveData
{
    public static BetterSaveData Instance { get; set; } = new();
    
    public List<BetterCharacterData> CharacterData { get; set; } = new();
    
    public void ToRegularSaveData()
    {
        
    }
}