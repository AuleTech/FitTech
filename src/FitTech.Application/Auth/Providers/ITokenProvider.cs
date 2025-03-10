using FitTech.Domain.Entities;

namespace FitTech.Application.Auth.Providers;

public interface ITokenProvider
{
    string Create(FitTechUser user);
}
