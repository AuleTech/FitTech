namespace AuleTech.Core.System.IO;

public class FileStreamWithName : IDisposable
{
    private readonly Stream _stream;

    private readonly SemaphoreSlim _syncLock = new(1);

    public FileStreamWithName(
        string fileName
        , Stream stream
        , bool disposeStream = false
    )
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));
        }

        _stream = new MemoryStream();
        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        stream.CopyTo(_stream);
        if (disposeStream)
        {
            stream.Dispose();
        }

        FileName = fileName;
    }

    public string FileName { get; }

    public void Dispose()
    {
        _stream?.Dispose();
    }

    public async Task<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        var result = new MemoryStream();

        await _syncLock.WaitAsync(cancellationToken);
        try
        {
            _stream.Seek(0, SeekOrigin.Begin);
            await _stream.CopyToAsync(result);
        }
        finally
        {
            _syncLock.Release(1);
        }

        result.Seek(0, SeekOrigin.Begin);
        return result;
    }
}
