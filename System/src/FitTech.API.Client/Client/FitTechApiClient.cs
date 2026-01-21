using FitTech.API.Client.Client.Paths;

namespace FitTech.API.Client.Client;

internal class FitTechApiClient : IFitTechApiClient 
{
    public FitTechApiClient(ITrainerApiClient trainer, IClientApiClient client, IAuthenticationApiClient auth)
    {
        Trainer = trainer;
        Client = client;
        Auth = auth;
    }

    public ITrainerApiClient Trainer { get; }
    public IClientApiClient Client { get; }
    public IAuthenticationApiClient Auth { get; }
}
