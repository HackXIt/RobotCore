namespace RobotCore.Library.Utility;

public static class EnumExtensions
{
    public static string GetStringValue(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            
            if (fieldInfo == null) return value.ToString();

            var attributes = (StringValueAttribute[])fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false);

            // Return the first if there is a match, otherwise fallback to default enum value
            return attributes.Length > 0 ? attributes[0].Value : value.ToString();
        }
}