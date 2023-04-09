using Newtonsoft.Json;
using WECCL.Content;

namespace WECCL.Saves;

public class BetterCharacterData
{
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

    public int[] experience;

    public string[] relationshipC;

    public int? negotiated;

    public int? agreement;

    public int[] moveFront;

    public int[] moveBack;

    public int[] moveGround;

    public int[] moveAttack;

    public int[] moveCrush;

    public int[] taunt;

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

    public static BetterCharacterData FromRegularCharacterData(Character character, Character[] allCharacters)
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
            bcd.relationshipC[i] = allCharacters[character.relationship[i]].name + "@" + character.relationship[i];
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
}