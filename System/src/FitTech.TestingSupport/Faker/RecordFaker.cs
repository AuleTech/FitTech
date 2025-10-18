using Bogus;

namespace FitTech.TestingSupport.Faker;

public class RecordFaker<T> : FitTechFaker<T> where T : class
{
    public RecordFaker()
    {
        this.WithRecord();
    }
}
