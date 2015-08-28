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
