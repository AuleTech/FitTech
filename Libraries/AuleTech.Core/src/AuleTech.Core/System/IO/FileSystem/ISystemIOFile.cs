using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using AuleTech.Core.System.IO.FileSystem.PlatformModels;

namespace AuleTech.Core.System.IO.FileSystem;

public interface ISystemIoFile
{
    Task WriteAllBytesAsync(string path
        , byte[] bytes
        , CancellationToken cancellationToken = default);

    /// <summary>
    ///     Reads all text from a file.
    /// </summary>
    /// <param name="path">the file to read from</param>
    /// <returns>the text contained in the file</returns>
    string ReadAllText(string path);

    string ReadAllText(string path
        , Encoding encoding);

    /// <summary>
    ///     Reads all text from a file.
    /// </summary>
    /// <param name="path">the file to read from</param>
    /// <param name="cancellationToken"></param>
    /// <returns>the text contained in the file</returns>
    Task<string> ReadAllTextAsync(string path
        , CancellationToken cancellationToken = default);


    Task<string> ReadAllTextAsync(string path
        , Encoding encoding
        , CancellationToken cancellationToken = default);

    Task<byte[]> ReadAllBytesAsync(string path
        , CancellationToken cancellationToken = default);

    Task DeleteAsync(string path
        , bool throwIfNotExists = true
        , CancellationToken cancellationToken = default);

    Task ExtractZipArchiveAsync(string archivePath
        , string targetFolder
        , bool overwriteOutputIfExists = false
        , CancellationToken cancellationToken = default);

    void CreateZipFromDirectory(string path
        , string outputFilePath);

    /// <summary>
    ///     Copies an existing file to a new file, and overwrites if new file exists
    /// </summary>
    /// <param name="sourceFile">The file name of source file.</param>
    /// <param name="destinationFile">The file name of destination file.</param>
    /// <param name="overwrite">If set to true then overwrite destination file if already exists.</param>
    void Copy(string sourceFile
        , string destinationFile
        , bool overwrite);

    Task CopyAsync(string sourceFile
        , string destinationFile
        , bool overWrite
        , CancellationToken cancellationToken = default);

    Task MoveAsync(string sourceFile
        , string destinationFile
        , CancellationToken cancellationToken = default);

    Task MoveAsync(string sourceFile
        , string destinationFile
        , bool overWrite = false
        , CancellationToken cancellationToken = default);


    Task CopyAsync(Stream sourceStream
        , string destinationFile
        , bool overWrite
        , CancellationToken cancellationToken = default);

    bool Exists(string fileName
        , CancellationToken cancellationToken = default);

    Task WriteStreamAsync(string destinationFile
        , Stream stream
        , FileMode fileMode = FileMode.Create
        , CancellationToken cancellationToken = default);

    FileAttributes GetAttributes(string fileOrFolderPath);

    /// <summary>
    ///     Opens a file and returns a stream
    /// </summary>
    /// <param name="path">the path of the file to be read</param>
    /// <returns>stream of the file</returns>
    Stream OpenRead(string path);

    /// <summary>
    ///     Creates a file
    /// </summary>
    /// <param name="path">path of the file created</param>
    /// <returns>The file stream of the file created</returns>
    Stream Create(string path);

    string GetFileName(string path);

    Stream Open(string path
        , FileMode mode);

    Stream Open(string path
        , FileMode mode
        , FileAccess access
        , FileShare share);

    /// <summary>
    ///     Creates and image object from a file
    /// </summary>
    /// <param name="path">File Path</param>
    /// <returns>Image</returns>
    PlatformImage ImageFromFile(string path);


    /// <summary>
    ///     Saves the given bitmap
    /// </summary>
    /// <param name="bitmap">Bitmap object</param>
    /// <param name="path">Image path to be save</param>
    /// <param name="format">Image format</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SaveBitMapAsync(Bitmap bitmap
        , string path
        , ImageFormat format
        , CancellationToken cancellationToken = default);

    DateTime GetCreationTimeUtc(string filePath);

    string GetFileExtension(string input
        , bool includeDot = true);

    long GetFileSize(string filePath);
    Stream OpenWrite(string path);

    Task<string[]> ReadAllLinesAsync(string filePath
        , CancellationToken cancellationToken = default);

    Task<string[]> ReadAllLinesAsync(string path
        , Encoding encoding
        , CancellationToken cancellationToken = default);

    Task WriteAllLinesAsync(string path
        , IEnumerable<string> contents
        , CancellationToken token = default);

    Task WriteAllLinesAsync(TextWriter writer
        , IEnumerable<string> contents
        , CancellationToken cancellationToken = default);

    Task<IEnumerable<Process>> GetProcessesLockingFileAsync(string filePath
        , CancellationToken cancellationToken);

    Task WaitForFileReleaseAsync(string filePath
        , TimeSpan waitTime
        , bool killProcessIfNotReleased);

    Task WriteAllTextAsync(string path
        , string content
        , CancellationToken cancellationToken = default);

    Task WriteAllTextAsync(string path
        , string contents
        , Encoding encoding
        , CancellationToken cancellationToken = default);

    Task WriteAllTextAsync(StreamWriter sw
        , string contents
        , CancellationToken cancellationToken = default);

    void WriteAllText(string path
        , string content);

    Task<string> CopyToTempAsync(string filePath
        , CancellationToken cancellationToken = default);

    Task<string> CopyToTempAsync(Stream stream
        , string? usingFileName = null
        , CancellationToken cancellationToken = default);

    bool IsLocked(string filePath);

    bool IsLocked(string filePath
        , TimeSpan waitTimeout);

    DateTime GetLastWriteTime(string path);

    Encoding GetUtf8Encoding(string path);

    Encoding GetUtf8Encoding(byte[] bytes);

    Task ReplaceTokensAsync(string path
        , IReadOnlyDictionary<string, string> tokensToReplace
        , CancellationToken cancellationToken);

    Task ReplaceTokenAsync(string path
        , string key
        , string value
        , CancellationToken cancellationToken);

    void AppendAllText(string path
        , string content);
}
