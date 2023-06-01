using Newtonsoft.Json;
using WECCL.Content;

namespace WECCL.Saves;

public class BetterCharacterData
{
    public string VERSION = "1.0.0";
    
    public int? id;

    public string name;

    public float? voice;

    public int? role;

    public int? heel;

    public string? musicC;

    public int? light;

    public int? prop;

    public int? pyro;

    public float? musicSpeed;

    public int? gender;

    public float? height;

    public int? age;

    public float? headSize;

    public float? bodyMass;

    public float? muscleMass;

    public float? armMass;

    public float? legMass;

    public BetterCostumeData[] costumeC;

    public int[] scar = new int[17];

    public float[] stat = new float[7];

    public float[] oldStat = new float[7];

    public float[] newStat = new float[7];

    public float? health;

    public float? spirit;

    public int? injury;

    public int? injuryTime;

    public int? dead;

    public int? absent;

    public int? worked;

    public int? news;

    public int? worth;

    public int? contract;

    public int? salary;

    public int? clause;

    public int? fed;

    public int? oldFed;

    public int[] experience = new int[11];

    public string[] relationshipC = new string[7];

    public int? negotiated;

    public int? agreement;

    public int[] moveFront = new int[17];

    public int[] moveBack = new int[9];

    public int[] moveGround = new int[7];

    public int[] moveAttack = new int[9];

    public int[] moveCrush = new int[9];

    public int[] taunt = new int[4];

    public int? stance;

    public int? location;

    public float? x;

    public float? y;

    public float? z;

    public int? platform;

    public int? seat;

    public int? anim;

    public int? promo;

    public int? promoVariable;

    public int? home;

    public float? angle;

    public int? toilet;

    public int? grudge;

    public int? possessive;

    public static BetterCharacterData FromRegularCharacter(Character character, Character[] allCharacters)
    {
        BetterCharacterData bcd =
            JsonConvert.DeserializeObject<BetterCharacterData>(JsonConvert.SerializeObject(character))!;
        bcd.costumeC = new BetterCostumeData[character.costume.Length];
        for (int i = 0; i < character.costume.Length; i++)
        {
            bcd.costumeC[i] = BetterCostumeData.FromRegularCostumeData(character.costume[i]);
        }
        bcd.relationshipC = new string[character.relationship.Length];
        for (int i = 0; i < character.relationship.Length; i++)
        {
            if (character.relationship[i] > allCharacters.Length)
            {
                bcd.relationshipC[i] = "0";
                continue;
            }
            bcd.relationshipC[i] = character.relationship[i] == 0 ? "0" : allCharacters[character.relationship[i]].name + "@" + character.relationship[i];
        }
        if (character.music > VanillaCounts.MusicCount)
        {
            var index = character.music - VanillaCounts.MusicCount - 1;
            var music = ContentMappings.ContentMap.MusicNameMap[index];
            bcd.musicC = "Custom/" + music;
        }
        else
        {
            bcd.musicC = "Vanilla/" + character.music;
        }
        return bcd;
    }

    public Character ToRegularCharacter(Character[] allCharacters)
    {
        Character character = JsonConvert.DeserializeObject<Character>(JsonConvert.SerializeObject(this))!;
        character.costume = new Costume[costumeC.Length];
        for (int i = 0; i < costumeC.Length; i++)
        {
            if (costumeC[i] == null)
            {
                character.costume[i] = null;
                continue;
            }
            character.costume[i] = costumeC[i].ToRegularCostume();
        }
        character.relationship = new int[relationshipC.Length];
        for (int i = 0; i < relationshipC.Length; i++)
        {
            if (this.relationshipC[i] == "0")
            {
                character.relationship[i] = 0;
                continue;
            }
            var split = relationshipC[i].Split('@');
            var name = split[0];
            try
            {
                character.relationship[i] = allCharacters.Single(c => c != null && c.name != null && c.name == name).id;
            }
            catch (Exception e)
            {
                character.relationship[i] = int.Parse(split[1]);
                Plugin.Log.LogWarning("Failed to find character with name " + name + ", using id instead.");
            }
        }
        if (musicC.StartsWith("Custom/"))
        {
            var music = musicC.Substring(7);
            var index = ContentMappings.ContentMap.MusicNameMap.IndexOf(music);
            character.music = index + VanillaCounts.MusicCount + 1;
        }
        else
        {
            character.music = int.Parse(musicC.Substring(8));
        }
        return character;
    }
    
    public void MergeIntoCharacter(Character character)
    {
        JsonConvert.PopulateObject(JsonConvert.SerializeObject(this), character);
    }
}