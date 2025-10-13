using Resend;

namespace FitTech.Application.Configuration;

public class ResendSettings : ResendClientOptions
{
    public string EmailFitTech { get; set; } = string.Empty;
}
