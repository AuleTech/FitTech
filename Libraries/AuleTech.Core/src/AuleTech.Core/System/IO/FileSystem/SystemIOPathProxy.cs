using System.Text.RegularExpressions;

namespace AuleTech.Core.System.IO.FileSystem
{
	internal class SystemIoPathProxy : ISystemIoPath
	{
		public string ConvertToLinuxPath(string windowsPath)
		{
			if (string.IsNullOrWhiteSpace(windowsPath))
				return windowsPath;

			var linuxPath = windowsPath.Replace('\\', '/');

			var driveRegex = new Regex(@"^[a-zA-Z]:");
			if (driveRegex.IsMatch(linuxPath))
			{
				linuxPath = driveRegex.Replace(linuxPath, m => $"/{m.Value[0].ToString().ToLower()}");
			}

			return linuxPath;
		}

		public string GetTempPath() => Path.GetTempPath();
		
		public string GetTempPath(string relativeDirectoryPath) => GetTempPath(relativeDirectoryPath, false);

		public string GetTempPath(string relativeDirectoryPath
		                          , bool createIfNotExists) =>
			Combine(GetTempPath(), relativeDirectoryPath, createIfNotExists);

		public string GetTempPath(bool createIfNotExists
		                          , params string[] relativeDirectoryPathParts)
        {
            var parts = relativeDirectoryPathParts.Append(GetTempPath()).ToArray();
			return Combine(parts, createIfNotExists);
		}

		public string GetTempPath(string subDirectory
		                          , bool createDirectoryIfNotExists
		                          , bool deleteContentIfExists)
		{
			var path = Combine(GetTempPath(), subDirectory);
			if (deleteContentIfExists && Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}

			return GetTempPath(subDirectory, createDirectoryIfNotExists);
		}

		public string GetPlatformTempPath(string subDirectory
		                                  , bool createIfNotExists=true
			, bool deleteContentIfExists=false) =>
			GetTempPath(Combine(".platform",subDirectory),createIfNotExists,deleteContentIfExists);

		public string GetUserPath() => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		public string GetUserPath(string relativeDirectoryPath) => GetUserPath(relativeDirectoryPath, false);

		public string GetUserPath(string relativeDirectoryPath
		                          , bool createIfNotExists) =>
			Combine(GetUserPath(), relativeDirectoryPath, createIfNotExists);

		public string Combine(string path1
		                      , string path2
		                      , bool createIfNotExists = false)=> Combine(new []{path1, path2}, createIfNotExists);

		public string Combine(string[] parts
		                      , bool createIfNotExists = false)
		{
			var result= (string)(PlatformPathStringStandard)Path.Combine(parts!);
			if (createIfNotExists && !Directory.Exists(result))
			{
				Directory.CreateDirectory(result);
			}
			return result;
		}

		public string Combine(string path1
		                      , string path2
		                      , string path3
		                      , bool createIfNotExists = false) => Combine(new[] { path1, path2,path3 }, createIfNotExists);

		public string GetExtension(string fileName) => GetExtension(fileName, true);

		public string GetExtension(string fileName
		                           , bool includePeriod)
		{
            ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
            
			var result = Path.GetExtension(fileName);
			if (!includePeriod)
			{
				result = result!.TrimStart('.');
			}

			return result!;
		}

		public string GetFileName(string fullFileName) => Path.GetFileName(fullFileName);
		public string GetDirectoryName(string fullFileName) => Path.GetDirectoryName(fullFileName)!;

		public string GetFileNameWithoutExtension(string fullFileName) =>
			Path.GetFileNameWithoutExtension(fullFileName);

		public char[] GetInvalidFileNameChars() => Path.GetInvalidFileNameChars();

		

		public string GetRelativePath(string relativeTo
		                              , string? path)
		{
			if (string.IsNullOrEmpty(relativeTo))
			{
				throw new ArgumentNullException(nameof(relativeTo));
			}

			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException(nameof(path));
			}

			if (relativeTo.StartsWith(".") || path.StartsWith("."))
			{
				throw new NotSupportedException("Only absolute paths supported");
			}

			if (!relativeTo.EndsWith(Path.DirectorySeparatorChar.ToString())
			 && !relativeTo.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
			{
				relativeTo += Path.DirectorySeparatorChar;
			}


			var fromUri = new Uri(relativeTo);
			var toUri = new Uri(path);

			if (fromUri.Scheme != toUri.Scheme)
			{
				return path;
			}

			var relativeUri = fromUri.MakeRelativeUri(toUri);
			var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
			{
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			return relativePath;
		}

		public string GetFullPath(string path) => Path.GetFullPath(path);

		public string GetFullPath(string relativePath
		                          , string fullBasePath) =>
			GetFullPath(Combine(fullBasePath, relativePath));

		public string GetFullDirectoryPath(string path) => Path.GetFullPath(GetDirectoryName(path));

		public string NormalizePath(string path) =>
			(PlatformPathStringStandard)Path.GetFullPath(new Uri(GetFullPath(path)).LocalPath)
				.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

		public string ResolveRelativePath(string sourcePath
		                                  , string offsetTarget)
		{
			if (!IsFile(sourcePath) && !sourcePath.EndsWith(Path.DirectorySeparatorChar.ToString()) && !sourcePath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
			{
				sourcePath += Path.DirectorySeparatorChar.ToString();
			}

			var relativeUri = new Uri(offsetTarget);
			var sourceUri = new Uri(sourcePath);
			var relativePath = sourceUri.MakeRelativeUri(relativeUri)
				.ToString();
			return relativePath;
		}

		public string ResolveRelativePath(string sourceFileFullPath
		                                  , string sourceRootFolderPath
		                                  , string destinationRootFolderPath)
		{
			if (string.IsNullOrWhiteSpace(sourceFileFullPath))
			{
				throw new ArgumentNullException(nameof(sourceFileFullPath));
			}

			if (string.IsNullOrWhiteSpace(sourceRootFolderPath))
			{
				throw new ArgumentNullException(nameof(sourceRootFolderPath));
			}

			if (string.IsNullOrWhiteSpace(destinationRootFolderPath))
			{
				throw new ArgumentNullException(nameof(destinationRootFolderPath));
			}

			sourceFileFullPath = NormalizePath(sourceFileFullPath);
			sourceRootFolderPath = NormalizePath(sourceRootFolderPath);
			destinationRootFolderPath = NormalizePath(destinationRootFolderPath);

			if (!sourceFileFullPath.StartsWith(sourceRootFolderPath))
			{
				throw new InvalidOperationException($"{sourceRootFolderPath} is not root of {sourceFileFullPath}");
			}

			return GetFullPath(Combine(destinationRootFolderPath
				, ResolveRelativePath(sourceRootFolderPath, sourceFileFullPath)));
		}

		public string GetParentDirPath(string path)
		{
			string result;
			try
			{
				var isDirectory = (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
				var directoryInfo = isDirectory
					? new DirectoryInfo(path).Parent
					: new FileInfo(path).Directory;

				result = directoryInfo!.FullName;
			}
			catch (DirectoryNotFoundException)
			{
				var parts = path.Replace('\\', '/')
					.Split('/');
				result = string.Join("\\"
					, parts.Take(parts.Length - 1));
			}

			return result;
		}

		public string GetParentDirPath(string path
		                               , string parentDirectoryName)
		{
			while (parentDirectoryName != GetFileName(path))
			{
				path = GetParentDirPath(path);
			}

			return path;
		}

		public string GetParentDirPathCommonToAll(string[] paths)
		{
			var splitPaths = paths.Select(p =>
					p.Replace('\\', '/')
						.Split('/')
				)
				.ToArray();
			var minLength = splitPaths.Min(p => p.Length);
			var commonPath = "";

			for (var i = 0; i < minLength; i++)
			{
				var current = splitPaths[0][i];
				if (splitPaths.All(p => p[i] == current))
				{
					commonPath += current + Path.DirectorySeparatorChar;
				}
				else
				{
					break;
				}
			}

			return commonPath.TrimEnd('\\', '/');

		}

		public bool IsFileOfType(string fullPath
		                         , params string[] fileExtensions)
		{
			var last = fullPath.Replace('\\', '/')
				.Split('/')
				.Last();

			return fileExtensions.Any(_ => last.EndsWith($".{_}", StringComparison.InvariantCultureIgnoreCase));
		}

		public bool IsPathRooted(string path) => Path.IsPathRooted(path);

		public bool HasExtension(string fileName
		                         , string extension) => GetExtension(fileName, false)
			.Equals(extension, StringComparison.InvariantCultureIgnoreCase);

		public string ReplaceDirectorySeparatorChar(string path
		                                            , string newValue) =>
			path.Replace(Path.DirectorySeparatorChar.ToString(), newValue);

		public char GetPathSeparator() => Path.PathSeparator;

		public char GetDirectorySeparator() => Path.DirectorySeparatorChar;
		public char GetAltDirectorySeparator() => Path.AltDirectorySeparatorChar;
		public string GetRandomFileName() => Path.GetRandomFileName();

		public string GetSpecialFolderPath(Environment.SpecialFolder specialFolder
		                                   , string subDirectory
		                                   , bool createIfNotExists) =>
			Combine(Environment.GetFolderPath(specialFolder), subDirectory, createIfNotExists);

		public bool IsDirectory(string path) => (File.GetAttributes(path) & FileAttributes.Directory)
                                                 == FileAttributes.Directory;

		public bool IsFile(string path) => !IsDirectory(path);
	}
}
