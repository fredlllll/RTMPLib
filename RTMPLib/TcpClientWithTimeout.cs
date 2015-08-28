using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace RTMPLib
{
	public class TcpClientWithTimeout
	{
		public TcpClient InternalClient
		{
			get;
			private set;
		}

		public bool Connected
		{
			get;
			private set;
		}

		public IPEndPoint EndPoint
		{
			get;
			private set;
		}
		protected Exception exception;

		public TcpClientWithTimeout(IPEndPoint ipe)
		{
			EndPoint = ipe;
		}

		public void Connect(int timeoutMilliseconds = 2000)
		{
			Connected = false;
			exception = null;
			Thread connectorThread = new Thread(TryConnect);
			connectorThread.IsBackground = true; // So that a failed connection attempt wont prevent the process from terminating while it does a long timeout
			connectorThread.Start();

			// wait for either the thread to finish
			connectorThread.Join(timeoutMilliseconds);

			if (Connected == true)
			{
				// it succeeded
			}
			if (exception != null)
			{
				// it crashed
				throw exception;
			}
			else
			{
				// it timed out
				connectorThread.Abort();
				string message = string.Format("TcpClient connection to {0}:{1} timed out", EndPoint.Address, EndPoint.Port);
				throw new TimeoutException(message);
			}
		}

		protected void TryConnect()
		{
			try
			{
				InternalClient = new TcpClient(EndPoint);
				// record that it succeeded, for the main thread to return to the caller
				Connected = true;
			}
			catch (Exception ex)
			{
				// record the exception for the main thread to re-throw back to the calling code
				exception = ex;
			}
		}
	}
}
