namespace AuleTech.Core.System.IO.FileSystem
{
	public interface ISystemIoPath
	{
		string ConvertToLinuxPath(string path);
		string GetTempPath();

		string GetTempPath(string relativeDirectoryPath);

		string GetTempPath(string relativeDirectoryPath
		                   , bool createIfNotExists);
		string GetTempPath(bool createIfNotExists,params string[] relativeDirectoryPathParts);
		string GetTempPath(string subDirectory
		                   , bool createDirectoryIfNotExists
		                   , bool deleteContentIfExists);
		string GetPlatformTempPath(string subDirectory, bool createIfNotExists=true, bool deleteContentIfExists=false);
		string GetUserPath();

		string GetUserPath(string relativeDirectoryPath);

		string GetUserPath(string relativeDirectoryPath
		                   , bool createIfNotExists);

		string Combine(string path1
		               , string path2
		               , bool createIfNotExists = false);
		string Combine(string[] parts
		               , bool createIfNotExists = false);

		string Combine(string path1
		               , string path2
		               , string path3
		               , bool createIfNotExists = false);

		string GetExtension(string fileName);

		string GetExtension(string fileName
		                    , bool includePeriod);

		string GetFileName(string fullFileName);

		string GetDirectoryName(string fullFileName);
		string GetFileNameWithoutExtension(string fullFileName);
		char[] GetInvalidFileNameChars();

		

		string GetRelativePath(string relativeTo
		                       , string path);

		string GetFullPath(string path);

		string GetFullPath(string relativePath
		                   , string fullBasePath);

		string GetFullDirectoryPath(string path);
		string NormalizePath(string path);

		string ResolveRelativePath(string sourcePath
		                           , string offsetTarget);

		string ResolveRelativePath(string sourceFileFullPath
		                           , string sourceRootFolderPath
		                           , string destinationRootFolderPath);

		string GetParentDirPath(string path);

		string GetParentDirPath(string path
		                        , string parentDirectoryName);

		string GetParentDirPathCommonToAll(string[] paths);

		bool IsFileOfType(string fullPath
		                  , params string[] fileExtensions);

		bool IsPathRooted(string path);

		bool HasExtension(string fileName
		                  , string extension);

		string ReplaceDirectorySeparatorChar(string path
		                                     , string newValue);

		char GetPathSeparator();
		char GetDirectorySeparator();
		char GetAltDirectorySeparator();
		string GetRandomFileName();

		string GetSpecialFolderPath(Environment.SpecialFolder specialFolder
			                            , string subDirectory
			                            , bool createIfNotExists);

		
	}
}
