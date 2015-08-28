using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using RTMPLib.Protocol;
using System.Threading;

namespace RTMPLib
{
	public class RTMPInterface
	{
		public RTMPConnection Connection
		{
			get;
			private set;
		}

		public Handshake Handshake
		{
			get;
			set;
		}

		private Thread readerThread;

		public RTMPInterface()
		{
			Handshake = null;
		}

		public void Connect(IPAddress ip, int port,int timeoutMilliseconds)
		{
			Connection = new RTMPConnection(ip, port, timeoutMilliseconds);
			if (Handshake == null)
			{
				Handshake = new Handshake();
			}
			Connection.DoHandshake(Handshake);

			readerThread = new Thread(reader_run);
			readerThread.IsBackground = true;
			readerThread.Start();
		}

		public delegate void MessageReceivedHandler(RTMPMessage message);
		public event MessageReceivedHandler MessageReceived;

		private void reader_run()
		{
			try
			{
				while (true)
				{
					RTMPMessage msg = Connection.ReceiveMessage();
					msg = RTMPMessage.Decode(msg);
					if (MessageReceived != null)
					{
						MessageReceived(msg);
					}
				}
			}
			catch
			{
				//lalala i dont care bye bye
			}
		}

		public void SendMessage(RTMPMessage message)
		{
			Connection.SendMessage(message);
		}
	}
}
