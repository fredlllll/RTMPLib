using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTMPLib
{
	public class RTMPAcknowledgement : RTMPMessage
	{
		public uint BytesSoFar
		{
			get;
			private set;
		}

		public RTMPAcknowledgement(RTMPConnection connection, uint bytesSoFar):base(connection)
		{
			Header.Format = RTMPMessageFormat.NoMessageId_8;
			Header.MessageTypeID = RTMPMessageTypeID.Acknowledgement;
			Header.ChunkStreamID = RTMPMessageChunkStreamID.LowLevelMessage;
			Body.BinaryWriter.Write(bytesSoFar);
		}

		public RTMPAcknowledgement(RTMPMessage msg):base(msg)
		{
			BytesSoFar = msg.Body.BinaryReader.ReadUInt();
		}
	}
}
