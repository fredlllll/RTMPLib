/**********************************************************************************************
 * C# RTMPLib - Version 1.0.0
 * by Frederik Gelder <frederik.gelder@freenet.de>
 *
 * Copyright 2015 Frederik Gelder
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 **********************************************************************************************/
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
