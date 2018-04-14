using System;
using System.Text.RegularExpressions;

namespace WebScreenshotTakerApp.Contracts
{
	public class WebScreenshotServiceSettings
	{
		private static readonly Regex URL_REGEX = new Regex(@"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public WebScreenshotServiceSettings()
		{ }

		public Uri Uri { get; set; }

		public int BrowserWidth { get; set; }
		public int BrowserHeight { get; set; }

		public int ThumbnailWidth { get; set; }
		public int ThumbnailHeight { get; set; }

		public void EnsureUri()
		{
			if (this.Uri == null || string.IsNullOrWhiteSpace(this.Uri.ToString()))
			{
				throw new ArgumentException($"{nameof(this.Uri)} cannot be null or white-space.", nameof(this.Uri));
			}

			if (!this.Uri.IsWellFormedOriginalString())
			{
				throw new ArgumentException($"{nameof(this.Uri)} was not well-formed.", nameof(this.Uri));
			}

			if (!URL_REGEX.IsMatch(this.Uri.ToString()))
			{
				throw new ArgumentException("The provided URL is not valid.", nameof(this.Uri));
			}
		}

		public void EnsureBrowserBounds()
		{
			if (this.BrowserWidth <= 0)
			{
				throw new ArgumentException($"{nameof(this.BrowserWidth)} cannot be less than or equal to zero.", nameof(this.BrowserWidth));
			}

			if (this.BrowserHeight <= 0)
			{
				throw new ArgumentException($"{nameof(this.BrowserHeight)} cannot be less than or equal to zero.", nameof(this.BrowserHeight));
			}
		}

		public void EnsureThumbnailBounds()
		{
			if (this.ThumbnailWidth <= 0)
			{
				throw new ArgumentException($"{nameof(this.ThumbnailWidth)} cannot be less than or equal to zero.", nameof(this.ThumbnailWidth));
			}

			if (this.ThumbnailHeight <= 0)
			{
				throw new ArgumentException($"{nameof(this.ThumbnailWidth)} cannot be less than or equal to zero.", nameof(this.ThumbnailHeight));
			}
		}
	}
}
