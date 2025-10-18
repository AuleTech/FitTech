using AuleTech.Core.Patterns.CQRS;
using Bogus;
using FitTech.TestingSupport;
using FitTech.TestingSupport.Faker;
using ICommand = System.Windows.Input.ICommand;

namespace FitTech.UnitTests;

public abstract class BaseCqrsUnitTest<TRequest, TSut> where TRequest : class
{
    protected Faker Faker = new Faker();
    
    protected FitTechFakeGenerators FakeGenerators = FitTechFakeGenerators.Create();
    
    protected abstract TSut CreateSut();
    protected abstract TRequest CreateRequest();
}

public abstract class FitTechCqrsUnitTest<TRequest, TSut> where TRequest : class
{
    protected Faker Faker = new Faker();
    protected RecordFaker<TRequest> Request = new ();  
    
    protected FitTechFakeGenerators FakeGenerators = FitTechFakeGenerators.Create();
    
    protected abstract TSut CreateSut();
}
