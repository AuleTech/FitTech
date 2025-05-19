using AuleTech.Core.System.IO.Configuration;
using AuleTech.Core.System.IO.FileSystem.Compression;
using AuleTech.Core.System.IO.FileSystem.Directories;
using AuleTech.Core.System.IO.FileSystem.Files;

namespace AuleTech.Core.System.IO.FileSystem;

internal class SystemIoProxy : ISystemIo
{
    private readonly string _storageLocation;

    public SystemIoProxy(ISystemIOSettings? systemIoSettings = null) : this(new SystemIoDirectoryProxy()
        , new SystemIoPathProxy()
        , new SystemIoFileProxy(false)
        , new SystemIoFileProxy(true)
        , systemIoSettings)
    {
    }

    public SystemIoProxy(ISystemIoDirectory systemIoDirectory
        , ISystemIoPath systemIoPath
        , ISystemIoFile systemIoFile
        , ISystemIoFileDirect systemIoFileDirect
        , ISystemIOSettings? systemIoSettings = null)
    {
        File = systemIoFile ?? throw new ArgumentNullException(nameof(systemIoFile));
        Path = systemIoPath ?? throw new ArgumentNullException(nameof(systemIoPath));
        _storageLocation = systemIoSettings?.StorageLocation
                           ?? systemIoPath.Combine(systemIoPath.GetTempPath(), "Platform");
        Directory = systemIoDirectory ?? throw new ArgumentNullException(nameof(systemIoDirectory));
        FileDirect = systemIoFileDirect ?? throw new ArgumentNullException(nameof(systemIoFileDirect));
        ZipCompression = new SystemIoCompressionProxy(this);
        Default = this;
    }

    public static SystemIoProxy Default { get; private set; } = new();

    public ISystemIoDirectory Directory { get; }
    public ISystemIoPath Path { get; }
    public ISystemIoFile File { get; }
    public ISystemIoFileDirect FileDirect { get; }
    public ISystemIOCompression ZipCompression { get; }

    public string GetStorageLocationDirPath()
    {
        return _storageLocation;
    }

    public DirectoryInfo GetStorageLocationDir()
    {
        return new DirectoryInfo(GetStorageLocationDirPath());
    }
}
