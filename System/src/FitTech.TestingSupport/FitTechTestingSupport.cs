using Microsoft.Extensions.DependencyInjection;
using Resend;

namespace FitTech.TestingSupport;

public static class FitTechTestingSupport
{
    public static IResend GetResendTestClient()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddHttpClient<ResendClient>();
        serviceCollection.Configure<ResendClientOptions>(options => options.ApiToken = "re_BLwgTzK4_Q97rx6znbYiKJ364qU8xeBCd");
        serviceCollection.AddTransient<IResend, ResendClient>();
        var sp = serviceCollection.BuildServiceProvider();
        
        return sp.GetRequiredService<IResend>();
    }
}
