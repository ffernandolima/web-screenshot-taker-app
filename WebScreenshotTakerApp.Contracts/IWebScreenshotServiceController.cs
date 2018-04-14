using System;

namespace WebScreenshotTakerApp.Contracts
{
	public interface IWebScreenshotServiceController : IDisposable
	{
		void TakeScreenshotAndGenerateThumbnail(string url, string localPath);
	}
}
