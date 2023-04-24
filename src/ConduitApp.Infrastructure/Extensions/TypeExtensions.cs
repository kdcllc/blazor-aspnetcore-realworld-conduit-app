namespace ConduitApp.Infrastructure.Extensions;

public static class TypeExtensions
{
    public static string GetTypeName(this Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            return $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            return type.Name;
        }
    }

    public static string GetTypeName(this object @object) => @object.GetType().GetTypeName();
}
