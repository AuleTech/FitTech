using System.Net;
using System.Text.RegularExpressions;

namespace FitTech.TestingSupport;

public static class FitTechEmailExtensions
{
    public static string? GetForgotPasswordTokenFromEmailBody(string emailBody)
    {
        string pattern = @"token=([^&""]+)";
        var match = Regex.Match(emailBody, pattern);

        if (!match.Success)
        {
            return null;
        }
        
        string tokenEncoded = match.Groups[1].Value;
        return WebUtility.UrlDecode(tokenEncoded);
    }
}
