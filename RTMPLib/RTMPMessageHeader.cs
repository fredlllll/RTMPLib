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
	public class RTMPMessageHeader
	{
		public RTMPMessage ParentMessage
		{
			get;
			set;
		}
		public RTMPMessageFormat Format // b11000000 2 bits
		{
			get;
			set;
		}
		public RTMPMessageChunkStreamID ChunkStreamID
		{// = 3; // b00111111 6 bits // 0 = extended with 1 byte, 1 = extended with 2 bytes to hold id, 2 = low level message, else 3 to 63 as ids
			get;
			set;
		}
		public uint Timestamp // 3 or 4 bytes
		{
			get;
			set;
		}
		public int MessageLengthFromHeader
		{
			get;
			private set;
		}

		public int MessageLength // 3 bytes
		{
			get
			{
				if (MessageLengthFromHeader < 0)
				{
					return ParentMessage.Body.Size;
				}
				else
				{
					return MessageLengthFromHeader;
				}
			}
		}
		public RTMPMessageTypeID MessageTypeID // 1 byte
		{
			get;
			set;
		}
		public int MessageStreamID // 4 bytes, (little endian from spec, but value doesnt really matter as it seems?)
		{
			get;
			set;
		}

		public RTMPMessageHeader(RTMPMessage parentMessage)
		{
			ParentMessage = parentMessage;

			Format = RTMPMessageFormat.Undefined;
			ChunkStreamID = RTMPMessageChunkStreamID.Undefined;
			Timestamp = 0;
			MessageLengthFromHeader = -1;
			MessageTypeID = RTMPMessageTypeID.AMF0;
			MessageStreamID = -1;
		}

		public void Send(BinaryWriter bw)
		{
			SendFormatAndChunkStreamID(bw);
			switch (Format)
			{
				case RTMPMessageFormat.BasicHeader_1:
					break;
				case RTMPMessageFormat.BasicHeaderAndTime_4:
					SendTimestamp(bw);
					SendExtendedTimestamp(bw);
					break;
				case RTMPMessageFormat.NoMessageId_8:
					SendTimestamp(bw);
					SendMessageLength(bw);
					SendMessageTypeID(bw);
					SendExtendedTimestamp(bw);
					break;
				case RTMPMessageFormat.FullHeader_12:
					SendTimestamp(bw);
					SendMessageLength(bw);
					SendMessageTypeID(bw);
					SendMessageStreamID(bw);
					SendExtendedTimestamp(bw);
					break;
			}
		}

		public RTMPMessageHeader(RTMPMessage rTMPMessage, BinaryReader br)
		{
			ParentMessage = rTMPMessage;

			//format
			ReadFormatAndChunkStreamID(br);
			RTMPMessageInfo info = ParentMessage.Connection.GetMessageInfo((int)ChunkStreamID);
			//yay code repetition ftw!
			switch (Format)
			{
				case RTMPMessageFormat.BasicHeader_1:
					Timestamp = info.Timestamp;
					MessageLengthFromHeader = info.MessageLength;
					MessageTypeID = info.MessageTypeID;
					MessageStreamID = info.MessageStreamID;
					break;
				case RTMPMessageFormat.BasicHeaderAndTime_4:
					ReadTimestamp(br);
					ReadExtendedTimestamp(br);

					info.Timestamp = Timestamp;
					MessageLengthFromHeader = info.MessageLength;
					MessageTypeID = info.MessageTypeID;
					MessageStreamID = info.MessageStreamID;
					break;
				case RTMPMessageFormat.NoMessageId_8:
					ReadTimestamp(br);
					ReadMessageLength(br);
					ReadMessageTypeID(br);
					ReadExtendedTimestamp(br);

					info.Timestamp = Timestamp;
					info.MessageLength = MessageLengthFromHeader;
					info.MessageTypeID = MessageTypeID;
					MessageStreamID = info.MessageStreamID;
					break;
				case RTMPMessageFormat.FullHeader_12:
					ReadTimestamp(br);
					ReadMessageLength(br);
					ReadMessageTypeID(br);
					ReadMessageStreamID(br);
					ReadExtendedTimestamp(br);

					info.Timestamp = Timestamp;
					info.MessageLength = MessageLengthFromHeader;
					info.MessageTypeID = MessageTypeID;
					info.MessageStreamID = MessageStreamID;
					break;
			}

			/*Timestamp = info.Timestamp;
			MessageLengthFromHeader = info.MessageLength;
			MessageTypeID = info.MessageTypeID;
			MessageStreamID = info.MessageStreamID;*/
		}

		private void ReadFormatAndChunkStreamID(BinaryReader br)
		{
			byte fmt = br.ReadByte();
			Format = (RTMPMessageFormat)(fmt >> 6); // b11000000
			//chunk stream id
			byte csid = (byte)(fmt & 0x3f); // b00111111
			switch (csid)
			{
				case 0: //extended with 1 byte
					byte next1 = br.ReadByte();
					ChunkStreamID = (RTMPMessageChunkStreamID)(64 + next1);
					break;
				case 1: //extended with 2 bytes
					next1 = br.ReadByte();
					byte next2 = br.ReadByte();
					ChunkStreamID = (RTMPMessageChunkStreamID)(64 + next1 + (next2 * 256));
					break;
				case 2: //low level message
					ChunkStreamID = RTMPMessageChunkStreamID.LowLevelMessage;
					break;
				default:
					ChunkStreamID = (RTMPMessageChunkStreamID)csid;
					break;
			}
		}

		private void SendFormatAndChunkStreamID(BinaryWriter bw)
		{
			byte fmt = (byte)((int)Format << 6);
			int chunkStreamId = (int)ChunkStreamID;
			if (chunkStreamId > 1 && chunkStreamId < 64)
			{
				//just put the cs_id over it
				fmt |= (byte)chunkStreamId;
				bw.Write(fmt);
			}
			else if (chunkStreamId > 63 && chunkStreamId < 320)
			{
				//1 byte extension, the cs_id is 0, fmt stays as it is
				byte cs_id = (byte)(chunkStreamId - 64);
				bw.Write(fmt);
				bw.Write(cs_id);
			}
			else if (chunkStreamId > 319 && chunkStreamId < 65600)
			{
				//2 byte extension, the cs_id is 1
				fmt |= (byte)1;
				/*id = ((the thirdbyte)*256 + the second byte + 64
				 *id -64 = third*256 + second
				 */
				int cs_id = chunkStreamId - 64;
				byte cs_id2 = (byte)(cs_id & 0xFF);
				byte cs_id3 = (byte)(cs_id / 256);
				bw.Write(fmt);
				bw.Write(cs_id2);
				bw.Write(cs_id3);
			}
			else
			{
				throw new NotSupportedException("only chunk stream ids from 2 to 65599 are supported (0 and 1 are reserved[and are calculated automatically], while 2 is a special stream for 'low level messages')");
			}
		}

		private bool extendedTimeStamp = false;
		private void ReadTimestamp(BinaryReader br)
		{
			// timestamp
			byte[] tmstmp = br.ReadBytes(3);
			Timestamp = 0;
			Timestamp |= (uint)(tmstmp[0] << 16);
			Timestamp |= (uint)(tmstmp[1] << 8);
			Timestamp |= (uint)tmstmp[2];
			extendedTimeStamp = Timestamp == 0xFFFFFF; // extended timestamp, 4 bytes
		}

		private void SendTimestamp(BinaryWriter bw)
		{
			extendedTimeStamp = Timestamp >= 0xFFFFFF; // extended timestamp, 4 bytes
			byte[] bytes = new byte[3];
			if (!extendedTimeStamp)
			{
				bytes[0] = (byte)(Timestamp >> 16);
				bytes[1] = (byte)(Timestamp >> 8);
				bytes[2] = (byte)Timestamp;
			}
			else
			{ //send 0xFFFFFF
				bytes[0] = 0xFF;
				bytes[1] = 0xFF;
				bytes[2] = 0xFF;
			}
			bw.Write(bytes);
		}

		private void ReadExtendedTimestamp(BinaryReader br)
		{
			if (extendedTimeStamp)
			{
				byte[] tmstmp = br.ReadBytes(4);
				Timestamp = 0;
				Timestamp |= (uint)(tmstmp[0] << 24);
				Timestamp |= (uint)(tmstmp[1] << 16);
				Timestamp |= (uint)(tmstmp[2] << 8);
				Timestamp |= (uint)tmstmp[3];
			}
		}

		private void SendExtendedTimestamp(BinaryWriter bw)
		{
			if (extendedTimeStamp)
			{
				byte[] bytes = new byte[4];
				bytes[0] = (byte)(Timestamp >> 24);
				bytes[1] = (byte)(Timestamp >> 16);
				bytes[2] = (byte)(Timestamp >> 8);
				bytes[3] = (byte)Timestamp;
				bw.Write(bytes);
			}
		}

		private void ReadMessageLength(BinaryReader br)
		{
			byte[] bodysize = br.ReadBytes(3);
			MessageLengthFromHeader = 0;
			MessageLengthFromHeader |= bodysize[0] << 16;
			MessageLengthFromHeader |= bodysize[1] << 8;
			MessageLengthFromHeader |= bodysize[2];
		}

		private void SendMessageLength(BinaryWriter bw)
		{
			byte[] bytes = new byte[3];
			int msgLen = MessageLength;
			bytes[0] = (byte)(msgLen >> 16);
			bytes[1] = (byte)(msgLen >> 8);
			bytes[2] = (byte)msgLen;
			bw.Write(bytes);
		}

		private void ReadMessageTypeID(BinaryReader br)
		{
			MessageTypeID = (RTMPMessageTypeID)br.ReadByte();
		}

		private void SendMessageTypeID(BinaryWriter bw)
		{
			bw.Write((byte)MessageTypeID);
		}

		private void ReadMessageStreamID(BinaryReader br)
		{
			br.InputEndianness = Endianness.LittleEndian;
			MessageStreamID = br.ReadInt(); // little endian??? crzy people
			br.InputEndianness = Endianness.BigEndian;
		}

		private void SendMessageStreamID(BinaryWriter bw)
		{
			bw.OutputEndianness = Endianness.LittleEndian;
			bw.Write(MessageStreamID);
			bw.OutputEndianness = Endianness.BigEndian;
		}
	}
}
