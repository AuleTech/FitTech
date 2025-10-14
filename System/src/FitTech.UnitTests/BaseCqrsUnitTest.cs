using AuleTech.Core.Patterns.CQRS;
using Bogus;
using ICommand = System.Windows.Input.ICommand;

namespace FitTech.UnitTests;

public abstract class BaseCqrsUnitTest<TRequest, TSut>
{
    protected Faker Faker = new Faker();
    
    protected abstract TSut CreateSut();
    protected abstract TRequest CreateRequest();
}
