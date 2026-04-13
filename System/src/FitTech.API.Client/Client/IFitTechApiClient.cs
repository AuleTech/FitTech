using FitTech.API.Client.Client.Paths;

namespace FitTech.API.Client.Client;

public interface IFitTechApiClient
{
    ITrainerApiClient Trainer { get; }
    IClientApiClient Client { get; }
    IAuthenticationApiClient Auth { get; }
}
