using FitTech.API.Client.ClientV2.Paths;

namespace FitTech.API.Client.ClientV2;

public interface IFitTechApiClientV2
{
    ITrainerApiClient Trainer { get; }
    IClientApiClient Client { get; }
    IAuthenticationApiClient Auth { get; }
}
