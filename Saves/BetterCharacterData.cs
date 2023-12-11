using Newtonsoft.Json;
using System.Reflection;
using WECCL.Content;

namespace WECCL.Saves;

public class BetterCharacterData
{
    public int? absent;

    public int? age;

    public int? agreement;

    public float? angle;

    public int? anim;

    public float? armMass;

    public float? bodyMass;

    public int? clause;

    public int? contract;

    public BetterCostumeData[] costumeC;

    public int? dead;

    public int?[] experience = new int?[11];

    public int? fed;

    public int? gender;

    public int? grudge;

    public float? headSize;

    public float? health;

    public int? heel;

    public float? height;

    public int? home;

    public int? id;

    public int? injury;

    public int? injuryTime;

    public float? legMass;

    public int? light;

    public int? location;

    public int?[] moveAttack = new int?[9];

    public int?[] moveBack = new int?[9];

    public int?[] moveCrush = new int?[9];

    public int?[] moveFront = new int?[17];

    public int?[] moveGround = new int?[7];

    public float? muscleMass;

    public string musicC;

    public float? musicSpeed;

    public string name;

    public int? negotiated;

    public int? news;

    public float?[] newStat = new float?[7];

    public int? oldFed;

    public float?[] oldStat = new float?[7];

    public int? platform;

    public int? possessive;

    public int? promo;

    public int? promoVariable;

    public int? prop;

    public int? pyro;

    public string[] relationshipC = new string[7];

    public int? role;

    public int? salary;

    public int?[] scar = new int?[17];

    public int? seat;

    public float? spirit;

    public int? stance;

    public float?[] stat = new float?[7];

    public int?[] taunt = new int?[4];

    public int? toilet;
    public string VERSION = "1.0.0";

    public float? voice;

    public int? worked;

    public int? worth;

    public float? x;

    public float? y;

    public float? z;

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

            bcd.relationshipC[i] = character.relationship[i] == 0
                ? "0"
                : allCharacters[character.relationship[i]].name + "@" + character.relationship[i];
        }

        if (character.music > VanillaCounts.Data.MusicCount)
        {
            int index = character.music - VanillaCounts.Data.MusicCount - 1;
            string music = ContentMappings.ContentMap.MusicNameMap[index];
            bcd.musicC = "Custom/" + music;
        }
        else
        {
            bcd.musicC = "Vanilla/" + character.music;
        }

        bcd.grudge = 0;

        return bcd;
    }

    public Character ToRegularCharacter(Character[] allCharacters)
    {
        Character character = JsonConvert.DeserializeObject<Character>(JsonConvert.SerializeObject(this))!;
        character.costume = new Costume[this.costumeC.Length];
        for (int i = 0; i < this.costumeC.Length; i++)
        {
            if (this.costumeC[i] == null)
            {
                character.costume[i] = null;
                continue;
            }

            character.costume[i] = this.costumeC[i].ToRegularCostume();
        }

        character.relationship = new int[this.relationshipC.Length];
        for (int i = 0; i < this.relationshipC.Length; i++)
        {
            if (this.relationshipC[i] == "0")
            {
                character.relationship[i] = 0;
                continue;
            }

            string[] split = this.relationshipC[i].Split('@');
            string name = split[0];
            try
            {
                character.relationship[i] = allCharacters.Single(c => c != null && c.name != null && c.name == name).id;
            }
            catch (Exception)
            {
                character.relationship[i] = int.Parse(split[1]);
                Plugin.Log.LogWarning("Failed to find character with name " + name + ", using id instead.");
            }
        }

        if (this.musicC.StartsWith("Custom/"))
        {
            try
            {
                string music = this.musicC.Substring(7);
                int index = ContentMappings.ContentMap.MusicNameMap.IndexOf(music);
                character.music = index + VanillaCounts.Data.MusicCount + 1;
            }
            catch (Exception)
            {
                Plugin.Log.LogWarning("Failed to find music from name " + this.musicC + ", setting to 0.");
                character.music = 0;
            }
        }
        else
        {
            character.music = int.Parse(this.musicC.Substring(8));
        }
        character.grudge = 0;
        character.team = 0;

        return character;
    }

    public void MergeIntoCharacter(Character character)
    {
        foreach (FieldInfo field in typeof(BetterCharacterData).GetFields())
        {
            if (field.FieldType.IsArray)
            {
                Array array = (Array)field.GetValue(this);
                if (array == null)
                {
                    continue;
                }

                bool allNull = true;
                bool allNonNull = true;
                foreach (object element in array)
                {
                    if (element != null)
                    {
                        allNull = false;
                    }
                    else
                    {
                        allNonNull = false;
                    }
                }

                if (allNull)
                {
                    field.SetValue(this, null);
                }
                else if (!allNonNull)
                {
                    throw new Exception("It is not possible to merge arrays with both null and non-null elements.");
                }
            }
        }

        // Ignore nulls and nulls in arrays
        JsonConvert.PopulateObject(JsonConvert.SerializeObject(this), character,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        character.grudge = 0;
        character.team = 0;
    }
}