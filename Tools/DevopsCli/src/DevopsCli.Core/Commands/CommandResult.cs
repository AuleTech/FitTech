namespace DevopsCli.Core.Commands;

public class CommandResult<TOut> : CommandResult
{
    public TOut? Value { get; set; }

    public static CommandResult<TOut> Succeed(TOut value) => new CommandResult<TOut>()
    {
        Value = value, Code = CommandCode.Succeed
    };
}

public class CommandResult
{
    public CommandCode Code { get; set; }
    public string? ErrorMessage { get; set; }
    
    public static CommandResult Succeed() => new () { Code = CommandCode.Succeed };
    public static CommandResult Failed(string? errorMessage) => new ()
    {
        Code = CommandCode.Error,
        ErrorMessage = errorMessage
    };

    public static implicit operator int(CommandResult result) => (int)result.Code;
}

public enum CommandCode
{
    Succeed = 0,
    Error = 255
}
