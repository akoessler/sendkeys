using System;

namespace SendKeys.Helper
{
	/// <summary>
	/// Struct to hold info about a window
	/// </summary>
	internal class WindowInfo
	{
		/// <summary>
		/// Window handle
		/// </summary>
		public IntPtr Handle { get; set; }

		/// <summary>
		/// Window title
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// ctor.
		/// </summary>
		public WindowInfo(IntPtr handle, string title)
		{
			this.Handle = handle;
			this.Title = title;
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"[{Handle}] {Title}";
		}
	}
}