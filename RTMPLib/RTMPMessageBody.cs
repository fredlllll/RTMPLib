using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RTMPLib
{
	public class RTMPMessageBody
	{
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

		public RTMPLib.BinaryReader BinaryReader
		{
			get;
			private set;
		}

		public RTMPMessageBody(RTMPMessage rTMPMessage, RTMPLib.BinaryReader br)
		{
			ParentMessage = rTMPMessage;
			int remaining = ParentMessage.Header.MessageLengthFromHeader;
			while (remaining > 0)
			{
				byte[] buffer = br.ReadBytes(Math.Min(ParentMessage.Connection.ChunkSize,remaining));
				ms.Write(buffer,0,buffer.Length);
				remaining -= ParentMessage.Connection.ChunkSize;
				if (remaining > 0)
				{
					new RTMPMessageHeader(ParentMessage, br); // read the header for the next message piece
				}
			}
			ms.Position = 0;
			BinaryReader = new BinaryReader(ms);
		}

		public RTMPLib.BinaryWriter BinaryWriter
		{
			get;
			private set;
		}

		public RTMPMessageBody(RTMPMessage rTMPMessage)
		{
			ParentMessage = rTMPMessage;
			BinaryWriter = new RTMPLib.BinaryWriter(ms);
		}

		public byte[] GetBytes()
		{
			return ms.ToArray();
		}

		public void Send(RTMPLib.BinaryWriter bw)
		{
			bw.Write(GetBytes());
		}


		public void Put(String s)
		{
			byte[] tmp = Encoding.UTF8.GetBytes(s);
			BinaryWriter.Write(tmp);
		}

		public String ReadString(int length)
		{
			return Encoding.UTF8.GetString(BinaryReader.ReadBytes(length));
		}

		public AMF0Object ReadAMF0Object()
		{
			try
			{
				return new AMF0Object(this);
			}
			catch
			{
				return null;
			}
		}
	}
}
