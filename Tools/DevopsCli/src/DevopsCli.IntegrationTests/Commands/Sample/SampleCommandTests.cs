using AuleTech.Core.Patterns.Result;
using DevopsCli.Core.Commands;
using DevopsCli.Core.Commands.Sample;

namespace DevopsCli.IntegrationTests.Commands.Sample;

[TestCliContainer]
public class SampleCommandTests
{
    private readonly ICommand<SampleCommandParams, Result> _sut;

    public SampleCommandTests(ICommand<SampleCommandParams, Result> sut)
    {
        _sut = sut;
    }

    [Test]
    [Timeout(3000)]
    public async Task SampleCommandRun(CancellationToken cancellationToken)
    {
        var result = await _sut.RunAsync(new SampleCommandParams { Param1 = "Test" }, cancellationToken);

        await Assert.That(result.Succeeded).IsTrue();
    }
}
