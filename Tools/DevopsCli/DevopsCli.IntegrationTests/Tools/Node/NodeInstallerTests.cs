using DevopsCli.Core.Tools;
using DevopsCli.Core.Tools.Node;

namespace DevopsCli.IntegrationTests.Tools.Node;

[TestCliContainer]
internal class NodeInstallerTests
{
    private readonly IInstaller<NodeTool> _sut;

    public NodeInstallerTests(IEnumerable<IInstaller<NodeTool>> installers)
    {
        _sut = installers.Single(x => x.IsSupported(Environment.OSVersion.Platform));
    }

    [Test]
    [Timeout(10000)]
    public async Task InstallAsyncWorks(CancellationToken cancellationToken)
    {
        var result = await _sut.InstallAsync(cancellationToken);

        await Assert.That(result.Succeeded).IsTrue();
    }
}
