using AuleTech.Core.System.IO.FileSystem.Compression;
using AuleTech.Core.System.IO.FileSystem.Files;

namespace AuleTech.Core.System.IO.FileSystem;

public interface ISystemIo
{
    ISystemIoDirectory Directory { get; }
    ISystemIoPath Path { get; }
    ISystemIoFile File { get; }
    ISystemIoFileDirect FileDirect { get; }
    ISystemIOCompression ZipCompression { get; }

    string GetStorageLocationDirPath();
    DirectoryInfo GetStorageLocationDir();
}
