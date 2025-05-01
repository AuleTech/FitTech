using System.IO.Compression;
using System.Text;
using AuleTech.Core.Resiliency;
using ICSharpCode.SharpZipLib.Tar;

namespace AuleTech.Core.System.IO.FileSystem.Compression
{
	internal class SystemIoCompressionProxy : ISystemIOCompression
	{
		private readonly ISystemIo _io;

		public SystemIoCompressionProxy(ISystemIo io)
		{
			_io = io ?? throw new ArgumentNullException(nameof(io));
		}

		public ZipArchive OpenRead(string archiveFileName) => ZipFile.OpenRead(archiveFileName);

		public async Task<Stream> GetZippedFileAsync(Stream zippedStream
		                                             , string fileName)
		{
			using var zipArchive = new ZipArchive(zippedStream);
			foreach (var entry in zipArchive.Entries)
			{
				if (entry.FullName.Equals(fileName))
				{
					var temp = _io.Path.Combine(_io.Path.GetTempPath("zipped_file_extractions", true, false)
						, Guid.NewGuid()
							.ToString());
					entry.ExtractToFile(temp);
					var ms = new MemoryStream();
					using (var openRead = _io.File.OpenRead(temp))
					{
						await openRead.CopyToAsync(ms);
						ms.Position = 0;
					}

					await _io.File.DeleteAsync(temp);
					return ms;
				}
			}

			throw new FileNotFoundException();
		}


		public ZipArchive OpenCreate(string archiveFileName) => ZipFile.Open(archiveFileName, ZipArchiveMode.Create);


		public void ExtractToDirectory(string sourceArchiveFileName
		                               , string destinationDirectoryName
		                               , bool overwrite = true
		                               , int maxFilesAllowed = 200
		                               , int maxSizeBytesAllowed = 209715200) //200MB
		{
			if (sourceArchiveFileName.EndsWith(".tar.gz", StringComparison.InvariantCultureIgnoreCase))
			{
				ExtractTarGz(sourceArchiveFileName, destinationDirectoryName);
			}
			else
			{
				ExtractZip(sourceArchiveFileName
					, destinationDirectoryName
					, maxFilesAllowed
					, maxSizeBytesAllowed
					, overwrite);
			}
		}

		public void ExtractToDirectory(Stream zippedStream
		                               , string destinationDirectoryName
		                               , bool overwrite = true
		                               , int maxFilesAllowed = 200
		                               , int maxSizeBytesAllowed = 209715200)
		{
			_io.Directory.CreateDirectory(destinationDirectoryName,false,false);
			using (var zip = new ZipArchive(zippedStream, ZipArchiveMode.Read, false))
			{
				zip.ExtractToDirectory(destinationDirectoryName);
			}
		}

		public async Task AddToZipAsync(Stream zippedStream
		                                , Stream sourceContentStream
		                                , string nameWithPath
		                                , bool doNotCloseArchiveStream = false
		                                , bool doNotCloseSourceContentStream = false)
		{
			using var archive = new ZipArchive(zippedStream, ZipArchiveMode.Update, doNotCloseArchiveStream);
			var readmeEntry = archive.CreateEntry(nameWithPath);
			using var entry = readmeEntry.Open();
			await sourceContentStream.CopyToAsync(entry);
			if (doNotCloseSourceContentStream)
			{
				sourceContentStream.Seek(0, SeekOrigin.Begin);
			}
			else
			{
				sourceContentStream.Close();
			}
		}

		public async Task AddDirectoryToZipAsync(string zipFilePath
		                                   , params string[] directoryPathsToAdd)
		{
			if (directoryPathsToAdd.Length == 0)
			{
				throw new ArgumentException("Value cannot be an empty collection.", nameof(directoryPathsToAdd));
			}

			using var zippedStream = _io.File.Open(zipFilePath, FileMode.OpenOrCreate);
			foreach (var directory in directoryPathsToAdd)
			{
				await AddDirectoryToZipAsync(zippedStream, directory);
			}
			zippedStream.Close();
		}

		public async Task AddFileToZipAsync(Stream zippedStream
		                                    , string filePath
		                                    , string entryFilePath)
		{
			using var sourceFileStream = File.OpenRead(filePath);
			await AddToZipAsync(zippedStream, sourceFileStream, entryFilePath, true, true);
		}

		public async Task AddDirectoryToZipAsync(Stream zippedStream
		                                         , string directoryPath)
		{
			var files = _io.Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				var relativeFilePath = _io.Path.GetRelativePath(directoryPath, file!);
				var entryFilePath = _io.Path.Combine(_io.Path.GetFileName(directoryPath), relativeFilePath);
				await AddFileToZipAsync(zippedStream, file!, entryFilePath);
			}
		}

		public bool TryValidateItCanBeExtracted(Stream zippedStream)
		{
			var tempFolder = _io.Path.GetTempPath($"{Guid.NewGuid()}", true, true);
			try
			{
				using var archive = new ZipArchive(zippedStream, ZipArchiveMode.Read, true);
				{
					archive.ExtractToDirectory(tempFolder);
				}
				zippedStream.Seek(0, SeekOrigin.Begin);
				return true;
			}
			catch (InvalidDataException)
			{
				return false;
			}
			finally
			{
				_io.Directory.Delete(tempFolder, true);
			}
		}

		public void GetZipEntriesValidated(string filePath
		                                   , int maxFilesAllowed = 200
		                                   , int maxSizeBytesAllowed = 209715200)
		{
			using (var fs = File.Open(filePath, FileMode.Open))
			{
				using (var archive = new ZipArchive(fs))
				{
					ValidateFileLimits(archive, maxFilesAllowed, maxSizeBytesAllowed);
				}
			}
		}

		public IEnumerable<PlatformZipFileEntry> GetZipEntries(Stream archiveStream
		                                                       , int maxFilesAllowed = 200
		                                                       , int maxSizeBytesAllowed = 209715200) //200MB
		{
			if (archiveStream.CanSeek)
			{
				archiveStream.Seek(0, SeekOrigin.Begin);
			}

			using (var archive = new ZipArchive(archiveStream))
			{
				ValidateFileLimits(archive, maxFilesAllowed, maxSizeBytesAllowed);

				foreach (var archiveEntry in archive.Entries)
				{
					yield return new PlatformZipFileEntry(archiveEntry, _io);
				}
			}
		}

		private void ValidateFileLimits(ZipArchive archive
		                                , int maxFilesAllowed
		                                , int maxSizeBytesAllowed)
		{
			var entriesCount = archive.Entries.Count;
			if (entriesCount > maxFilesAllowed)
			{
				throw new InvalidOperationException(
					$"The number of entries in the zipped file({entriesCount}) exceeds the maximum allowed({maxFilesAllowed}). List of Entries: {string.Join("|", archive.Entries)}");
			}

			var totalSizeArchive = archive.Entries.Sum(e => e.Length);
			if (totalSizeArchive > maxSizeBytesAllowed)
			{
				throw new InvalidOperationException(
					$"The uncompressed data size detected so far({totalSizeArchive} bytes) exceeds the maximum allowed size of {maxSizeBytesAllowed} bytes");
			}
		}

		private void ExtractZip(string sourceArchiveFileName
		                        , string destinationDirectoryName
		                        , int maxFilesAllowed
		                        , int maxSizeBytesAllowed
		                        , bool overwrite)
		{
			ValidateFileLimits();

			PerformExtraction();
            // ReSharper disable once LocalFunctionHidesMethod
            void ValidateFileLimits()
			{
				
				// ReSharper disable once ConvertToUsingDeclaration
				using (var archive = ZipFile.OpenRead(sourceArchiveFileName))
				{
					var entriesCount = archive.Entries.Count;
					if (entriesCount > maxFilesAllowed)
					{
						throw new InvalidOperationException(
							$"The number of entries in the zipped file({entriesCount}) exceeds the maximum allowed({maxFilesAllowed}). List of Entries: {string.Join("|", archive.Entries)}");
					}

					var totalSizeArchive = archive.Entries.Sum(e => e.Length);
					if (totalSizeArchive > maxSizeBytesAllowed)
					{
						throw new InvalidOperationException(
							$"The uncompressed data size detected so far({totalSizeArchive} bytes) exceeds the maximum allowed size of {maxSizeBytesAllowed} bytes");
					}
				}
			}

			void PerformExtraction()
			{
				using var archive = ZipFile.OpenRead(sourceArchiveFileName);
				{
					foreach (var file in archive.Entries)
					{
						ResilientOperations.Default.FileSystemRetry(_ => ExtractEntry(file));
					}
				}

				void ExtractEntry(ZipArchiveEntry file)
				{
					var completeFileName = _io.Path.Combine(destinationDirectoryName, file.FullName);
					if (!completeFileName.StartsWith(destinationDirectoryName))
					{
						throw new ArgumentException(
							$"The file contains potentially malicious entries. entry('{file.FullName}')");
					}

					var directory = _io.Path.GetDirectoryName(completeFileName);
					_io.Directory.CreateDirectory(directory, false, false);

					if (!string.IsNullOrWhiteSpace(file.Name))
					{
						file.ExtractToFile(completeFileName, overwrite);
					}
				}
			}
		}


		private void ExtractTarGz(string filename, string outputDir)
		{
			ValidateInput();
			using var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
			using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
			using var tarStream = new TarInputStream(gzipStream, Encoding.UTF8);
			while (tarStream.GetNextEntry() is { } entry)
			{
				var outputFilePath = _io.Path.Combine(outputDir, entry.Name);
				if (entry.IsDirectory)
				{
					_io.Directory.CreateDirectory(outputFilePath);
				}
				else
				{
					var outputDirectory = _io.Path.GetDirectoryName(outputFilePath);
					_io.Directory.CreateDirectory(outputDirectory);
					using var outputStream = _io.File.Create(outputFilePath);
					tarStream.CopyEntryContents(outputStream);
				}
			}

			void ValidateInput()
			{
				if (string.IsNullOrWhiteSpace(filename))
					throw new ArgumentException("Filename must be provided", nameof(filename));
				if (string.IsNullOrWhiteSpace(outputDir))
					throw new ArgumentException("Output directory must be provided", nameof(outputDir));
				if (!_io.File.Exists(filename))
					throw new FileNotFoundException($"File not found: {filename}", filename);
			}
		}
	}
}
