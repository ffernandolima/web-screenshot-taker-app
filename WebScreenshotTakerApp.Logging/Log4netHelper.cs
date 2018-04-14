using log4net;
using System;
using System.IO;
using System.Linq;

namespace WebScreenshotTakerApp.Logging
{
	public static class Log4netHelper
	{
		private static string _loggerPrefix;

		public static string LoggerPrefix
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_loggerPrefix))
				{
					_loggerPrefix = DefaultPrefix();
				}

				return _loggerPrefix;
			}
			set
			{
				_loggerPrefix = value;
			}
		}

		public static ILog RollingFileLogger
		{
			get
			{
				return LogManager.GetLogger(LoggerPrefix + ".RollingFileLogger");
			}
		}

		public static void All(Action<ILog> logAction)
		{
			var loggers = new[] { RollingFileLogger };

			var parallelQuery = loggers.AsParallel().Where(x => x != null);

			parallelQuery.ForAll(logAction);
		}

		private static string DefaultPrefix()
		{
			var appName = AppDomain.CurrentDomain.FriendlyName;

			var domainName = Path.GetFileNameWithoutExtension(appName);
#if DEBUG
			domainName = domainName.Replace(".vshost", string.Empty);
#endif
			return domainName;
		}
	}
}
