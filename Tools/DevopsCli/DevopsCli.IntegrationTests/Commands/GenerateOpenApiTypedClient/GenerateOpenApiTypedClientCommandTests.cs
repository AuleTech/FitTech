using AuleTech.Core.Patterns;
using AuleTech.Core.System.IO.FileSystem;
using DevopsCli.Core.Commands;
using DevopsCli.Core.Commands.GenerateOpenApiTypedClient;

namespace DevopsCli.IntegrationTests.Commands.GenerateOpenApiTypedClient;

[TestCliContainer]
public class GenerateOpenApiTypedClientCommandTests
{
    private readonly ICommand<GenerateOpenApiTypedClientParams, Result> _sut;
    private readonly ISystemIo _systemIo;
    private string? _workingFolder;
    public GenerateOpenApiTypedClientCommandTests(ICommand<GenerateOpenApiTypedClientParams, Result> sut, ISystemIo systemIo)
    {
        _sut = sut;
        _systemIo = systemIo;
    }

    [Before(Test)]
    public void Setup()
    {
        _workingFolder = _systemIo.Path.GetTempPath($"{Guid.NewGuid().ToString()[..3]}", true, true);
    }

    [After(Test)]
    public async Task TierDownAsync()
    {
        await _systemIo.Directory.DeleteAsync(_workingFolder!, true, CancellationToken.None, false, true);
    }
    
    [Test]
    public async Task GenerateOpenApiTypedClientCommandWorks()
    {
        var commandConfiguration = new GenerateOpenApiTypedClientParams
        {
            OpenApiJsonUrl = "https://petstore.swagger.io/v2/swagger.json",
            OutputFolder = _workingFolder!,
            Namespace = "SampleNamespace.Client.DecoratedProxy"
        };

        var result = await _sut.RunAsync(commandConfiguration, TestContext.Current!.CancellationToken);
        await Assert.That(result.Succeeded).IsTrue();
        //TODO: Add check on working folder
    }
}
