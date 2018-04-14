using System;

namespace WebScreenshotTakerApp.Exceptions
{
	public class WebScreenshotTakerAppException : Exception
	{
		private const string EXCEPTION_MESSAGE = "An error occurred while processing your request.";

		public WebScreenshotTakerAppException(Exception innerException)
			: base(EXCEPTION_MESSAGE, innerException)
		{
		}
	}
}
