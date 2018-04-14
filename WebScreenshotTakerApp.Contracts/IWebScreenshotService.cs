using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WebScreenshotTakerApp.Contracts
{
	public interface IWebScreenshotService : IDisposable
	{
		WebScreenshotServiceSettings Settings { get; }

		Task<Bitmap> TakeScreenshotAsync();
		Task<Bitmap> TakeScreenshotAndGenerateThumbnailAsync();
	}
}
