using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTMPLib
{
	public class RTMPAudioMessage : RTMPMessage
	{

		public byte Format
		{
			get;
			private set;
		}

		public byte[] Data
		{
			get;
			private set;
		}


		public RTMPAudioMessage(RTMPConnection connection,int chunkStream, byte format, byte[] data)
			: base(connection)
		{
			Header.Format = RTMPMessageFormat.NoMessageId_8;
			Header.MessageTypeID = RTMPMessageTypeID.AudioPacket;
			Header.ChunkStreamID = (RTMPMessageChunkStreamID)chunkStream;

			Format = format;
			Data = data;
			Body.BinaryWriter.Write(format);
			Body.BinaryWriter.Write(data);
		}
		

		public RTMPAudioMessage(RTMPMessage msg) : base(msg)
		{
			Format = msg.Body.BinaryReader.ReadByte();
			Data = msg.Body.BinaryReader.ReadBytes(msg.Header.MessageLengthFromHeader-1);
		}
	}
}
