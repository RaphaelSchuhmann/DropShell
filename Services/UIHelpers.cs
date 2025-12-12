using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShell.Services
{
	public static class UIHelpers
	{
		public static void ShowMain()
		{
			var wnd = App._mainWindow;

			// use the window's dispatcher explicitly
			var d = wnd.Dispatcher;
			d.Invoke(() =>
			{
				wnd.Show();
			});
		}

		public static void HideMain()
		{
			var wnd = App._mainWindow;

			wnd.Dispatcher.Invoke(() =>
			{
				wnd.Hide();
			});
		}
	}

}
