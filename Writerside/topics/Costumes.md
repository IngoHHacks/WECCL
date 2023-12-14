# 👕 Costumes

<show-structure for="chapter" depth="2"/>

<link-summary>
How to add custom costumes to the game.
</link-summary>

## Adding Costumes
Costumes are added by inserting <include from="snippets.md" element-id="prefixed"/> image files into <include from="snippets.md" element-id="apath"/>.

## Prefixes
<list columns="3">
<li>arms_elbow_pad</li>
<li>arms_flesh</li>
<li>arms_glove</li>
<li>arms_material</li>
<li>arms_wristband</li>
<li/>
<li>body_collar</li>
<li>body_flesh_female</li>
<li>body_flesh_male</li>
<li>body_material</li>
<li>body_pattern</li>
<li/>
<li>face_beard</li>
<li>face_female</li>
<li>face_male</li>
<li>face_mask</li>
<li/>
<li/>
<li>hair_shave</li>
<li>hair_texture_solid</li>
<li>hair_texture_transparent</li>
<li>legs_flesh</li>
<li>legs_footwear</li>
<li>legs_kneepad</li>
<li>legs_laces</li>
<li>legs_material</li>
<li>legs_pattern</li>
</list>

### Metadata

Metadata files can be used to add additional information to assets, e.g. skin tone. Metadata files must be named the
same as the asset, but with the `.meta` extension.
Example: `abc.png` and `abc.meta`.
Meta files must be a newline-separated list of key-value pairs in the format `key: value` (space is optional).  
The following keys are supported:

| Key       | Description                                                                                                                                                                                                                                                                       |
|-----------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| skin_tone | The skin tone of the character. Must be formatted as `r,g,b` (e.g. `1.0, 0.0, 0.0`) or an HTML string (e.g. `#FF0000` or `red`). `1.0, 1.0, 1.0` is the default skin tone (white). `0.75, 0.75, 0.75` and `0.5, 0.45, 0.3` are the two other skin tones used in the vanilla game. |

### Supported Formats
<list columns="3">
<li>PNG</li>
<li>JPG</li>
<li>JPEG</li>
<li>BMP</li>
<li>TGA</li>
<li>GIF</li>
</list>

## Meshes

Meshes work similarly to textures, but require an asset bundle to be created through Unity.
For more information on how to create asset bundles, see [Creating Asset Bundles](AssetBundles.md).
The first submesh will be the one affected by the game's mesh color setting. Others must be set manually in the metadata file.  

### Prefixes {id="prefixes2"}
<list columns="3">
<li>arms_shape</li>
<li/>
<li/>
<li>body_shape</li>
<li/>
<li/>
<li>face_headwear</li>
<li>face_shape</li>
<li/>
<li>hair_extension</li>
<li>hair_hairstyle_solid</li>
<li>hair_hairstyle_transparent</li>
<li>legs_shape</li>
<li/>
<li/>
</list>

### Metadata {id="metadata2"}

Metadata files can be used to add additional information to meshes, e.g. mesh color. Metadata files must be named the
same as the mesh, but with the `.meta` extension.
Example: `abc` and `abc.meta`.
Meta files must be a newline-separated list of key-value pairs in the format `key: value` (space is optional).
The following keys are supported:

| Key           | Description                                                                                                                                                                                                         |
|---------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| scale         | The scale of the mesh. Must be formatted as `x,y,z` (e.g. `1.0, 1.0, 1.0`). `1.0, 1.0, 1.0` is the default scale.                                                                                                   |
| position      | The position of the mesh. Must be formatted as `x,y,z` (e.g. `0.0, 0.0, 0.0`). `0.0, 0.0, 0.0` is the default position.                                                                                             |
| rotation      | The rotation of the mesh. Must be formatted as `x,y,z` (e.g. `0.0, 0.0, 0.0`). `0.0, 0.0, 0.0` is the default rotation.                                                                                             |
| submeshXcolor | The color of submesh X. Must be formatted as `r,g,b` (e.g. `1,0, 0.0, 0.0`) or an HTML string (e.g. `#FF0000` or `red`). Overriding submesh 0 is not supported, as it is affected by the game's mesh color setting. |
