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

namespace RTMPLib.Internal
{
	public class RTMPMessageBody
	{
		public RTMPConnection Connection
		{
			get;
			set;
		}

		public RTMPMessage ParentMessage
		{
			get;
			set;
		}

		public int Size
		{
			get
			{
				return (int)ms.Length; // messages cant be bigger than int
			}
		}

		private MemoryStream ms = new MemoryStream();

		public RTMPLib.Internal.BinaryReader MemoryReader
		{
			get;
			private set;
		}

		public RTMPLib.Internal.BinaryWriter MemoryWriter
		{
			get;
			private set;
		}	

		public RTMPMessageBody(RTMPMessage message)
		{
			ParentMessage = message;
			MemoryWriter = new RTMPLib.Internal.BinaryWriter(ms);
		}

		public RTMPMessageBody(RTMPConnection connection, RTMPMessageHeader header, RTMPLib.Internal.BinaryReader br)
		{
			Connection = connection;
			
			byte[] buffer = null;
			if (header.MessageLength < 0) //this is a follow up message
			{
				RTMPChunkStream csinfo = Connection.GetChunkStream((int)header.ChunkStreamID);
				buffer = br.ReadBytes(Math.Min(csinfo.RemainingBytes, Connection.IncomingChunkSize));
			}
			else
			{
				buffer = br.ReadBytes(Math.Min(header.MessageLength, Connection.IncomingChunkSize));
			}
			ms.Write(buffer, 0, buffer.Length);
			/*int remaining = connection.IncomingChunkSize;

			while (remaining > 0)
			{
				byte[] buffer = br.ReadBytes(remaining);
				ms.Write(buffer, 0, buffer.Length);
				remaining -= ParentMessage.Connection.IncomingChunkSize;
			}

			int remaining = ParentMessage.Header.MessageLength;
			while (remaining > 0)
			{
				byte[] buffer = br.ReadBytes(Math.Min(ParentMessage.Connection.IncomingChunkSize,remaining));
				ms.Write(buffer,0,buffer.Length);
				remaining -= ParentMessage.Connection.IncomingChunkSize;
				if (remaining > 0)
				{
					new RTMPMessageHeader(ParentMessage, br); // read the header for the next message piece
				}
			}*/
			ms.Position = 0;
			MemoryReader = new RTMPLib.Internal.BinaryReader(ms);
		}

		public byte[] GetBytes()
		{
			return ms.ToArray();
		}

		public void Send(RTMPLib.Internal.BinaryWriter bw)
		{
			bw.Write(GetBytes());
		}

		public void Put(String s)
		{
			byte[] tmp = Encoding.UTF8.GetBytes(s);
			MemoryWriter.Write(tmp);
		}

		public String ReadString(int length)
		{
			return Encoding.UTF8.GetString(MemoryReader.ReadBytes(length));
		}

		public AMF0Object ReadAMF0Object()
		{
			try
			{
				return new AMF0Object(this);
			}
			catch //yeah this is stupid. basically means this message has no amf0 object. as its binary data you cant really tell whats exactly going wrong
			{
				return null;
			}
		}
	}
}
