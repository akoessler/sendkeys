using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SendKeys.Helper
{
	/// <summary>
	/// Helper class for window stuff
	/// </summary>
	internal static class WindowHelper
	{
		/// <summary>
		/// Find a window by process and/or window title
		/// </summary>
		/// <param name="process">If set, search window within that process. If null or invalid, search on desktop.</param>
		/// <param name="windowTitle">Title of the window to search for.</param>
		public static WindowInfo FindWindowHandle(Process process, string windowTitle)
		{
			var mainWindowHandle = process?.MainWindowHandle ?? IntPtr.Zero;
			var mainWindowResult = new WindowInfo(mainWindowHandle, GetWindowText(mainWindowHandle));
			if (string.IsNullOrEmpty(windowTitle))
			{
				Log.Verbose("No title provided -> use main window handle");
				return mainWindowResult;
			}
			else
			{
				return FindFirstWindowByTitle(mainWindowResult, windowTitle);
			}
		}

		/// <summary>
		/// Put the window into foreground
		/// </summary>
		public static void ToForeground(WindowInfo findWindowResult)
		{
			Log.Info("Set window to foreground: {0}", findWindowResult);
			Native.SetForegroundWindow(findWindowResult.Handle);
			Log.Verbose("done");
		}

		/// <summary>
		/// Search for the window
		/// </summary>
		private static WindowInfo FindFirstWindowByTitle(WindowInfo parentWindow, string titleToSearch)
		{
			WindowInfo result = null;

			Log.Verbose("Search for window with title: {0}", titleToSearch);
			Log.Verbose("Enum child windows of parent window {0}", parentWindow);

			Native.EnumChildWindows(parentWindow.Handle, (windowHandle, lParam) =>
			{
				var windowTitle = GetWindowText(windowHandle);
				if (windowTitle?.Contains(titleToSearch, StringComparison.InvariantCultureIgnoreCase) == true)
				{
					result = new WindowInfo(windowHandle, windowTitle);
					return false;
				}

				Log.Verbose("not matching window [{0}] {1}", windowHandle, windowTitle);
				return true; // return true to iterate all windows
			}, IntPtr.Zero);

			if (result != null)
			{
				Log.Verbose("Found a matching window: {0}", result);
				return result;
			}
			else
			{
				Log.Info("Did not find a matching window for title: {0}", titleToSearch);
				return null;
			}
		}

		/// <summary>
		/// Get window title from a window handle
		/// </summary>
		private static string GetWindowText(IntPtr hWnd)
		{
			int length = Native.GetWindowTextLength(hWnd);
			if (length > 0)
			{
				var sb = new StringBuilder(length + 1);
				Native.GetWindowText(hWnd, sb, sb.Capacity);
				return sb.ToString();
			}

			return string.Empty;
		}

		/// <summary>
		/// Native access to winapi function for accessing the window information
		/// </summary>
		private static class Native
		{
			[DllImport("User32.dll")]
			public static extern int SetForegroundWindow(IntPtr windowHandle);

			[DllImport("user32.dll", CharSet = CharSet.Unicode)]
			public static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

			[DllImport("user32.dll", CharSet = CharSet.Unicode)]
			public static extern int GetWindowTextLength(IntPtr hWnd);

			[DllImport("user32.dll")]
			public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc enumProc, IntPtr lParam);

			public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
		}
	}
}
