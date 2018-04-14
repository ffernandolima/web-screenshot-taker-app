using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Drawing;
using System.Threading.Tasks;
using WebScreenshotTakerApp.Contracts;

namespace WebScreenshotTakerApp
{
	public class WebScreenshotService : IWebScreenshotService, IDisposable
	{
		private TaskCompletionSource<bool> _tscBrowserInitialized;
		private TaskCompletionSource<bool> _tscLoadingStateChanged;

		public WebScreenshotServiceSettings Settings { get; private set; }

		public WebScreenshotService(WebScreenshotServiceSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings), $"{nameof(settings)} cannot be null.");
			}

			this.Settings = settings;

			this.Settings.EnsureUri();
			this.Settings.EnsureBrowserBounds();

			this._tscBrowserInitialized = new TaskCompletionSource<bool>();
			this._tscLoadingStateChanged = new TaskCompletionSource<bool>();
		}

		public async Task<Bitmap> TakeScreenshotAsync()
		{
			const int WINDOWLESSFRAMERATE = 1;
			const int MILLISECONDSTIMEOUT = 2000;

			using (var browserSettings = new BrowserSettings { WindowlessFrameRate = WINDOWLESSFRAMERATE })
			using (var chromiumWebBrowser = new ChromiumWebBrowser(browserSettings: browserSettings) { Size = new Size(this.Settings.BrowserWidth, this.Settings.BrowserHeight) })
			{
				if (!chromiumWebBrowser.IsBrowserInitialized)
				{
					chromiumWebBrowser.BrowserInitialized += this.BrowserInitialized;
				}
				else
				{
					this.BrowserInitialized(chromiumWebBrowser, EventArgs.Empty);
				}

				chromiumWebBrowser.LoadingStateChanged += this.LoadingStateChanged;
				chromiumWebBrowser.LoadError += this.LoadError;

				await this._tscBrowserInitialized.Task;
				await this._tscLoadingStateChanged.Task;

				await Task.Delay(MILLISECONDSTIMEOUT);

				return await chromiumWebBrowser.ScreenshotAsync();
			}
		}

		public async Task<Bitmap> TakeScreenshotAndGenerateThumbnailAsync()
		{
			this.Settings.EnsureThumbnailBounds();

			var screenshot = await this.TakeScreenshotAsync();
			var thumbnail = (Bitmap)screenshot.GetThumbnailImage(this.Settings.ThumbnailWidth, this.Settings.ThumbnailHeight, null, IntPtr.Zero);

			return thumbnail;
		}

		private void BrowserInitialized(object sender, EventArgs e)
		{
			const double ZOOMLEVEL = 0.0;

			var chromiumWebBrowser = (ChromiumWebBrowser)sender;
			chromiumWebBrowser.BrowserInitialized -= this.BrowserInitialized;

			chromiumWebBrowser.SetZoomLevel(ZOOMLEVEL);
			chromiumWebBrowser.Load(this.Settings.Uri.ToString());

			this._tscBrowserInitialized.TrySetResult(true);
		}

		private void LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
		{
			if (!e.IsLoading)
			{
				var chromiumWebBrowser = (ChromiumWebBrowser)sender;
				chromiumWebBrowser.LoadingStateChanged -= this.LoadingStateChanged;

				this._tscLoadingStateChanged.TrySetResult(true);
			}
		}

		private void LoadError(object sender, LoadErrorEventArgs e)
		{
			if (e.Frame.IsMain)
			{
				var chromiumWebBrowser = (ChromiumWebBrowser)sender;
				chromiumWebBrowser.LoadError -= this.LoadError;

				throw new Exception($"An error occurred while taking screenshot. Url: {this.Settings.Uri.ToString()}.", new Exception(e.ErrorText));
			}
		}

		#region IDisposable Members

		private bool _disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					this.Settings = null;

					this._tscBrowserInitialized = null;
					this._tscLoadingStateChanged = null;
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
