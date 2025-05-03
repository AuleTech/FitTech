namespace AuleTech.Core.Reflection;

public static class ReflectionExtensions
{
    public static bool ImplementsGenericInterface(this Type type, Type genericInterface)
    {
        if (type.IsInterface || type.IsAbstract)
        {
            return false;
        }

        return type.GetInterfaces().Any(i => i.IsGenericType &&
                                             i.GetGenericTypeDefinition() == genericInterface);
    }
}
