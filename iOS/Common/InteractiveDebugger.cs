using System;
using System.Net.Sockets;
using Foundation;
using System.Net;
using System.Threading;
using System.Text;
using System.Linq;

namespace KinderChat.iOS
{
	public class ServerEventArgs : EventArgs
	{
		public string Request { get; set; }
	}

	public class Server : NSNetServiceDelegate
	{
		public event EventHandler<ServerEventArgs> OnRequest;

		NSNetService service;
		TcpListener tcpListener;

		public Server ()
		{

		}

		public void Start (string name)
		{
			service = new NSNetService ("", "_debugger._tcp.", name, 0);

			service.Delegate = this;
			service.Publish (NSNetServiceOptions.ListenForConnections);
		}

		public override void Published (NSNetService sender)
		{
			ThreadPool.QueueUserWorkItem (_ => {
				if(!TryCreateListener (sender, out tcpListener))
					return;

				tcpListener.Start ();

				byte[] buffer = new byte[1024];
				while (true) {
					using (TcpClient client = tcpListener.AcceptTcpClient ()) {
						using (NetworkStream strem = client.GetStream ()) {
							var size = strem.Read (buffer, 0, buffer.Length);
							string request = Encoding.ASCII.GetString (buffer, 0, size);
							RaiseRequestEvent (request);
						}
					}
				}
			});
		}

		public override void PublishFailure (NSNetService sender, NSDictionary errors)
		{
			Console.WriteLine ("PublishFailure {0}", errors);
		}

		bool TryCreateListener (NSNetService sender, out TcpListener listener)
		{
			try {
				listener = CreateListener((int)sender.Port);
				return true;
			} catch (Exception e) {
				Console.WriteLine (e);
				listener = null;
				return false;
			}
		}

		TcpListener CreateListener(int port) {
			IPHostEntry hostEntry = Dns.GetHostEntry (Dns.GetHostName ());
			var addressList = hostEntry.AddressList.Where (ip => ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6);
			IPAddress serverAddres = addressList.First ();

			Console.WriteLine (string.Format ("server started {0}:{1}", serverAddres.ToString (), port));

			return new TcpListener (serverAddres, port);
		}

		void RaiseRequestEvent (string request)
		{
			var handler = OnRequest;
			if (handler != null)
				handler (this, new ServerEventArgs { Request = request });
		}
	}}

