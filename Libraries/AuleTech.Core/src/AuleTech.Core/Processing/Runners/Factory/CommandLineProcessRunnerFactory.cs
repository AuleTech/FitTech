using Microsoft.Extensions.Logging;

namespace AuleTech.Core.Processing.Runners.Factory
{
	internal class CommandLineProcessRunnerFactory: IProcessRunnerFactory
    {
        private readonly ILogger<CommandLineProcessRunner> _logger;

        public CommandLineProcessRunnerFactory(ILogger<CommandLineProcessRunner> logger)
        {
            _logger = logger;
        }

        public IProcessRunner GetOne() => new CommandLineProcessRunner(_logger);
	}
}
