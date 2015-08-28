using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTMPLib
{
	public class RTMPSetChunkSize : RTMPMessage
	{

		public uint ChunkSize
		{
			get;
			private set;
		}

		public RTMPSetChunkSize(RTMPConnection connection, uint chunkSize)
			: base(connection)
		{
			Header.MessageTypeID = RTMPMessageTypeID.SetChunkSize;
			ChunkSize = chunkSize;
			Body.BinaryWriter.Write(chunkSize);
		}

		public RTMPSetChunkSize(RTMPMessage msg)
			: base(msg)
		{
			ChunkSize = msg.Body.BinaryReader.ReadUInt();
		}
	}
}
