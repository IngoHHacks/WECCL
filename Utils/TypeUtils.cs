using WECCL.Animation;

namespace WECCL.Utils;

public static class TypeUtils
{
    public static bool IsValid(Type type, string value)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Boolean:
                return AnimationParser.ParseBool(value).Result;
            case TypeCode.Byte:
                return byte.TryParse(value, out _);
            case TypeCode.Char:
                return char.TryParse(value, out _);
            case TypeCode.DateTime:
                return DateTime.TryParse(value, out _);
            case TypeCode.Decimal:
                return decimal.TryParse(value, out _);
            case TypeCode.Double:
                return double.TryParse(value, out _);
            case TypeCode.Int16:
                return short.TryParse(value, out _);
            case TypeCode.Int32:
                if (type.IsEnum)
                {
                    bool name = Enum.GetNames(type).Any(x => x.Equals(value, StringComparison.OrdinalIgnoreCase));
                    bool index = int.TryParse(value, out int result) && Enum.IsDefined(type, result);
                    return name || index;
                }
                return AnimationParser.ParseInt(value).Result;
            case TypeCode.Int64:
                return long.TryParse(value, out _);
            case TypeCode.SByte:
                return sbyte.TryParse(value, out _);
            case TypeCode.Single:
                return AnimationParser.ParseFloat(value).Result;
            case TypeCode.String:
                return true;
            case TypeCode.UInt16:
                return ushort.TryParse(value, out _);
            case TypeCode.UInt32:
                return uint.TryParse(value, out _);
            case TypeCode.UInt64:
                return ulong.TryParse(value, out _);
            case TypeCode.Object:
                return false;
            default:
                return false;
        }
    }
}