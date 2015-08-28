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
using System.Net;
using System.Net.Sockets;
using RTMPLib.Protocol;

namespace RTMPLib
{
	public class RTMPConnection
	{
		public TcpClient InternalTcpClient
		{
			get;
			private set;
		}

		public CountingStream Counter
		{
			get;
			private set;
		}

		private BinaryWriter bw;
		public BinaryWriter InternalBinaryWriter
		{
			get
			{
				return bw;
			}
			private set
			{
				bw = value;
			}
		}

		private BinaryReader br;
		public BinaryReader InternalBinaryReader
		{
			get
			{
				return br;
			}
			private set
			{
				br = value;
			}
		}

		public RTMPConnection(IPAddress address, int port, int timeoutMilliseconds = 2000)
		{
			TcpClientWithTimeout btc = new TcpClientWithTimeout(new IPEndPoint(address, port));
			btc.Connect(timeoutMilliseconds);
			InternalTcpClient = btc.InternalClient;
			Counter = new CountingStream(InternalTcpClient.GetStream());
			InternalBinaryReader = new BinaryReader(Counter);
			InternalBinaryWriter = new BinaryWriter(Counter);

			ChunkSize = 128;
		}

		public void DoHandshake(Handshake handshake)
		{
			handshake.Do(bw, br);
		}

		private Dictionary<int, RTMPMessageInfo> messageInfoForChunk = new Dictionary<int, RTMPMessageInfo>();
		public RTMPMessageInfo GetMessageInfo(int chunkStreamID)
		{
			RTMPMessageInfo info;
			if (messageInfoForChunk.ContainsKey(chunkStreamID))
			{
				info = messageInfoForChunk[chunkStreamID];
			}
			else
			{
				info = new RTMPMessageInfo();
				messageInfoForChunk[chunkStreamID] = info;
			}
			return info;
		}

		public RTMPMessage ReceiveMessage()
		{
			RTMPMessage newmessage = new RTMPMessage(this, br);
			//SetLastMessage((int)newmessage.Header.ChunkStreamID, newmessage);
			return newmessage;
		}

		public void SendMessage(RTMPMessage message)
		{
			throw new NotImplementedException();
		}

		public int ChunkSize {
			get; 
			set;
		}
	}
}
