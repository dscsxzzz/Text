using System.ComponentModel;

namespace SharedModels.RabbitMQHelpers;
public enum Queue
{
    [Description("userQueue")]
    UserQueue = 1,
    [Description("aiModelQueue")]
    AIModelQueue,
}

public static class EnumExtensions
{
    public static string GetDescription<T>(this T enumValue) where T : Enum
    {
        var type = enumValue.GetType();
        var member = type.GetMember(enumValue.ToString());
        var attributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

        return attributes.Length > 0
            ? ((DescriptionAttribute)attributes[0]).Description
            : enumValue.ToString(); // Fallback to enum name if no Description is present
    }
}
