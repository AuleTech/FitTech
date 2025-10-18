namespace FitTech.TestingSupport.Faker;

public class FakerWithPrivateConstructor<T> : FitTechFaker<T> where T : class
{
    public FakerWithPrivateConstructor()
    {
        this.WithPrivateConstructors();
    }
}
