namespace WECCL.Content;

public static class CustomContent
{
    internal static List<AudioClip> CustomClips = new();

    internal static Dictionary<string, CostumeData> CustomCostumes = new()
    {
        { "legs_material", new CostumeData("legs_material", "legs", typeof(Texture2D)) },
        { "legs_flesh", new CostumeData("legs_flesh", typeof(Texture2D)) },
        { "legs_shape", new CostumeData("legs_shape", "custom", typeof(Mesh)) },
        { "body_material", new CostumeData("body_material", "body", typeof(Texture2D)) },
        { "body_flesh_male", new CostumeData("body_flesh_male", "body_flesh", typeof(Texture2D)) },
        { "body_flesh_female", new CostumeData("body_flesh_female", "body_female", typeof(Texture2D)) },
        { "body_shape", new CostumeData("body_shape", "custom", typeof(Mesh)) },
        { "face_female", new CostumeData("face_female", typeof(Texture2D)) },
        { "face_male", new CostumeData("face_male", "face", typeof(Texture2D)) },
        { "face_shape", new CostumeData("face_shape", "custom", typeof(Mesh)) },
        { "arms_material", new CostumeData("arms_material", "arm", typeof(Texture2D)) },
        { "arms_flesh", new CostumeData("arms_flesh", "arm_flesh", typeof(Texture2D)) },
        { "arms_shape", new CostumeData("arms_shape", "custom", typeof(Mesh)) },
        { "arms_glove", new CostumeData("arms_glove", "glove", typeof(Texture2D)) },
        { "legs_footwear_special", new CostumeData("legs_footwear_special", "custom", typeof(Texture2D)) },
        { "legs_footwear", new CostumeData("legs_footwear", "shoes", typeof(Texture2D)) },
        { "body_collar", new CostumeData("body_collar", "collar", typeof(Texture2D)) },
        {
            "hair_texture_transparent", new CostumeData("hair_texture_transparent", "hair_alpha", typeof(Texture2D))
        },
        { "hair_texture_solid", new CostumeData("hair_texture_solid", "hair", typeof(Texture2D)) },
        { "hair_hairstyle_solid", new CostumeData("hair_hairstyle_solid", "custom", typeof(Mesh)) },
        { "hair_hairstyle_transparent", new CostumeData("hair_hairstyle_transparent", "custom", typeof(Mesh)) },
        { "hair_extension", new CostumeData("hair_extension", "custom", typeof(Mesh)) },
        { "hair_shave", new CostumeData("hair_shave", "shave", typeof(Texture2D)) },
        { "face_beard", new CostumeData("face_beard", "beard", typeof(Texture2D)) },
        { "face_mask", new CostumeData("face_mask", "mask", typeof(Texture2D)) },
        { "body_pattern", new CostumeData("body_pattern", typeof(Texture2D)) },
        { "legs_kneepad", new CostumeData("legs_kneepad", "kneepads", typeof(Texture2D)) },
        { "legs_pattern", new CostumeData("legs_pattern", typeof(Texture2D)) },
        { "legs_laces", new CostumeData("legs_laces", "lace", typeof(Texture2D)) },
        { "face_headwear", new CostumeData("face_headwear", "custom", typeof(Mesh)) },
        { "arms_elbow_pad", new CostumeData("arms_elbow_pad", "pad", typeof(Texture2D)) },
        { "arms_wristband", new CostumeData("arms_wristband", "wristband", typeof(Texture2D)) }
    };

    internal static Dictionary<string, Texture2D> ResourceOverridesTextures = new();
    internal static Dictionary<string, AudioClip> ResourceOverridesAudio = new();

    internal static List<Tuple<string, string, Character>> ImportedCharacters = new();
    internal static List<string> FilesToDeleteOnSave = new();
}