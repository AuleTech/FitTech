namespace AuleTech.Core.Extensions.Language;

public static class StringExtensions
{
    extension(string? value)
    {
        public bool IsEmpty => string.IsNullOrWhiteSpace(value);
    }
}
