namespace WECCL.Utils;

public static class TextureUtils
{
    public static Texture2D ResizeTexture(Texture2D original, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        RenderTexture.active = rt;
        Graphics.Blit(original, rt);
        Texture2D resized = new(width, height);
        resized.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resized.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return resized;
    }
}