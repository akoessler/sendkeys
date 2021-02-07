using System;
using System.Diagnostics;
using System.Linq;

namespace SendKeys.Helper
{
	/// <summary>
	/// Helper class for process stuff
	/// </summary>
	internal static class ProcessHelper
	{
		/// <summary>
		/// Search a process by either process id or name
		/// </summary>
		public static Process FindProcess(int processId, string processName)
		{
			return FindProcessById(processId) ?? FindProcessByNameExact(processName) ?? FindProcessByNameContains(processName);
		}

		/// <summary>
		/// Get the process by id, if valid
		/// </summary>
		private static Process FindProcessById(int processId)
		{
			if (processId != 0)
			{
				return null;
			}

			Log.Verbose("Searching process by id: {0}", processId);

			var process = Process.GetProcessById(processId);
			if (process.Id != 0)
			{
				LogFoundProcess("by id", process);
				return process;
			}
			else
			{
				Log.Verbose("Did not find a process with id: {0}", processId);
				return null;
			}
		}

		/// <summary>
		/// Find a running process with an exact name
		/// </summary>
		private static Process FindProcessByNameExact(string processName)
		{
			if (string.IsNullOrEmpty(processName))
			{
				return null;
			}

			Log.Verbose("Searching process by exact name: {0}", processName);

			var process = Process.GetProcessesByName(processName).FirstOrDefault(x => x.Id != 0);

			if (process != null)
			{
				LogFoundProcess("by exact name", process);
				return process;
			}
			else
			{
				Log.Verbose("Did not find a process with exact name: {0}", processName);
				return null;
			}
		}

		/// <summary>
		/// Find a running process whose name contains the search string
		/// </summary>
		private static Process FindProcessByNameContains(string processName)
		{
			if (string.IsNullOrEmpty(processName))
			{
				return null;
			}

			Log.Verbose("Searching process by name contains: {0}", processName);

			var process = Process.GetProcesses()
				.OrderBy(x => x.ProcessName)
				.Where(x => x.Id != 0)
				.FirstOrDefault(x => x.ProcessName.Contains(processName, StringComparison.OrdinalIgnoreCase));

			if (process != null)
			{
				LogFoundProcess("that contains", process);
				return process;
			}
			else
			{
				Log.Verbose("Did not find a process that contains: {0}", processName);
				return null;
			}
		}

		/// <summary>
		/// Helper method to log info about a found process
		/// </summary>
		private static void LogFoundProcess(string foundBy, Process process)
		{
			Log.Verbose("Found process {0}: [{1}] {2} ({3}) {4}",
				foundBy,
				process.Get(x => x.Id),
				process.Get(x => x.ProcessName),
				process.Get(x => x.StartTime),
				process.Get(x => x.MainModule?.FileName));
		}

		/// <summary>
		/// Helper method to safely access process properties
		/// </summary>
		private static string Get(this Process process, Func<Process, object> valueGetter)
		{
			try
			{
				return valueGetter(process)?.ToString();
			}
			catch
			{
				return null;
			}
		}
	}
}