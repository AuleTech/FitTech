using System.IO.Compression;
using AuleTech.Core.Resiliency;

namespace AuleTech.Core.System.IO.FileSystem.Directories;

internal partial class SystemIoDirectoryProxy : ISystemIoDirectory
{

	public bool Exists(string folder) => Directory.Exists((PlatformPathStringStandard)folder);

	public string CreateDirectory(string folder
	                              , bool deleteIfExists = false
	                              , bool throwIfExists = true)
	{
		folder = (PlatformPathStringStandard)folder;
		if (deleteIfExists)
		{
			Delete(folder, true, false);
		}

		if (throwIfExists || !Exists(folder))
		{
			Directory.CreateDirectory(folder);
		}

		return folder;
	}

	public IEnumerable<string> GetFiles(string path) => Directory.GetFiles(path);

	public IEnumerable<string> GetFiles(string path
	                                    , string searchPattern) => Directory.GetFiles(path, searchPattern);

	/// <param name="searchPattern">separate various with |</param>
	public IEnumerable<string?> GetFiles(string path
	                                    , string searchPattern
	                                    , SearchOption searchOption)
	{
		var result = new List<string?>();
		foreach (var s in searchPattern.Split('|'))
		{
			result.AddRange(Directory.GetFiles(path, s, searchOption));
		}

		return result;
	}

	public IEnumerable<string> GetFiles(string path
	                                    , params string[] searchPatterns) =>
		searchPatterns.SelectMany(pattern => GetFiles(path, pattern))
			.Distinct()
			.ToArray();

	public string?[] GetFiles(string path
        , string[] searchPatterns
        , SearchOption searchOption) =>
		searchPatterns.SelectMany(pattern => GetFiles(path, pattern, searchOption))
			.Distinct()
			.ToArray();

	public void CreateZipFromFolder(string sourceFolder
	                                , string filePath
	                                , bool overrideIfExists)
	{
		filePath = (PlatformPathStringStandard)filePath;
		if (overrideIfExists && File.Exists(filePath))
		{
			File.Delete(filePath);
		}

		ZipFile.CreateFromDirectory(sourceFolder, filePath);
	}

	public void Delete(string folder
	                   , bool recursive
	                   , bool throwIfNotExists = true) =>
		DeleteAsync(folder, recursive, CancellationToken.None, throwIfNotExists)
			.Wait();

	public async Task DeleteAsync(string folder
	                              , bool recursive
	                              , CancellationToken cancellationToken
	                              , bool throwIfNotExists = true
	                              , bool forceUnlock = false)
	{
		folder = (PlatformPathStringStandard)folder;
		Console.WriteLine(() => $"Delete folder {folder} recursive:{recursive} throwIfNotExists:{throwIfNotExists}");
		var exists = Directory.Exists(folder);
		if (throwIfNotExists && !exists)
		{
			throw new DirectoryNotFoundException(folder);
		}

		if (exists)
		{
			await ResilientOperations.Default.RetryIfNeededAsync(async _ =>
				{
					try
					{
						if (Directory.Exists(folder))
						{
							Directory.Delete(folder, recursive);
						}
					}
					catch (UnauthorizedAccessException)
					{
						SetDirectoryAndFilesAttributes(folder, FileAttributes.Normal);
						throw;
					}
					catch (IOException)
					{
						if (forceUnlock)
						{
							await DirectoryUnlocker.Instance.UnlockFromLocalProcessesAsync(folder, cancellationToken);
						}

						throw;
					}
				}
				, TimeSpan.FromSeconds(120) //Network File system could be under pressure
				, TimeSpan.FromMilliseconds(100)
				, cancellationToken: cancellationToken
			);
		}
	}

	public void SetDirectoryAndFilesAttributes(string folder
	                                           , FileAttributes fileAttributes
	                                           , bool recursive = true)
	{
		folder = (PlatformPathStringStandard)folder;
		var directoryInfo = new DirectoryInfo(folder);
		directoryInfo.Attributes &= ~fileAttributes;

		foreach (var file in directoryInfo.GetFiles())
		{
			file.Attributes =fileAttributes;
		}

		if (recursive)
		{
			foreach (var subDir in directoryInfo.GetDirectories())
			{
				SetDirectoryAndFilesAttributes(subDir.FullName, fileAttributes);
			}
		}
	}

	public string GetCurrentDirectory() => Directory.GetCurrentDirectory();

	public void Clone(string sourceDir
	                  , string destinationDir)
	{
		sourceDir = (PlatformPathStringStandard)sourceDir;
		destinationDir = (PlatformPathStringStandard)destinationDir;
		SystemIoProxy.Default.Directory.CreateDirectory(destinationDir, false, false);

		CopyFilesRecursively(new DirectoryInfo(sourceDir), new DirectoryInfo(destinationDir));

		void CopyFilesRecursively(DirectoryInfo source
		                          , DirectoryInfo target)
		{
			foreach (var dir in source.GetDirectories())
			{
				CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
			}

			foreach (var file in source.GetFiles())
			{
				file.CopyTo(Path.Combine(target.FullName, file.Name));
			}
		}
	}

	public string CloneToTemp(string sourceDir)
	{
		var path = SystemIoProxy.Default.Path;
		var tempDirectory = $"{path.GetTempPath()}/{Guid.NewGuid().ToString().Replace("-", string.Empty)}/"
			.Trim()
			.TrimEnd('.');
		Clone(sourceDir, tempDirectory);
		return tempDirectory;
	}

	public DirectoryInfo GetParent(string path) => Directory.GetParent(path)!;

	public string AsStringDescription(string folder)
	{
		var lines = new List<string>();
		TraverseDir(new DirectoryInfo(folder), string.Empty);
		return string.Join(Environment.NewLine, lines);

		void TraverseDir(DirectoryInfo dir
		                 , string spaces)
		{
			lines.Add($"{spaces}{dir.Name}:");
			lines.AddRange(dir.GetFiles()
				.Select(f => $"{spaces}--{f.Name}"));
			var children = dir.GetDirectories();
			foreach (var child in children)
			{
				TraverseDir(child, $"{spaces} ");
			}
		}
	}

	public string[] GetDirectories(string folderPath) => Directory.GetDirectories(folderPath);

	public string[] GetDirectories(string folderPath
	                               , SearchOption searchOption) => GetDirectories(folderPath, "*", searchOption);

	public string[] GetDirectories(string folderPath
	                               , string searchPattern
	                               , SearchOption searchOption = SearchOption.AllDirectories)
		=> Directory.GetDirectories(folderPath, searchPattern, searchOption);

	public async Task CopyDirectoryContentAsync(string source
	                                            , string destination)
	{
		var files = GetFiles(source, "*", SearchOption.AllDirectories);

		var tasks = files.Select(async file =>
		{
			var sourceFileRelativePath = SystemIoProxy.Default.Path.GetRelativePath(source, file!);
			var destinationFilePath = (string)(PlatformPathStringStandard)Path.Combine(destination, sourceFileRelativePath);
			var destinationDirectory = (string)(PlatformPathStringStandard)Path.GetDirectoryName(destinationFilePath)!;
			if (!Directory.Exists(destinationDirectory))
			{
				Directory.CreateDirectory(destinationDirectory!);
			}

			await SystemIoProxy.Default.File.CopyAsync(file!, destinationFilePath, true);
		});

		await Task.WhenAll(tasks);
	}

	public string GetDirectoryRoot(string path) => Directory.GetDirectoryRoot(path);
	public DateTime GetCreationTimeUtc(string path) => Directory.GetCreationTimeUtc(path);

	public void SetCurrentDirectory(string folderPath)
	{
		Directory.SetCurrentDirectory(folderPath);
	}
}
