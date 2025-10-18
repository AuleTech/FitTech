using System.Linq.Expressions;
using Bogus;

namespace FitTech.TestingSupport.Faker;

public class FitTechFaker<T> : Faker<T> where T : class
{
    public void RuleForOverride<TProperty>(Expression<Func<T, TProperty>> propFunc, Func<Bogus.Faker, TProperty> setter)
    {
        Actions.Remove(PropertyName.For(propFunc));
        RuleFor(propFunc, setter);
    }
}
