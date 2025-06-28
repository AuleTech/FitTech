using System.Reflection;
using System.Text;
using AuleTech.Core.System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Hosting;

namespace AuleTech.Core.Maui;

public static class MauiConfigurationExtensions
{
    public static async Task AddAppSettingsAsync(this MauiAppBuilder builder, string projectName,
        Assembly appSettingsAssemblyLocation)
    {
        await using var appsettingsSteam = await GetAppSettingsFile()!.SanitizeAppSettingsAsync();
        await using var environmentAppSettingsStream =
            await GetAppSettingsFile(Environments.Development)!
                .SanitizeAppSettingsAsync(); //TODO: Define how to set environment

        builder.Configuration
            .AddJsonStream(appsettingsSteam)
            .AddJsonStream(environmentAppSettingsStream);

        Stream? GetAppSettingsFile(string? environment = null)
        {
            return appSettingsAssemblyLocation
                .GetManifestResourceStream(string.IsNullOrWhiteSpace(environment)
                    ? $"{projectName}.appsettings.json"
                    : $"{projectName}.appsettings.{environment}.json");
        }
    }

    public static async Task<Stream> SanitizeAppSettingsAsync(this Stream appSettingStream)
    {
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            return appSettingStream;
        }

        var appSettingsText = await appSettingStream.ReadAllTextAsync();

        appSettingsText = appSettingsText.Replace("localhost", "10.0.2.2");

        appSettingStream.Close();
        await appSettingStream.DisposeAsync();

        return Encoding.UTF8.GetBytes(appSettingsText).ToStream();
    }
}
