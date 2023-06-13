namespace WECCL.Utils;

public static class VectorUtils
{
    public static Vector3 Round(this Vector3 vector, int decimals)
    {
        return new Vector3((float)Math.Round(vector.x, decimals), (float)Math.Round(vector.y, decimals),
            (float)Math.Round(vector.z, decimals));
    }
}