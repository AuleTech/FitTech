namespace AuleTech.Core.System.IO.FileSystem;

internal class PlatformPathStringStandard
{
    private readonly string _path;

    // File I/O functions in the Windows API convert "/" to "\" as part of converting the name to an NT-style name, except when using the "\\?\" prefix
    // https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=powershell
    public PlatformPathStringStandard(string inputPath)
    {
        _path = !OperatingSystem.IsWindows()
            ? inputPath
            : ResolveStandardPath();

        return;

        string ResolveStandardPath()
        {
            var prefix = GetPrefix();
            var part = inputPath.TrimStart(prefix.ToCharArray());
            part = part
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (inputPath.StartsWith(prefix))
            {
                part = $"{prefix}{part}";
            }

            return part;
        }

        string GetPrefix()
        {
            var pathSplit = inputPath.Split(Path.VolumeSeparatorChar);

            switch (pathSplit.Length)
            {
                case 1:
                    return string.Empty;
                case 2:
                {
                    var s = pathSplit[0];
                    return $"{s.Substring(0, s.Length - 1)}:";
                }
                default:
                    throw new ArgumentException($"Value is not valid: '{inputPath}'.", nameof(inputPath));
            }
        }
    }


    public static implicit operator string(PlatformPathStringStandard standard)
    {
        return standard._path;
    }

    public static implicit operator PlatformPathStringStandard(string part)
    {
        return new PlatformPathStringStandard(part);
    }

    public override string ToString()
    {
        return _path;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is PlatformPathStringStandard standard && Equals(standard);
    }

    protected bool Equals(PlatformPathStringStandard other)
    {
        return _path == other._path;
    }

    public override int GetHashCode()
    {
        return _path.GetHashCode();
    }
}
