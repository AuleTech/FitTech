using AuleTech.Core.Patterns.Result;

namespace FitTech.Application;

public abstract record SelfValidatedDto
{
    public abstract Result Validate();
}
