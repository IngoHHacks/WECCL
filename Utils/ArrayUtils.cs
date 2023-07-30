namespace WECCL.Utils;

public static class ArrayUtils
{
    public static int[] GenerateArrayForNum(int characters)
    {
        if (characters <= 0)
        {
            return new[] { -1 };
        }

        int[] arr = new int[characters + 1];
        for (int i = 1; i <= characters; i++)
        {
            arr[i] = i;
        }

        return arr;
    }

    public static IEnumerable<Character> SortBy(this IEnumerable<Character> c, int propertyId)
    {
        switch (propertyId)
        {
            case 1:
                return c.OrderBy(x => x.name);
            case 2:
                return c.OrderByDescending(x => x.health);
            case >= 3 and <= 8:
                return c.OrderByDescending(x => x.stat[propertyId - 2]);
            case 9:
                return c.OrderByDescending(x => x.salary * x.contract);
            case 10:
                return c.OrderByDescending(x => x.age);
            case 11:
                return c.OrderBy(x => x.age);
            default:
                return c;
        }
    }
}