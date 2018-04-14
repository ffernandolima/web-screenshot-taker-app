using System;
using System.ComponentModel;
using System.Configuration;

namespace WebScreenshotTakerApp.Configuration
{
	public class Settings
	{
		// Static holder for instance, need to use lambda to construct since constructor private
		private static readonly Lazy<Settings> _instance = new Lazy<Settings>(() => new Settings());

		// Private to prevent direct instantiation
		private Settings()
		{
			this.AppSettingsToObject();
		}

		// Accessor for instance
		public static Settings Instance => _instance.Value;
		public int BrowserWidth { get; private set; }
		public int BrowserHeight { get; private set; }
		public int ThumbnailWidth { get; private set; }
		public int ThumbnailHeight { get; private set; }
		public TimeSpan MillisecondsTimeoutWaitTask { get; private set; }
		public int MaximumRetries { get; private set; }

		private void AppSettingsToObject()
		{
			foreach (var property in this.GetType().GetProperties())
			{
				var value = ConfigurationManager.AppSettings[property.Name];

				if (!string.IsNullOrEmpty(value))
				{
					var propertyType = property.PropertyType;

					var converter = TypeDescriptor.GetConverter(propertyType);

					if (converter.CanConvertFrom(typeof(string)))
					{
						property.SetValue(this, converter.ConvertFrom(value), null);
					}
				}
			}
		}
	}
}
