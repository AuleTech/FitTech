using DevopsCli.Core.Commands;
using DevopsCli.Core.Commands.Sample;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DevopsCli.UnitTests.Commands.Sample;

public class SampleCommandTests
{
    private readonly ILogger<SampleCommand> _logger = Substitute.For<ILogger<SampleCommand>>();
    private SampleCommand _sut => new(_logger);


    [Test]
    [Timeout(3000)]
    public async Task SampleCommand_Works(CancellationToken cancellation)
    {
        var result = await _sut.RunAsync(new SampleCommandParams { Param1 = "test" }, cancellation);
        await Assert.That(result.Code).IsEqualTo(CommandCode.Succeed);
    }

    [Test]
    [Timeout(3000)]
    public async Task SampleCommand_FailsWhenParam1IsNull(CancellationToken cancellation)
    {
        var result = await _sut.RunAsync(new SampleCommandParams(), cancellation);
        await Assert.That(result.Code).IsEqualTo(CommandCode.Error);
    }
}
