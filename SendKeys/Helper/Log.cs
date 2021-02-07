using System;

namespace SendKeys.Helper
{
	/// <summary>
	/// Log helper class
	/// </summary>
	internal static class Log
	{
		/// <summary>
		/// True if verbose logging is enabled
		/// </summary>
		public static bool VerboseEnabled { get; set; }

		/// <summary>
		/// Log info message
		/// </summary>
		public static void Info(string format, params object[] arg)
		{
			Write(ConsoleColor.White, format, arg);
		}

		/// <summary>
		/// Log a verbose message. Only written if verbose logging is enabled
		/// </summary>
		public static void Verbose(string format, params object[] arg)
		{
			if (VerboseEnabled)
			{
				Write(ConsoleColor.Cyan, format, arg);
			}
		}

		/// <summary>
		/// Log an error message
		/// </summary>
		public static void Error(string format, params object[] arg)
		{
			Write(ConsoleColor.Red, "ERROR: " + format, arg);
		}

		/// <summary>
		/// Actually write the log message to console
		/// </summary>
		private static void Write(ConsoleColor color, string format, params object[] arg)
		{
			if (VerboseEnabled)
			{
				format = DateTime.Now.ToString("HH:mm:ss.fff") + "  " + format;
			}

			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(format, arg);
			Console.ForegroundColor = oldColor;
		}
	}
}