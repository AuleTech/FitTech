using System.Text;

namespace AuleTech.Core.System.IO.FileSystem.Files;

internal partial class SystemIoFileProxy
{
    private const int DefaultBufferSize = 4096;
    public static readonly UTF8Encoding DefaultUtf8Encoding = new(false, true);

    private readonly bool _bypassCache;

    private StreamReader AsyncStreamReader(string path)
    {
        return AsyncStreamReader(path, DefaultUtf8Encoding);
    }

    private StreamReader AsyncStreamReader(string path
        , Encoding encoding)
    {
        var stream = AsyncReadFileStream(path);

        return new StreamReader(stream, encoding, true);
    }


    private StreamWriter AsyncStreamWriter(string path
        , bool append)
    {
        return AsyncStreamWriter(path, DefaultUtf8Encoding, append);
    }

    private StreamWriter AsyncStreamWriter(string path
        , Encoding encoding
        , bool append)
    {
        var stream = AsyncWriteFileStream(path, append);

        return new StreamWriter(stream, encoding);
    }

    private FileStream AsyncWriteFileStream(string path
        , bool append)
    {
        if (!append)
        {
            var io = SystemIoProxy.Default;
            var directory = io.Path.GetDirectoryName(path);
            io.Directory.CreateDirectory(directory, false, false);
        }

        var fileOptions = _bypassCache
            ? FileOptions.Asynchronous | FileOptions.WriteThrough
            : FileOptions.Asynchronous | FileOptions.SequentialScan;
        var stream = new FileStream(
            path
            , append ? FileMode.Append : FileMode.Create
            , FileAccess.ReadWrite
            , FileShare.ReadWrite
            , DefaultBufferSize
            , fileOptions);
        return stream;
    }

    private FileStream AsyncReadFileStream(string path)
    {
        var fileOptions = _bypassCache
            ? FileOptions.Asynchronous | FileOptions.SequentialScan
            : FileOptions.None;
        var stream = new FileStream(
            path
            , FileMode.Open
            , FileAccess.Read
            , FileShare.ReadWrite
            , DefaultBufferSize
            , fileOptions);
        return stream;
    }

    private static void ThrowIfInvalidFilePathOrEncoding(string path
        , Encoding encoding)
    {
        ThrowIfInvalidFilePath(path);

        if (encoding == null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }
    }

    private static void ThrowIfInvalidFilePath(string? path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path), "Value cannot be null.");
        }

        if (path.Length == 0)
        {
            throw new ArgumentException("Value cannot be empty.", nameof(path));
        }
    }
}
