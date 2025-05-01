using System.Drawing;

namespace AuleTech.Core.System.IO.FileSystem.PlatformModels
{
	public class PlatformImage : IDisposable
	{
		private readonly Lazy<Image> _image;
		private readonly Stream _stream;

		public Image Image => _image.Value;

		public PlatformImage(Stream stream)
		{
			_stream = stream ?? throw new ArgumentNullException(nameof(stream));
			_image = new Lazy<Image>(() => Image.FromStream(stream));
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (_image.IsValueCreated)
			{
				_image.Value?.Dispose();
			}

			_stream?.Dispose();
		}

		~PlatformImage()
		{
			Dispose(false);
		}

		public static implicit operator Image(PlatformImage src) => src.Image;
	}
}
