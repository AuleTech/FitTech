using FitTech.API.Client.ClientV2.Paths;

namespace FitTech.API.Client.ClientV2;

internal class FitTechApiClientV2 : IFitTechApiClientV2 
{
    public FitTechApiClientV2(ITrainerApiClient trainer, IClientApiClient client, IAuthenticationApiClient auth)
    {
        Trainer = trainer;
        Client = client;
        Auth = auth;
    }

    public ITrainerApiClient Trainer { get; }
    public IClientApiClient Client { get; }
    public IAuthenticationApiClient Auth { get; }
}
