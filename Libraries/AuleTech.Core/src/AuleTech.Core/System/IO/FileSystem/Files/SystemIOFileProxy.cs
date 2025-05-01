using System.Buffers;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Text;
using AuleTech.Core.Resiliency;
using AuleTech.Core.System.IO.FileSystem.PlatformModels;
using Microsoft.Extensions.Logging;

namespace AuleTech.Core.System.IO.FileSystem.Files
{
	internal sealed partial class SystemIoFileProxy : ISystemIoFileDirect
	{
		public SystemIoFileProxy(bool bypassCache)
		{
			_bypassCache = bypassCache;
		}

		public async Task WriteAllBytesAsync(string path
		                                     , byte[] bytes
		                                     , CancellationToken cancellationToken = default)
		{
			ThrowIfInvalidFilePath(path);

			if (bytes == null)
			{
				throw new ArgumentNullException(nameof(bytes));
			}

			await ResilientOperations.Default.FileSystemRetryAsync(async _ =>
				{
					using (var fs = AsyncWriteFileStream(path, false))
					{
						await fs.WriteAsync(bytes,0,bytes.Length, cancellationToken)
							.ConfigureAwait(false);
						await fs.FlushAsync(cancellationToken)
							.ConfigureAwait(false);
					}
				}
				, cancellationToken);
		}

		public Task<string[]> ReadAllLinesAsync(string filePath
		                                        , CancellationToken cancellationToken = default) =>
			ReadAllLinesAsync(filePath, DefaultUtf8Encoding, cancellationToken);

		public async Task<string[]> ReadAllLinesAsync(string path
		                                              , Encoding encoding
		                                              , CancellationToken cancellationToken = default)
		{
			ThrowIfInvalidFilePathOrEncoding(path, encoding);
			return await ResilientOperations.Default.FileSystemRetryAsync(async _ =>
				{
					using (var sr = AsyncStreamReader(path, encoding))
					{
						cancellationToken.ThrowIfCancellationRequested();
						string? line;
						var lines = new List<string>();
						while ((line = await sr.ReadLineAsync(cancellationToken)
							       .ConfigureAwait(false))
						    != null)
						{
							lines.Add(line);
							cancellationToken.ThrowIfCancellationRequested();
						}

						return lines.ToArray();
					}
				}
				, cancellationToken);
		}

		public string ReadAllText(string path) => ReadAllText(path, DefaultUtf8Encoding);

		public string ReadAllText(string path, Encoding encoding)
		{
			char[]? buffer = null;
			return ResilientOperations.Default.FileSystemRetry( _ =>
			{
				using (var sr = AsyncStreamReader(path, encoding))
				{
					try
					{
						buffer = ArrayPool<char>.Shared.Rent(sr.CurrentEncoding.GetMaxCharCount(DefaultBufferSize));
						var sb = new StringBuilder();

						int read;
						do
						{
							read = sr.Read(buffer, 0, buffer.Length);
							sb.Append(buffer, 0, read);
						} while (read != 0);

						return sb.ToString();
					}
					finally
					{
						sr.Dispose();
						if (buffer != null)
						{
							ArrayPool<char>.Shared.Return(buffer);
						}
					}
				}
			});
		}

		public Task<string> ReadAllTextAsync(string path
		                                     , CancellationToken cancellationToken = default) =>
			ReadAllTextAsync(path, DefaultUtf8Encoding, cancellationToken);

		public async Task<string> ReadAllTextAsync(string path
		                                           , Encoding encoding
		                                           , CancellationToken cancellationToken = default)
		{
			ThrowIfInvalidFilePathOrEncoding(path, encoding);
			char[] ?buffer = null;
			return await ResilientOperations.Default.FileSystemRetryAsync(async _ =>
				{
					using (var sr = AsyncStreamReader(path, encoding))
					{
						try
						{
							cancellationToken.ThrowIfCancellationRequested();
							buffer = ArrayPool<char>.Shared.Rent(sr.CurrentEncoding.GetMaxCharCount(DefaultBufferSize));
							var sb = new StringBuilder();

							int read;
							do
							{
								read = await sr.ReadAsync(buffer, 0, buffer.Length)
									.ConfigureAwait(false);
								sb.Append(buffer, 0, read);
							} while (read != 0);

							return sb.ToString();
						}
						finally
						{
							sr.Dispose();
							if (buffer != null)
							{
								ArrayPool<char>.Shared.Return(buffer);
							}
						}
					}
				}
				, cancellationToken);
		}

		public async Task<byte[]> ReadAllBytesAsync(string path
		                                              , CancellationToken cancellationToken = default)
		{
			ThrowIfInvalidFilePath(path);
			return await ResilientOperations.Default.FileSystemRetryAsync(async _ =>
				{
					using (var sr = AsyncReadFileStream(path))
					{
						cancellationToken.ThrowIfCancellationRequested();
						return await sr.ReadAllBytesAsync(cancellationToken); 
					}
				}
				, cancellationToken);
		}

		public Task DeleteAsync(string path
		                        , bool throwIfNotExists = true
		                        , CancellationToken cancellationToken = default)
		{
			ResilientOperations.Default.RetryIfNeeded(_ =>
				{
					Console.WriteLine(() => $"{nameof(DeleteAsync)}({_})");
					if (_bypassCache)
					{
						using (var stream = new FileStream(path
							, FileMode.Open
							, FileAccess.Read
							, FileShare.None
							, 1
							, FileOptions.DeleteOnClose | FileOptions.Asynchronous | FileOptions.WriteThrough))
						{
							stream.Flush();
							stream.Close();
						}
					}
					else
					{
						if (Exists(path, cancellationToken))
						{
							File.Delete(path);
						}
					}

					return Exists(path);
				}
				, mustRetry: exists => exists
				, timeout: TimeSpan.FromSeconds(15)
				, waitBetweenAttempts: TimeSpan.FromMilliseconds(50)
				, onException: ex =>
				{
					if (throwIfNotExists && ex is FileNotFoundException)
					{
						throw ex;
					}
				}
				, cancellationToken: cancellationToken);
			return Task.CompletedTask;
		}

		public async Task ExtractZipArchiveAsync(string archivePath
		                                         , string targetFolder
		                                         , bool overwriteOutputIfExists = false
		                                         , CancellationToken cancellationToken = default)
		{
			Console.WriteLine(() => $"{nameof(ExtractZipArchiveAsync)}");
			var io = SystemIoProxy.Default;
			var zipFile = archivePath;
			string? tempFolder = null;
			if (overwriteOutputIfExists)
			{
				if (io.Path.GetDirectoryName(archivePath)
					.Equals(targetFolder, StringComparison.InvariantCultureIgnoreCase))
				{
					tempFolder = io.Path.GetTempPath(Guid.NewGuid()
							.ToString()
						, true
						, true);
					zipFile = io.Path.Combine(tempFolder
						, Guid.NewGuid()
							.ToString());
					await CopyAsync(archivePath, zipFile, true, cancellationToken);
				}

				ClearTargetFolder();
			}

			io.ZipCompression.ExtractToDirectory(zipFile, targetFolder);

			if (!Exists(archivePath, cancellationToken))
			{
				await CopyAsync(zipFile, archivePath, true, cancellationToken);
			}

			if (tempFolder != null)
			{
				io.Directory.Delete(tempFolder, true, false);
			}

			void ClearTargetFolder()
			{
				try
				{
					ResilientOperations.Default.RetryIfNeeded(pendingAttempts =>
						{
							try
							{
								io.Directory.Delete(targetFolder, true, false);
							}
							catch
							{
								Console.WriteLine(() => $"{nameof(ClearTargetFolder)} pending attempts({pendingAttempts})");
								throw;
							}
						}
						, 100
						, cancellationToken: cancellationToken);
				}
				catch (IOException ex)
				{
					if (!ex.Message?.Contains("Directory not empty") ?? false)
					{
						throw;
					}

					string files;
					try
					{
						files = string.Join('.',io.Directory.GetFiles(targetFolder));
					}
					catch (Exception e)
					{
						files = $"Could not resolve. Got {e.Message}";
					}

					throw new IOException($"{ex.Message}. Files in directory({files.Length}): ({files})", ex);
				}
			}
		}

		public void CreateZipFromDirectory(string path
		                                   , string outputFilePath)
		{
			if (!_bypassCache)
			{
				ZipFile.CreateFromDirectory(path, outputFilePath, CompressionLevel.Optimal, false);
			}
			else
			{
				var io = SystemIoProxy.Default;
				var stream = AsyncWriteFileStream(outputFilePath, false);
				using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, false))
				{
					foreach (var file in io.Directory.GetFiles(path, "*", SearchOption.AllDirectories))
					{
						archive.CreateEntryFromFile(file!
							, io.Path.GetRelativePath(path, file!)
							, CompressionLevel.Optimal);
					}
				}
			}
		}

		public void Copy(string sourceFile
		                 , string destinationFile
		                 , bool overwrite) => CopyAsync(sourceFile, destinationFile, overwrite)
			.GetAwaiter()
			.GetResult();

		public async Task CopyAsync(string sourceFile
		                            , string destinationFile
		                            , bool overWrite
		                            , CancellationToken cancellationToken = default)
		{
			using (var source = OpenRead(sourceFile))
			{
				await CopyAsync(source, destinationFile, overWrite, cancellationToken);
			}
		}

		public Task MoveAsync(string sourceFile
		                      , string destinationFile
		                      , CancellationToken cancellationToken = default) =>
			MoveAsync(sourceFile, destinationFile, false, cancellationToken);

		public async Task MoveAsync(string sourceFile
		                            , string destinationFile
		                            , bool overWrite
		                            , CancellationToken cancellationToken = default)
		{
			if (overWrite && Exists(destinationFile, cancellationToken))
			{
				await DeleteAsync(destinationFile, false, cancellationToken);
			}

			File.Move(sourceFile, destinationFile);
		}

		public async Task CopyAsync(Stream sourceStream
		                            , string destinationFile
		                            , bool overWrite
		                            , CancellationToken cancellationToken = default)
		{
			if (!overWrite && Exists(destinationFile, cancellationToken))
			{
				throw new IOException($"File {destinationFile} exists and not overWrite selected");
			}

			await ResilientOperations.Default.FileSystemRetryAsync(async _ =>
				{
					if (sourceStream.CanSeek)
					{
						sourceStream.Seek(0, SeekOrigin.Begin);
					}

					var tmp = $"{destinationFile}.tmp";
					using (var destination = Create(tmp))
					{
						await sourceStream.CopyToAsync(destination);
					}

					await MoveAsync(tmp, destinationFile!,overWrite,cancellationToken);
				}
				, cancellationToken);
		}

	

		public bool Exists(string fileName
		                   , CancellationToken cancellationToken = default)
		{
			if (!_bypassCache)
			{
				fileName = (PlatformPathStringStandard)fileName;
				return ResilientOperations.Default.RetryIfNeeded(_ => File.Exists(fileName)
					, TimeSpan.FromSeconds(30)
					, TimeSpan.Zero
					, cancellationToken: cancellationToken);
			}

			try
			{
				using (AsyncReadFileStream(fileName))
				{
					;
				}

				return true;
			}
			catch (FileNotFoundException)
			{
				return false;
			}
			catch (DirectoryNotFoundException)
			{
				return false;
			}
		}

		public async Task WriteStreamAsync(string destinationFile
		                                   , Stream stream
		                                   , FileMode fileMode = FileMode.Create
		                                   , CancellationToken cancellationToken = default)
		{
			await ResilientOperations.Default.FileSystemRetryAsync(async _ =>
				{
					Console.WriteLine(() => $"{nameof(WriteStreamAsync)}({_})");
					if (stream.CanSeek)
					{
						stream.Seek(0, SeekOrigin.Begin);
					}

					using (var file = Open(destinationFile, fileMode))
					{
						await stream.CopyToAsync(file);
					}
				}
				, cancellationToken);
		}

		public FileAttributes GetAttributes(string fileOrFolderPath) => File.GetAttributes(fileOrFolderPath);

		public Stream OpenRead(string path) => ResilientOperations.Default.FileSystemRetry(_ =>
			AsyncReadFileStream(path));

		public Stream Create(string path) => Open(path, FileMode.Create);
		public string GetFileName(string path) => Path.GetFileName(path);

		public Stream Open(string path
		                   , FileMode mode) => ResilientOperations.Default.FileSystemRetry(_ =>
			AsyncWriteFileStream(path, FileMode.Append == mode));

		public Stream Open(string path
		                   , FileMode mode
		                   , FileAccess access
		                   , FileShare share) => ResilientOperations.Default.FileSystemRetry(_ =>
			File.Open(path
				, mode
				, access
				, share));
		public PlatformImage ImageFromFile(string path)
		{
			if (!OperatingSystem.IsWindows())

			{
				throw new NotSupportedException("https://aka.ms/systemdrawingnonwindows");
			}
			return ResilientOperations.Default.FileSystemRetry(_ => new PlatformImage(OpenRead(path)));
		}

		public async Task SaveBitMapAsync(Bitmap bitmap
		                                  , string path
		                                  , ImageFormat format
		                                  , CancellationToken cancellationToken = default)
		{
			if (!OperatingSystem.IsWindows())
			{
				throw new NotSupportedException("https://aka.ms/systemdrawingnonwindows");
			}
			var io = SystemIoProxy.Default;

			var folder = io.Path.GetDirectoryName(path);
			io.Directory.CreateDirectory(folder, false, false);
			if (!_bypassCache)
			{
				ResilientOperations.Default.FileSystemRetry(_ => bitmap.Save(path, format), cancellationToken);
				return;
			}

			using (var fileStream = AsyncWriteFileStream(path, false))
			{
				bitmap.Save(fileStream, format);
				await fileStream.FlushAsync(cancellationToken);
				fileStream.Close();
			}
		}

		public DateTime GetCreationTimeUtc(string filePath) =>
			ResilientOperations.Default.FileSystemRetry(_ => File.GetCreationTimeUtc(filePath));

		public string GetFileExtension(string input
		                               , bool includeDot = true)
		{
			var fileExtension = Path.GetExtension(input);
			return includeDot ? fileExtension : fileExtension.TrimStart('.');
		}

		public long GetFileSize(string filePath)
		{
			if (!_bypassCache)
			{
				return ResilientOperations.Default.FileSystemRetry(_ => new FileInfo(filePath).Length);
			}

			using var str = OpenRead(filePath);
			return str.Length;
		}

		public Stream OpenWrite(string path) => AsyncWriteFileStream(path, false);

		public Task WriteAllLinesAsync(string path
		                               , IEnumerable<string> contents
		                               , CancellationToken token = default) =>
			ResilientOperations.Default.FileSystemRetryAsync(async _ =>
				{
					using (var writer = AsyncStreamWriter(path, DefaultUtf8Encoding, false))
					{
						await WriteAllLinesAsync(writer, contents, token);
					}
				}
				, token);

		public async Task WriteAllLinesAsync(TextWriter writer
		                                     , IEnumerable<string> contents
		                                     , CancellationToken cancellationToken = default)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			if (contents == null)
			{
				throw new ArgumentNullException(nameof(contents));
			}

			foreach (var line in contents)
			{
				cancellationToken.ThrowIfCancellationRequested();
				await writer.WriteLineAsync(line);
			}

			cancellationToken.ThrowIfCancellationRequested();
			await writer.FlushAsync();
		}

		public void WriteAllText(string path
		                         , string content)
		{
			using (var writer = AsyncStreamWriter(path, false))
			{
				writer.Write(content);
				writer.Flush();
			}
		}

		public Task WriteAllTextAsync(string path
		                              , string content
		                              , CancellationToken cancellationToken = default) =>
			WriteAllTextAsync(path, content, DefaultUtf8Encoding, cancellationToken);

		public async Task WriteAllTextAsync(string path
		                                    , string contents
		                                    , Encoding encoding
		                                    , CancellationToken cancellationToken = default)
		{
			ThrowIfInvalidFilePathOrEncoding(path, encoding);


			if (string.IsNullOrEmpty(contents))
			{
				using (AsyncWriteFileStream(path, false))
				{
					;
				}
			}
			else
			{
				using (var writer = AsyncStreamWriter(path, encoding, false))
				{
					await WriteAllTextAsync(writer, contents, cancellationToken);
				}
			}
		}

		public async Task WriteAllTextAsync(StreamWriter sw
		                                    , string contents
		                                    , CancellationToken cancellationToken = default)
		{
			await ResilientOperations.Default.FileSystemRetryAsync(async _ =>
				{
					if (sw.BaseStream.CanSeek)
					{
						sw.BaseStream.Seek(0, SeekOrigin.Begin);
					}

					char[] buffer=Array.Empty<char>();
					try
					{
						buffer = ArrayPool<char>.Shared.Rent(DefaultBufferSize);
						var count = contents.Length;
						var index = 0;
						while (index < count)
						{
							var batchSize = Math.Min(DefaultBufferSize, count - index);
							contents.CopyTo(index, buffer, 0, batchSize);
							await sw.WriteAsync(buffer, 0, batchSize)
								.ConfigureAwait(false);

							index += batchSize;
						}

						cancellationToken.ThrowIfCancellationRequested();
						await sw.FlushAsync()
							.ConfigureAwait(false);
					}
					finally
					{
						if (buffer != null)
						{
							ArrayPool<char>.Shared.Return(buffer);
						}
					}
				}
				, cancellationToken);
		}

		public async Task<string> CopyToTempAsync(string filePath
		                                          , CancellationToken cancellationToken = default)
		{
			var path = SystemIoProxy.Default.Path;
			var extension = path.GetExtension(filePath);

			var result = $"{path.GetTempPath()}/{Guid.NewGuid().ToString().Replace("-", string.Empty)}.{extension}"
				.Trim()
				.TrimEnd('.');
			await CopyAsync(filePath, result, true, cancellationToken);
			return result;
		}

		public async Task<string> CopyToTempAsync(Stream stream
		                                          , string? usingFileName = null
		                                          , CancellationToken cancellationToken = default)
		{
			var path = SystemIoProxy.Default.Path;
			usingFileName ??= Guid.NewGuid()
				.ToString()
				.Replace("-", string.Empty);
			var result = $"{path.GetTempPath()}{usingFileName}";
			await CopyAsync(stream, result, true, cancellationToken);
			return result;
		}

		public bool IsLocked(string filePath
		                     , TimeSpan waitTimeout)
		{
			try
			{
				return ResilientOperations.Default.RetryIfNeeded(_ => IsLocked(filePath)
					, mustRetry: isLocked => isLocked
					, timeout: waitTimeout
					, waitBetweenAttempts: TimeSpan.FromMilliseconds(100)
				);
			}
			catch (Exception ex)
			{
				Console.WriteLine(() => ex.ToString());
				return true;
			}
		}

		public bool IsLocked(string filePath)
		{
			var result = false;
			try
			{
				using (var file =
					Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
				{
					file.Close();
				}
			}
			catch (IOException)
			{
				result = true;
			}

			return result;
		}

		public DateTime GetLastWriteTime(string path)
		{
			ThrowIfInvalidFilePath(path);
			return File.GetLastWriteTime(path).ToUniversalTime();
		}

		public Encoding GetUtf8Encoding(string path)
		{
			using var reader = new FileStream(path, FileMode.Open, FileAccess.Read);
			var bytes = new byte[3];
			reader.ReadExactly(bytes, 0, 3);
			return GetUtf8Encoding(bytes);
		}

		public Encoding GetUtf8Encoding(byte[] bytes)
		{
			var isBom = bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
			return new UTF8Encoding(isBom);
		}

		public async Task ReplaceTokensAsync(string path
		                                     , IReadOnlyDictionary<string, string> tokensToReplace
		                                     , CancellationToken cancellationToken)
		{
			var fileContent = await ReadAllTextAsync(path, cancellationToken);
			Console.WriteLine(() => $"Setting variables in: {path}");
			foreach (var entry in tokensToReplace)
			{
				Console.WriteLine(() => $"Replacing {entry.Key} by '{entry.Value}'.");
				fileContent = fileContent.Replace(entry.Key, entry.Value);
			}

			await WriteAllTextAsync(path, fileContent, cancellationToken);
		}

		public Task ReplaceTokenAsync(string path
		                               , string key
		                               , string value
		                               , CancellationToken cancellationToken) =>
			ReplaceTokensAsync(path
				, new Dictionary<string, string>
				{
					{ key, value }
				}
				, cancellationToken
			);

		public void AppendAllText(string path
		                       , string content) =>
			File.AppendAllText(path, content);
	}
}
