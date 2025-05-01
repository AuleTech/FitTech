using System.IO.Compression;

namespace AuleTech.Core.System.IO.FileSystem.Compression;

public class PlatformZipFileEntry
{
    private readonly ZipArchiveEntry _decorated;
    private readonly ISystemIo _io;

    private readonly Lazy<Stream> _streamLazy;

    public PlatformZipFileEntry(ZipArchiveEntry archiveEntry
        , ISystemIo io)
    {
        _decorated = archiveEntry ?? throw new ArgumentNullException(nameof(archiveEntry));
        _io = io ?? throw new ArgumentNullException(nameof(io));
        if (string.IsNullOrWhiteSpace(_decorated.Name))
        {
            throw new ArgumentException("Invalid entry name(empty) in zip file");
        }

        _streamLazy = new Lazy<Stream>(() => _decorated.Open());
    }

    public Stream Stream => _streamLazy.Value;
    public string FileName => _decorated.Name;

    public async Task<string> ExtractAsync(string destinationPathDir
        , bool overwrite
        , CancellationToken cancellationToken = default)
    {
        var completeFileName = Extract(destinationPathDir, overwrite);

        using var sourceStream = _decorated.Open();

        await _io.File.CopyAsync(sourceStream
            , completeFileName
            , overwrite
            , cancellationToken);

        return completeFileName;
    }

    public async Task<string> ExtractDirectAsync(string destinationPathDir
        , bool overwrite
        , CancellationToken cancellationToken = default)
    {
        var completeFileName = Extract(destinationPathDir, overwrite);

        using var sourceStream = _decorated.Open();
        await _io.FileDirect.CopyAsync(sourceStream
            , completeFileName
            , overwrite
            , cancellationToken);

        return completeFileName;
    }

    public bool HasExtensionOneOf(params string[] extensions)
    {
        var extension = GetExtension();
        return extensions.Any(x => x.Equals(extension, StringComparison.InvariantCultureIgnoreCase));
    }

    private string GetExtension()
    {
        return _io.Path.GetExtension(_decorated.Name, false);
    }

    private string Extract(string destinationPathDir
        , bool overwrite)
    {
        var completeFileName = _io.Path.Combine(destinationPathDir, _decorated.FullName);
        if (!completeFileName.StartsWith(destinationPathDir))
        {
            throw new ArgumentException(
                $"The file contains potentially malicious entries. entry('{_decorated.FullName}')");
        }

        var directory = _io.Path.GetDirectoryName(completeFileName);
        _io.Directory.CreateDirectory(directory, false, false);

        return completeFileName;
    }
}
