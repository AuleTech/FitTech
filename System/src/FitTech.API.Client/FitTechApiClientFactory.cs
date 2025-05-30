﻿using FitTech.API.Client.Configuration;

namespace FitTech.API.Client;

public class FitTechApiClientFactory : IFitTechApiClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly FitTechApiConfiguration _configuration;

    public FitTechApiClientFactory(IHttpClientFactory httpClientFactory, FitTechApiConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public IFitTechApiClient Create()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration.Url);

        return new FitTechApiClient(client);
    }
}

public interface IFitTechApiClientFactory
{
    IFitTechApiClient Create();
}
