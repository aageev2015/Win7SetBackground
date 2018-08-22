using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Win7SetBackground
{
	static class Program
	{
		const uint SPI_SETDESKWALLPAPER = 0x0014;
		const uint SPI_GETDESKWALLPAPER = 0x0073;
		const uint spif_updateinifile = 0x01;
		const uint spif_sendchange = 0x02;
	
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SystemParametersInfo(uint uiaction, uint uiparam, string pvparam, uint fwinini);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SystemParametersInfo(uint uiaction, uint uiparam, StringBuilder pvparam, uint fwinini);

		static void Main(string[] args)
		{
			handleArgs(args);
#if DEBUG
			Console.ReadLine();
#endif
		}

		static void handleArgs(string[] args)
		{
			if (args.Length == 0)
			{
				printHelp();
				return;
			}

			var firstArg = args[0];
			if (new string[] { @"get", @"--get", @"-get" }.Contains(firstArg))
			{
				var path = getWallpaperPath();
				Console.WriteLine(path);
				return;
			}

			setWallpaperPath(firstArg);
		}


		private static void printHelp()
		{
			Console.WriteLine(
@"Usage:
     {0} get
         returns current wallpaper path
  or {0} [picturePath]
         setup new wallpaper path
", System.Diagnostics.Process.GetCurrentProcess().ProcessName);
		}

#region External invocation utils
		private static string getWallpaperPath()
		{
			var s = new StringBuilder(300);
			bool doneSuccessfuly = SystemParametersInfo(SPI_GETDESKWALLPAPER, 300, s, 0);
			if (!doneSuccessfuly)
			{
				handleExternalException();
			}
			return s.ToString();
		}

		private static void setWallpaperPath(string path)
		{
			bool doneSuccessfuly = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, spif_updateinifile | spif_sendchange);
			if (!doneSuccessfuly)
			{
				handleExternalException();
			}
		}

		private static void handleExternalException()
		{
			var errorCode = Marshal.GetLastWin32Error();
			throw new ExternalException(string.Format("Invocation exeption. user32.dll->SystemParametersInfo. errorCode: {0}", errorCode));

		}
#endregion
	}
}
