using System;

namespace WebScreenshotTakerApp.Framework.Helpers
{
	/// <remarks>
	/// https://msdn.microsoft.com/en-us/library/dn589788.aspx
	/// http://blogs.msdn.com/b/dgartner/archive/2010/03/09/trying-and-retrying-in-c.aspx
	/// </remarks>
	public static class RetryHelper
	{
		public static TResult Do<TResult>(this Func<TResult> retryAction, Func<Exception, int, bool> shouldRetry)
		{
			var result = default(TResult);

			Action action = () =>
			{
				result = retryAction();
			};

			Do(action, shouldRetry);

			return result;
		}

		public static void Do(this Action retryAction, Func<Exception, int, bool> shouldRetry)
		{
			var counter = 0;
			Exception exception;

			do
			{
				try
				{
					retryAction.Invoke();
					return;
				}
				catch (Exception ex)
				{
					counter++;
					exception = ex;
				}

			} while (shouldRetry(exception, counter));

			throw exception;
		}
	}
}
