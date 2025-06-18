using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;

namespace FitTech.Application.Auth.Dtos;

public abstract record SelfValidatedDto
{
    public abstract Result Validate();
}
