using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ExactDropboxSyncer.UI
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			string baseAddress = "http://localhost:12345/";
			HttpListener listener = new HttpListener();
			listener.Prefixes.Add(baseAddress);
			listener.Start();

			ThreadStart threadStart = () => {
				while (true) {
					try {
						HttpListenerContext context = listener.GetContext();
						context.Response.OutputStream.Close();
					}
					catch { }
				}
			};
			var thread = new Thread(threadStart);
			thread.Start();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm(new FileSyncerFactory()));

			listener.Stop();
			listener.Close();
			thread.Abort();
		}
	}
}
