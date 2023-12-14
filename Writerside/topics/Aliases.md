# 📝 Aliases

## What are Aliases?
Aliases are a way to give a file a different name.  
For example, you could have a file named `song1.ogg` and give it the alias `MyFirstSong`.  
This way, the save file stores `MyFirstSong` instead of `song1.ogg`, which makes it easier to change the file name later.  
For example, if you want to rename `song1.ogg` to `super_cool_song.ogg`, you can change the reference in the alias file from `song1.ogg` to `super_cool_song.ogg` and the save file will still work.

## Creating Aliases
Files can be aliased by placing a `.aliases` file inside <include from="snippets.md" element-id="apath"/>.  
The file must contain a list of aliases in the format `file=alias`.
Example:
```
song1.ogg=MyFirstSong
subfolder/song2.ogg=MySecondSong
path/to/song3.ogg=MyThirdSong
```
Left is file name, right is alias.  
The path is relative to <include from="snippets.md" element-id="apath"/>.