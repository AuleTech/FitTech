using System.Runtime.CompilerServices;
using Bogus;

namespace FitTech.TestingSupport.Faker;

public static class FakerExtensions
{
    public static Faker<T> WithRecord<T>(this Faker<T> faker) where T : class
    {
        faker.CustomInstantiator( _ => (RuntimeHelpers.GetUninitializedObject(typeof(T)) as T)! );
        return faker;
    }
    
    public static Faker<T> WithPrivateConstructors<T>(this Faker<T> faker) where T : class
    {
        faker.CustomInstantiator(f => (Activator.CreateInstance(typeof(T), nonPublic: true) as T)!);
        return faker;
    }
}
