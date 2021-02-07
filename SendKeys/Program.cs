using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using SendKeys.Helper;

namespace SendKeys
{
	internal class Program
	{
		/// <summary>
		/// Main entry point. Does argument parsing. Actual program is in Run method.
		/// </summary>
		internal static int Main(string[] args)
		{
			var app = new CommandLineApplication();

			app.Description = @"Sends keystrokes to a target process or window.";
			app.ExtendedHelpText = @"
Searches for a window of another process and sends keystrokes to that window.

If only process (id or name) is provided, keys are sent to the main window of that process.
If only a window title is provided, a window with that title is searched on the desktop.
If both process and window title is provided, a window within that process is searched.

At least one of process or title must be provided to find a target window.

All other arguments, including unknown options, are considered as keys to be sent to the target window.
Keys arguments can be enclosed in quotes, e.g. to include spaces.
Keys are sent in the order they are provided.

Printable keys can be provided as they are. Control keys enclosed with braces, e.g. ""{ENTER}"".
Full key code reference: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.sendkeys.send
";
			app.HelpOption();
			var verbose = app.Option<bool>("-v|--verbose", "Enable verbose logging", CommandOptionType.NoValue);
			var pid = app.Option<int>("-p|--id", "Process id", CommandOptionType.SingleValue);
			var name = app.Option<string>("-n|--name", "Process name", CommandOptionType.SingleValue);
			var title = app.Option<string>("-t|--title", "Window title", CommandOptionType.SingleValue);
			var keys = app.Argument("keys-to-send", "Keys to send", multipleValues: true);
			app.UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue;

			app.OnExecute(() => Run(verbose.HasValue(), pid.ParsedValue, name.ParsedValue, title.ParsedValue, keys.Values.Concat(app.RemainingArguments).ToList()));

			return app.Execute(args);
		}

		/// <summary>
		/// Runs the actual program logic
		/// </summary>
		private static int Run(bool verbose, int pid, string name, string title, List<string> keys)
		{
			Log.VerboseEnabled = verbose;

			if (pid == 0 && string.IsNullOrEmpty(name) && string.IsNullOrEmpty(title))
			{
				Log.Error("No process or window info provided. Please provide a target to send the keys to.");
				return 1;
			}

			var process = ProcessHelper.FindProcess(pid, name);

			if ((process?.Id ?? 0) != 0)
			{
				Log.Info("Process found: [{0}] {1}", process.Id, process.ProcessName);
			}
			else
			{
				Log.Verbose("No process found, cannot proceed");
			}

			var window = WindowHelper.FindWindowHandle(process, title);
			if (window != null && window.Handle != IntPtr.Zero)
			{
				Log.Info("Window found: [{0}] {1}", window.Handle, window.Title);
			}
			else
			{
				Log.Error("No window handle found, cannot proceed");
				return 3;
			}

			WindowHelper.ToForeground(window);

			foreach (var key in keys)
			{
				Log.Info("Send keys: {0}", key);
				System.Windows.Forms.SendKeys.SendWait(key);
				Log.Verbose("done");
			}

			Log.Verbose("all finished");

			return 0;
		}
	}
}
