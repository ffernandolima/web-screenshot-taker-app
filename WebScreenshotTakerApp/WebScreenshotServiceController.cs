using ImageMagick;
using System;
using System.Drawing.Imaging;
using System.IO;
using WebScreenshotTakerApp.Configuration;
using WebScreenshotTakerApp.Contracts;
using WebScreenshotTakerApp.Exceptions;
using WebScreenshotTakerApp.Framework.Helpers;
using WebScreenshotTakerApp.Logging;

namespace WebScreenshotTakerApp
{
	public class WebScreenshotServiceController : IWebScreenshotServiceController
	{
		private readonly Settings _settings;

		public WebScreenshotServiceController()
		{
			this._settings = Settings.Instance;
		}

		public void TakeScreenshotAndGenerateThumbnail(string url, string localPath)
		{
			this.EnsureParameters(url, localPath);

			try
			{
				RetryHelper.Do(() => this.TakeScreenshotAndGenerateThumbnailInternal(url, localPath), this.ShouldRetry);
			}
			catch (Exception ex)
			{
				var exception = new WebScreenshotTakerAppException(ex);

				Log4netHelper.All(x => x.Error($"{exception.ToString()}"));

				throw exception;
			}
		}

		private void EnsureParameters(string url, string localPath)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				var argumentException = new ArgumentException($"{nameof(url)} cannot be null or white-space.", $"{nameof(url)}");
				throw argumentException;
			}

			if (string.IsNullOrWhiteSpace(localPath))
			{
				var argumentException = new ArgumentException($"{nameof(localPath)} cannot be null or white-space.", $"{nameof(localPath)}");
				throw argumentException;
			}
		}

		private void TakeScreenshotAndGenerateThumbnailInternal(string url, string localPath)
		{
			var thumbnailFile = new FileInfo(localPath);

			var webScreenshotServiceSettings = new WebScreenshotServiceSettings
			{
				Uri = new Uri(url),

				BrowserWidth = this._settings.BrowserWidth,
				BrowserHeight = this._settings.BrowserHeight,

				ThumbnailWidth = this._settings.ThumbnailWidth,
				ThumbnailHeight = this._settings.ThumbnailHeight
			};

			using (var webScreenshotService = new WebScreenshotService(webScreenshotServiceSettings))
			using (var thumbnailTask = webScreenshotService.TakeScreenshotAndGenerateThumbnailAsync())
			{
				var completed = thumbnailTask.Wait((int)this._settings.MillisecondsTimeoutWaitTask.TotalMilliseconds);
				if (!completed)
				{
					throw new Exception($"The thumbnail could not be generated. The process has waited for {this._settings.MillisecondsTimeoutWaitTask.TotalSeconds} seconds. Url: {url}.");
				}

				var exception = thumbnailTask.Exception;
				if (exception != null)
				{
					throw new Exception($"An internal error occurred while running the task. Url: {url}.", exception);
				}

				if (!thumbnailFile.Directory.Exists)
				{
					thumbnailFile.Directory.Create();
					thumbnailFile.Directory.Refresh();
				}

				if (thumbnailFile.Exists)
				{
					thumbnailFile.Delete();
					thumbnailFile.Refresh();
				}

				thumbnailTask.Result.Save(thumbnailFile.FullName, ImageFormat.Jpeg);
			}

			thumbnailFile.Refresh();

			using (var image = new MagickImage(thumbnailFile.FullName))
			{
				var isWhite = this.CheckImageColor(image, MagickColors.White);
				if (isWhite)
				{
					throw new Exception($"Something went wrong, the thumbnail is totally white. Url: {url}.");
				}

				var isBlack = this.CheckImageColor(image, MagickColors.Black);
				if (isBlack)
				{
					throw new Exception($"Something went wrong, the thumbnail is totally black. Url: {url}.");
				}
			}
		}

		private bool ShouldRetry(Exception exception, int count)
		{
			return count <= this._settings.MaximumRetries;
		}

		private bool CheckImageColor(MagickImage image, MagickColor color)
		{
			using (var pixels = image.GetPixels())
			{
				foreach (var pixel in pixels)
				{
					var pixelColor = pixel.ToColor();

					if (pixelColor != color)
					{
						return false;
					}
				}
			}

			return true;
		}

		#region IDisposable Members

		private bool _disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{

				}
			}

			this._disposed = true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Members
	}
}
