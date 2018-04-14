using System;
using System.IO;
using System.Reflection;

namespace WebScreenshotTakerApp.CefSharp
{
	public static class CefSharpAssemblyResolver
	{
		public static void Initialize()
		{
			AppDomain.CurrentDomain.AssemblyResolve += Resolve;
		}

		static Assembly Resolve(object sender, ResolveEventArgs args)
		{
			if (args.Name.StartsWith("CefSharp"))
			{
				var splittedAssemblyName = args.Name.Split(new[] { ',' }, 2);

				var assemblyName = string.Concat(splittedAssemblyName[0], ".dll");

				var fullAssemblyName = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Environment.Is64BitProcess ? "x64" : "x86", assemblyName);

				if (File.Exists(fullAssemblyName))
				{
					var assembly = Assembly.LoadFile(fullAssemblyName);

					return assembly;
				}
			}

			return null;
		}
	}
}
