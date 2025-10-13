namespace AuleTech.Core.System.IO.FileSystem;

public interface ISystemIoDirectory
{
    bool Exists(string folder);

    string CreateDirectory(string folder
        , bool deleteIfExists = false
        , bool throwIfExists = true);

    IEnumerable<string> GetFiles(string path);

    IEnumerable<string> GetFiles(string path
        , string searchPattern);

    IEnumerable<string?> GetFiles(string path
        , string searchPattern
        , SearchOption searchOption);

    IEnumerable<string> GetFiles(string path
        , params string[] searchPatterns);

    string?[] GetFiles(string path
        , string[] searchPatterns
        , SearchOption searchOption);

    void CreateZipFromFolder(string sourceFolder
        , string filePath
        , bool overrideIfExists = true);

    void Delete(string folder
        , bool recursive
        , bool throwIfNotExists = true);

    Task DeleteAsync(string folder
        , bool recursive
        , CancellationToken cancellationToken
        , bool throwIfNotExists = true
        , bool forceUnlock = false
    );

    void Clone(string sourceDir
        , string destinationDir);

    string CloneToTemp(string sourceDir);

    DirectoryInfo GetParent(string path);

    string AsStringDescription(string folder);
    string[] GetDirectories(string folderPath);

    string[] GetDirectories(string folderPath
        , SearchOption searchOption);

    string[] GetDirectories(string folderPath
        , string searchPattern
        , SearchOption searchOption = SearchOption.AllDirectories);

    Task CopyDirectoryContentAsync(string source
        , string destination);

    void SetDirectoryAndFilesAttributes(string folder
        , FileAttributes fileAttributes
        , bool recursive = true);

    string GetCurrentDirectory();

    string GetDirectoryRoot(string path);
    DateTime GetCreationTimeUtc(string path);
    void SetCurrentDirectory(string folderPath);
}
