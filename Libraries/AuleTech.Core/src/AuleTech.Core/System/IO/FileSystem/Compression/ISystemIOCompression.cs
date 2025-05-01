using System.IO.Compression;

namespace AuleTech.Core.System.IO.FileSystem.Compression;

public interface ISystemIOCompression
{
    ZipArchive OpenRead(string archiveFileName);

    Task<Stream> GetZippedFileAsync(Stream zippedStream
        , string fileName);

    ZipArchive OpenCreate(string archiveFileName);


    void ExtractToDirectory(string sourceArchiveFileName
        , string destinationDirectoryName
        , bool overwrite = true
        , int maxFilesAllowed = 200
        , int maxSizeBytesAllowed = 209715200);


    void ExtractToDirectory(Stream zippedStream
        , string destinationDirectoryName
        , bool overwrite = true
        , int maxFilesAllowed = 200
        , int maxSizeBytesAllowed = 209715200);

    Task AddToZipAsync(Stream zippedStream
        , Stream sourceContentStream
        , string nameWithPath
        , bool doNotCloseArchiveStream = false
        , bool doNotCloseSourceContentStream = false);


    bool TryValidateItCanBeExtracted(Stream zippedStream);

    Task AddDirectoryToZipAsync(Stream zippedStream
        , string directoryPath);

    Task AddDirectoryToZipAsync(string zipFilePath
        , params string[] directoryToAddPath);

    Task AddFileToZipAsync(Stream zippedStream
        , string filePath
        , string entryFilePath);

    IEnumerable<PlatformZipFileEntry> GetZipEntries(Stream archiveStream
        , int maxFilesAllowed = 200
        , int maxSizeBytesAllowed = 209715200);

    void GetZipEntriesValidated(string filePath
        , int maxFilesAllowed = 200
        , int maxSizeBytesAllowed = 209715200);
}
